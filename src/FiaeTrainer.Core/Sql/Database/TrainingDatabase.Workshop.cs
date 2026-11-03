using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using FiaeTrainer.Core.Sql.Content;
using FiaeTrainer.Core.Sql.Results;
using FiaeTrainer.Core.Sql.Workshop;

namespace FiaeTrainer.Core.Sql.Database;

public sealed partial class TrainingDatabase
{
    /// <summary>
    /// Führt ein geprüftes Werkstattskript in einer frischen, nicht gepoolten Sitzung aus.<br/>
    /// Dauerhafte Daten bleiben auch bei COMMIT unverändert, weil sämtliche Ziele sitzungseigene Tabellen sind.
    /// </summary>
    public async Task<WorkshopRun> ExecuteWorkshopAsync(string sessionSql, bool verificationData = false, CancellationToken cancellationToken = default)
    {
        CancellationToken requestToken = cancellationToken;
        using CancellationTokenSource deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(TimeSpan.FromSeconds(8));
        cancellationToken = deadline.Token;
        SqlConnectionStringBuilder builder = GetConnectionBuilder();
        builder.Pooling = false;
        await using SqlConnection connection = new(builder.ConnectionString);
        try
        {
            await connection.OpenAsync(cancellationToken);
            await using (SqlCommand restrict = new("EXECUTE AS USER='TrainerReader' WITH NO REVERT; SET QUOTED_IDENTIFIER ON; SET DATEFORMAT ymd;", connection))
                await restrict.ExecuteNonQueryAsync(cancellationToken);
            using Stream stream = SqlContent.Open("workshop.sql");
            string seed = await new StreamReader(stream).ReadToEndAsync(cancellationToken);
            if (verificationData)
                seed += " UPDATE #Artikel SET Preis=Preis+2, Bestand=Bestand+1 WHERE Id IN (1,2); INSERT #Artikel VALUES(6,N'Zusatzsensor',N'Sensoren',70.00,2,1),(7,N'Alte Hülle',N'Zubehör',8.00,0,0); UPDATE #Konten SET Guthaben=Guthaben+175 WHERE Id=1; UPDATE #Konten SET Guthaben=Guthaben+80 WHERE Id=2;";
            await using (SqlCommand setup = new(seed, connection) { CommandTimeout = 4 })
                await setup.ExecuteNonQueryAsync(cancellationToken);
            WorkshopRun run = new();
            if (!string.IsNullOrWhiteSpace(sessionSql))
            {
                TSqlScript parsed = (TSqlScript)new TSql160Parser(true).Parse(new StringReader(sessionSql), out _);
                foreach (TSqlStatement statement in parsed.Batches[0].Statements)
                {
                    if (statement is RollbackTransactionStatement)
                        foreach (string table in new[] { "Artikel", "Konten" })
                            run.BeforeRollback[table] = await ReadWorkshopTableAsync(connection, $"SELECT * FROM [#{table}] ORDER BY Id", cancellationToken);
                    new Sql160ScriptGenerator().GenerateScript(statement, out string sql);
                    await using SqlCommand command = new(sql, connection) { CommandTimeout = 4 };
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
            await using (SqlCommand transactions = new("SELECT @@TRANCOUNT", connection))
                if (Convert.ToInt32(await transactions.ExecuteScalarAsync(cancellationToken)) != 0)
                    return new WorkshopRun { Error = "Deine Transaktion ist noch offen. Schließe sie mit COMMIT oder ROLLBACK ab. Die Arbeitskopie wird verworfen." };

            foreach (string table in WorkshopSqlValidator.TableNames)
            {
                await using SqlCommand exists = new("SELECT OBJECT_ID(@name)", connection);
                exists.Parameters.AddWithValue("@name", "tempdb..#" + table);
                if (await exists.ExecuteScalarAsync(cancellationToken) is null or DBNull)
                    continue;
                run.Tables[table] = await ReadWorkshopTableAsync(connection, $"SELECT * FROM [#{table}] ORDER BY Id", cancellationToken);
                const string STRUCTURE_SQL = "SELECT c.name AS Spalte,t.name AS Typ, CASE WHEN t.name=N'nvarchar' THEN c.max_length/2 ELSE c.max_length END AS Laenge,c.precision AS Praezision,c.scale AS Nachkommastellen,c.is_nullable AS NullErlaubt,CONVERT(bit,CASE WHEN EXISTS(SELECT 1 FROM tempdb.sys.index_columns ic JOIN tempdb.sys.indexes i ON i.object_id=ic.object_id AND i.index_id=ic.index_id WHERE ic.object_id=c.object_id AND ic.column_id=c.column_id AND i.is_primary_key=1) THEN 1 ELSE 0 END) AS Primaerschluessel FROM tempdb.sys.columns c JOIN tempdb.sys.types t ON c.user_type_id=t.user_type_id WHERE c.object_id=OBJECT_ID(@name) ORDER BY c.column_id";
                run.Structures[table] = await ReadWorkshopTableAsync(connection, STRUCTURE_SQL, cancellationToken, "tempdb..#" + table);
            }

            run.ConstraintChecks.Columns.AddRange(["Prüffall", "Einfügen erlaubt"]);
            await AddConstraintProbeAsync(connection, run, "Artikel: negativer Bestand", "INSERT #Artikel (Id,Name,Kategorie,Preis,Bestand,Aktiv) VALUES(999,N'Prüfung',N'Prüfung',1,-1,1)", cancellationToken);
            await AddConstraintProbeAsync(connection, run, "Artikel: Bestand null Stück", "INSERT #Artikel (Id,Name,Kategorie,Preis,Bestand,Aktiv) VALUES(999,N'Prüfung',N'Prüfung',1,0,1)", cancellationToken);
            if (run.Tables.ContainsKey("Rueckgaben"))
            {
                await AddConstraintProbeAsync(connection, run, "Rückgabe: gültige Menge", "INSERT #Rueckgaben (Id,Grund,Menge) VALUES(999,N'Prüfung',1)", cancellationToken);
                await AddConstraintProbeAsync(connection, run, "Rückgabe: Menge 0", "INSERT #Rueckgaben (Id,Grund,Menge) VALUES(999,N'Prüfung',0)", cancellationToken);
                await AddConstraintProbeAsync(connection, run, "Rückgabe: negative Menge", "INSERT #Rueckgaben (Id,Grund,Menge) VALUES(999,N'Prüfung',-1)", cancellationToken);
                await AddConstraintProbeAsync(connection, run, "Rückgabe: fehlender Grund", "INSERT #Rueckgaben (Id,Grund,Menge) VALUES(999,NULL,1)", cancellationToken);
                await AddConstraintProbeAsync(connection, run, "Rückgabe: doppelte Id", "INSERT #Rueckgaben (Id,Grund,Menge) VALUES(999,N'A',1),(999,N'B',1)", cancellationToken);
            }
            return run;
        }
        catch (SqlException exception)
        {
            return new WorkshopRun { Error = ExplainError(exception.Number, sessionSql), TechnicalFailure = exception.Number is 53 or -1 or 26 or 4060 or 18456 or 10060 or 10061 };
        }
        catch (OperationCanceledException) when (!requestToken.IsCancellationRequested)
        {
            return new WorkshopRun { Error = "Das Skript wurde nach acht Sekunden beendet. Vereinfache es und prüfe besonders ungewollte Tabellenkombinationen." };
        }
    }

    /// <summary> Liest höchstens 200 Zeilen der Arbeitskopie oder ihrer Struktur; ein größerer Stand wird als gekürzt markiert. </summary>
    private static async Task<QueryResult> ReadWorkshopTableAsync(SqlConnection connection, string sql, CancellationToken token, string? objectName = null)
    {
        await using SqlCommand command = new(sql, connection) { CommandTimeout = 4 };
        if (objectName != null)
            command.Parameters.AddWithValue("@name", objectName);
        await using SqlDataReader reader = await command.ExecuteReaderAsync(token);
        QueryResult result = new();
        for (int index = 0; index < reader.FieldCount; index++)
            result.Columns.Add(reader.GetName(index));
        while (await reader.ReadAsync(token))
        {
            if (result.Rows.Count == MAX_ROWS)
                return new QueryResult { Columns = result.Columns, Rows = result.Rows, Truncated = true };
            object?[] row = new object?[reader.FieldCount];
            for (int index = 0; index < reader.FieldCount; index++)
                row[index] = reader.IsDBNull(index) ? null : reader.GetValue(index);
            result.Rows.Add(row);
        }
        return result;
    }

    /// <summary> Prüft eine Einfügung innerhalb einer zurückgenommenen Transaktion und wertet nur erwartete Constraint-Fehler als Ablehnung. </summary>
    private static async Task AddConstraintProbeAsync(SqlConnection connection, WorkshopRun run, string label, string insertSql, CancellationToken token)
    {
        // Jeder Prüfversuch wird zurückgerollt. Fehlende Datenspalten bleiben echte Fehler und werden nicht als Constraint gewertet.
        string probeSql = $"SET XACT_ABORT OFF; DECLARE @allowed bit=1; BEGIN TRANSACTION; BEGIN TRY {insertSql}; END TRY BEGIN CATCH IF ERROR_NUMBER() IN(515,547,2601,2627) SET @allowed=0; ELSE THROW; END CATCH; IF @@TRANCOUNT>0 ROLLBACK; SELECT @allowed;";
        await using SqlCommand command = new(probeSql, connection) { CommandTimeout = 4 };
        object? allowed = await command.ExecuteScalarAsync(token);
        run.ConstraintChecks.Rows.Add([label, Convert.ToBoolean(allowed)]);
    }
}
