using FiaeTrainer.Core.Sql.Exams;
using Microsoft.Data.SqlClient;

namespace FiaeTrainer.Core.Sql.Database;

public sealed partial class TrainingDatabase
{
    /// <summary> Führt jeden Prüfversuch unter eingeschränkten Rechten in einer neuen Sitzung mit ausschließlich temporären Tabellen aus. </summary>
    public async Task<SqlExamRun> ExecuteExamAsync(SqlExamCase item, SqlExamTask task, string answer, bool verification = false)
    {
        string? validation = SqlExamValidator.Prepare(item, task, answer, out string sql);
        if (validation != null) return new() { Error = validation };
        SqlConnectionStringBuilder builder = GetConnectionBuilder();
        builder.Pooling = false;
        using CancellationTokenSource deadline = new(TimeSpan.FromSeconds(8));
        await using SqlConnection connection = new(builder.ConnectionString);
        try
        {
            await connection.OpenAsync(deadline.Token);
            await using (SqlCommand restrict = new("EXECUTE AS USER='TrainerReader' WITH NO REVERT; SET QUOTED_IDENTIFIER ON; SET DATEFORMAT ymd;", connection)) await restrict.ExecuteNonQueryAsync(deadline.Token);
            await using (SqlCommand seed = new(item.SeedSql + (verification ? item.VerificationSql : ""), connection) { CommandTimeout = 4 }) await seed.ExecuteNonQueryAsync(deadline.Token);
            SqlExamRun result = new();
            if (task.Operation == "Query")
            {
                result.Query = await ReadWorkshopTableAsync(connection, sql, deadline.Token);
                if (result.Query.Truncated) result.Error = "Das Ergebnis überschreitet 200 Zeilen.";
            }
            else
            {
                await using (SqlCommand command = new(sql, connection) { CommandTimeout = 4 }) await command.ExecuteNonQueryAsync(deadline.Token);
                foreach (string table in item.Tables)
                {
                    result.State[table] = await ReadWorkshopTableAsync(connection, $"SELECT * FROM [#{table}] ORDER BY Id", deadline.Token);
                    const string COLUMNS = "SELECT c.name AS Spalte,t.name AS Typ,CASE WHEN t.name=N'nvarchar' THEN c.max_length/2 ELSE c.max_length END AS Laenge,c.precision AS Praezision,c.scale AS Nachkommastellen,c.is_nullable AS NullErlaubt FROM tempdb.sys.columns c JOIN tempdb.sys.types t ON c.user_type_id=t.user_type_id WHERE c.object_id=OBJECT_ID(@name) ORDER BY c.column_id";
                    result.State[table + ":Spalten"] = await ReadWorkshopTableAsync(connection, COLUMNS, deadline.Token, "tempdb..#" + table);
                }
            }
            return result;
        }
        catch (SqlException exception) { return new() { Error = ExplainError(exception.Number, answer), TechnicalFailure = exception.Number is 53 or -1 or 26 or 4060 or 18456 or 10060 or 10061 }; }
        catch (OperationCanceledException) { return new() { Error = "Die SQL-Ausführung wurde nach acht Sekunden beendet." }; }
    }
}
