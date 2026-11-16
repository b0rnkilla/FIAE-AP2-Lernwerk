using FiaeTrainer.Core.Sql.Exams;
using FiaeTrainer.Core.Sql.Results;
using FiaeTrainer.Core.Sql.Validation;
using Microsoft.Data.SqlClient;

namespace FiaeTrainer.Core.Sql.Database;

public sealed partial class TrainingDatabase
{
    /// <summary> Jeder Datenstand und jede Referenz erhält eine eigene rücksetzbare Arbeitskopie. </summary>
    private async Task<SqlExamRun> ExecuteWorkSchemaExamAsync(SqlExamCase item, SqlExamTask task, string answer, string sql, bool atomic, bool verification)
    {
        var builder = GetConnectionBuilder();
        builder.Pooling = false;
        await using SqlConnection connection = new(builder.ConnectionString);
        using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        var token = deadline.Token;
        Task ExecuteAsync(string command) => ExecuteWorkCommandAsync(connection, command, token);
        bool evaluatingAnswer = false;
        try
        {
            await OpenWorkSchemaAsync(connection, token);
            await ExecuteAsync(item.SeedSql);
            if (verification) await ExecuteAsync(item.VerificationSql);
            foreach (string preparation in task.Preparation) await ExecuteAsync(preparation);
            if (task.Operation == "Archive") await ExecuteAsync("SET XACT_ABORT ON;");
            SqlExamRun result = new();
            evaluatingAnswer = true;
            if (task.Operation == "Query")
            {
                result.Query = await ReadWorkshopTableAsync(connection, sql, token);
                if (result.Query.Truncated) result.Error = "Das Ergebnis überschreitet 200 Zeilen.";
                return result;
            }

            await ExecuteAsync(sql);
            evaluatingAnswer = false;
            if (task.Operation == "Access")
                await ExecuteAsync("EXECUTE AS USER='LernwerkRezeption';");
            foreach (var criterion in task.Criteria)
            {
                if (criterion.AtomicTransfer)
                    result.State[criterion.Name] = new() { Columns = ["Atomarer Transfer"], Rows = [[atomic]] };
                else if (criterion.Probe)
                    result.State[criterion.Name] = await ReadExamProbeAsync(connection, criterion.Sql, token);
                else
                    result.State[criterion.Name] = await ReadWorkshopTableAsync(connection, criterion.Sql, token);
            }
            return result;
        }
        catch (SqlException e)
        {
            bool technical = !evaluatingAnswer || e.Class >= 20 || e.Number is -2 or -1 or 26 or 53 or 1205 or 1222 or 4060 or 18456 or 10060 or 10061;
            return new() { TechnicalFailure = technical, Error = technical ? $"Die Prüfungsarbeitskopie ist nicht verfügbar (SQL {e.Number})."
                : SqlSyntaxFeedback.ExplainBinding(answer, e.Number, e.Message, ExplainError(e.Number, answer)) };
        }
        catch (OperationCanceledException)
        {
            return new() { TechnicalFailure = true, Error = "Die Prüfungsarbeitskopie hat nicht rechtzeitig geantwortet." };
        }
    }

    /// <summary> Verhaltensproben unterscheiden einen abgewiesenen Versuch von einer erfolgreichen Änderung. </summary>
    private static async Task<QueryResult> ReadExamProbeAsync(SqlConnection connection, string sql, CancellationToken token)
    {
        await ExecuteWorkCommandAsync(connection, "SAVE TRANSACTION Pruefschritt;", token);
        try
        {
            return await ReadWorkshopTableAsync(connection, sql, token);
        }
        catch (SqlException e) when (e.Number is 547 or 229)
        {
            return new() { Columns = ["Abgewiesen"], Rows = [[e.Number]] };
        }
        finally
        {
            await ExecuteWorkCommandAsync(connection, "ROLLBACK TRANSACTION Pruefschritt;", token);
        }
    }
}
