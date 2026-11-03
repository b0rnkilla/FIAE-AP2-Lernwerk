using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using FiaeTrainer.Core.Sql.Content;
using FiaeTrainer.Core.Sql.Results;

namespace FiaeTrainer.Core.Sql.Database;

/// <summary> Erstellt ausschließlich die eigene Trainingsdatenbank und führt begrenzte Lernabfragen aus. </summary>
public sealed partial class TrainingDatabase(string connectionString)
{
    #region Fields

    /// <summary> Unveränderlicher Name verhindert eine versehentliche Initialisierung fremder Datenbanken. </summary>
    public const string DATABASE_NAME = "FIAE_AP2_Training";
    /// <summary> Harte Begrenzung von Ergebnissen im Trainer. </summary>
    public const int MAX_ROWS = 200;
    /// <summary> Kennzeichnet die von dieser Anwendung angelegte Datenbank. </summary>
    private const string DATABASE_MARKER = "FIAE-AP2-Trainer-v1";

    #endregion

    #region Methods

    /// <summary> Erstellt eine fehlende Trainingsdatenbank; bestehende fremde Datenbanken werden niemals initialisiert. </summary>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        SqlConnectionStringBuilder builder = GetConnectionBuilder();
        builder.InitialCatalog = "master";
        await using (SqlConnection master = new(builder.ConnectionString))
        {
            await master.OpenAsync(cancellationToken);
            await using SqlCommand check = new("SELECT DB_ID(@name)", master);
            check.Parameters.AddWithValue("@name", DATABASE_NAME);
            object? databaseId = await check.ExecuteScalarAsync(cancellationToken);
            if (databaseId is null or DBNull)
            {
                await using SqlCommand create = new($"CREATE DATABASE [{DATABASE_NAME}]", master) { CommandTimeout = 30 };
                await create.ExecuteNonQueryAsync(cancellationToken);
                builder.InitialCatalog = DATABASE_NAME;
                await using SqlConnection created = new(builder.ConnectionString);
                await created.OpenAsync(cancellationToken);
                await using SqlCommand mark = new("EXEC sys.sp_addextendedproperty @name=N'TrainerIdentity', @value=@marker;", created);
                mark.Parameters.AddWithValue("@marker", DATABASE_MARKER);
                await mark.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        await using SqlConnection connection = new(GetConnectionBuilder().ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using SqlCommand marker = new("SELECT CONVERT(nvarchar(100), value) FROM sys.extended_properties WHERE class=0 AND name=N'TrainerIdentity'", connection);
        if (!string.Equals(await marker.ExecuteScalarAsync(cancellationToken) as string, DATABASE_MARKER, StringComparison.Ordinal))
            throw new InvalidOperationException("Eine gleichnamige Datenbank existiert bereits, gehört aber nicht zum Trainer. Sie wird nicht verändert.");

        using Stream stream = SqlContent.Open("database.sql");
        string sql = await new StreamReader(stream).ReadToEndAsync(cancellationToken);
        await using SqlCommand initialize = new(sql, connection) { CommandTimeout = 30 };
        await initialize.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary> Führt bereits validiertes SQL unter einem Datenbankbenutzer ohne Login und ohne Schreibrechte aus. </summary>
    public async Task<QueryResult> ExecuteAsync(string validatedSql, CancellationToken cancellationToken = default)
    {
        SqlConnectionStringBuilder builder = GetConnectionBuilder();
        // Keine Wiederverwendung einer Verbindung mit unwiderruflich eingeschränktem Ausführungskontext.
        builder.Pooling = false;
        await using SqlConnection connection = new(builder.ConnectionString);
        try
        {
            await connection.OpenAsync(cancellationToken);
            await using (SqlCommand restrict = new("EXECUTE AS USER = 'TrainerReader' WITH NO REVERT; SET QUOTED_IDENTIFIER ON; SET DATEFORMAT ymd; SET LOCK_TIMEOUT 1000; SET ROWCOUNT 201;", connection))
                await restrict.ExecuteNonQueryAsync(cancellationToken);

            await using SqlCommand command = new(validatedSql, connection) { CommandTimeout = 4 };
            await using SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
            QueryResult result = new();
            for (int index = 0; index < reader.FieldCount; index++)
                result.Columns.Add(reader.GetName(index));
            while (await reader.ReadAsync(cancellationToken))
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
        catch (SqlException exception)
        {
            return new QueryResult { Error = ExplainError(exception.Number, validatedSql), TechnicalFailure = exception.Number is 53 or -1 or 26 or 4060 or 18456 or 10060 or 10061 };
        }
    }

    /// <summary> Verwirft Verbindungen, deren Datenbankname nicht exakt der eigenen Trainingsdatenbank entspricht. </summary>
    private SqlConnectionStringBuilder GetConnectionBuilder()
    {
        SqlConnectionStringBuilder builder = new(connectionString);
        if (!string.Equals(builder.InitialCatalog, DATABASE_NAME, StringComparison.Ordinal))
            throw new InvalidOperationException($"Als Datenbank ist ausschließlich {DATABASE_NAME} erlaubt.");
        return builder;
    }

    /// <summary> Übersetzt bekannte SQL-Server-Fehler in Lernhinweise, ohne interne Datenbankdetails anzuzeigen. </summary>
    private static string ExplainError(int number, string sql) => number switch
    {
        207 when HasDoubleQuotedColumn(sql) => "Ein Spaltenname wurde nicht gefunden. Du verwendest doppelte Anführungszeichen (\"…\") für einen Spaltenbezug. Falls du einen Textwert meinst, verwende einfache Anführungszeichen: zum Beispiel WHERE Status = 'verschwunden'. Mit \"verschwunden\" sucht SQL Server eine Spalte dieses Namens. Prüfe andernfalls die Spaltennamen in der Tabellenübersicht.",
        207 => "Ein Spaltenname wurde nicht gefunden. Vergleiche deine Schreibweise mit der Tabellenübersicht.",
        208 => "Die Tabelle wurde nicht gefunden. Vergleiche den Namen mit der Übersicht, zum Beispiel Sendungen.",
        209 => "Ein Spaltenname ist mehrdeutig. Ergänze den Tabellenalias, zum Beispiel m.Id.",
        4104 => "Ein Tabellenalias oder Spaltenbezug ist nicht zugeordnet. Prüfe FROM, JOIN und deine Aliase.",
        8120 => "Eine ausgewählte Spalte fehlt in GROUP BY oder in einer Aggregatfunktion. Jede Gruppe muss einen eindeutigen Ergebniswert liefern.",
        147 => "Aggregatfunktionen wie COUNT gehören hier in HAVING statt in WHERE.",
        102 or 156 => "Die SQL-Schreibweise ist noch nicht korrekt. Prüfe Kommas, Klammern und die Reihenfolge der Klauseln.",
        245 or 8114 => "Datentypen passen nicht zusammen. Texte und Datumswerte stehen in einfachen Anführungszeichen.",
        515 => "Eine Pflichtspalte darf nicht NULL sein. Prüfe NOT NULL und deine eingefügten Werte.",
        547 => "Eine Datenregel wurde verletzt. Prüfe CHECK-Bedingungen und zulässige Werte.",
        2627 or 2601 => "Ein Schlüsselwert ist bereits vorhanden. PRIMARY KEY und UNIQUE verlangen eindeutige Werte.",
        2714 => "Diese Übungstabelle existiert bereits. Jede Ausführung beginnt mit den angegebenen Ausgangstabellen.",
        2705 => "Eine Spalte mit diesem Namen existiert bereits. Prüfe die Tabellenstruktur.",
        3902 or 3903 => "COMMIT oder ROLLBACK benötigt eine zuvor begonnene Transaktion.",
        229 or 916 => "Dieser Zugriff ist in der Trainingsdatenbank nicht erlaubt. Nutze ausschließlich die Falltabellen.",
        -2 or 1222 => "Die Abfrage wurde wegen der Zeitbegrenzung beendet. Prüfe besonders fehlende JOIN-Bedingungen.",
        53 or -1 or 26 or 4060 or 18456 => "Die Verbindung zum SQL Server ist nicht verfügbar. Prüfe die Datenbankverbindung auf der Startseite.",
        _ => $"SQL Server konnte die Abfrage nicht ausführen (Fehler {number}). Prüfe die Spalten, Datentypen und SQL-Bausteine."
    };

    /// <summary> Erkennt doppelt zitierte Spalten im Syntaxbaum; Kommentare, Textinhalte und Ausgabealiase zählen nicht dazu. </summary>
    private static bool HasDoubleQuotedColumn(string sql)
    {
        TSqlFragment fragment = new TSql160Parser(true).Parse(new StringReader(sql), out _);
        DoubleQuotedColumnVisitor visitor = new();
        fragment.Accept(visitor);
        return visitor.Found;
    }

    /// <summary> Unterscheidet tatsächlich zitierte Spaltenbezüge von Anführungszeichen innerhalb anderer SQL-Elemente. </summary>
    private sealed class DoubleQuotedColumnVisitor : TSqlFragmentVisitor
    {
        /// <summary> Mindestens ein Spaltenbezug verwendet doppelte Anführungszeichen. </summary>
        public bool Found { get; private set; }

        public override void ExplicitVisit(ColumnReferenceExpression node)
        {
            Found |= node.MultiPartIdentifier?.Identifiers.Any(x => x.QuoteType == QuoteType.DoubleQuote) == true;
            base.ExplicitVisit(node);
        }
    }

    #endregion
}
