using FiaeTrainer.Core.Sql.Validation;
using FiaeTrainer.Core.Sql.Workshop;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace FiaeTrainer.Core.Sql.Exams;

/// <summary> Begrenzt die mehrteiligen Prüfungsaufträge auf ihre eigenen Ziele im Arbeitsschema. </summary>
internal static class SqlExamWorkScript
{
    internal static string? Prepare(SqlExamCase item, SqlExamTask task, TSqlScript script, out string sql, out bool atomic)
    {
        sql = "";
        atomic = false;
        if (script.Batches.Count != 1 || script.Batches[0].Statements.Count is < 1 or > 10)
            return "Verwende ein zusammenhängendes Skript mit höchstens zehn Anweisungen.";
        var statements = script.Batches[0].Statements;
        string? error = SqlWritingRules.Validate(script);
        if (error != null) return error;

        if (task.Operation == "Access")
        {
            List<string> permissions = [];
            foreach (var statement in statements)
            {
                string? command = ScenarioScript.PermissionCommand(statement, "Ausgabeliste", "Ausgaben");
                if (statement is not (GrantStatement or RevokeStatement or DenyStatement) || command == null)
                    return $"Zeile {statement.StartLine}: Vergib oder entziehe Rechte nur für Empfang an Ausgabeliste oder Ausgaben.";
                permissions.Add(command);
            }
            sql = string.Join("\n", permissions);
            return null;
        }

        bool wrapped = statements is [BeginTransactionStatement { Name: null, Distributed: false, MarkDefined: false }, .., CommitTransactionStatement { Name: null, DelayedDurabilityOption: var durability }] && durability == default
            && statements.Count(s => s is BeginTransactionStatement or CommitTransactionStatement) == 2;
        if (statements.Any(s => s is BeginTransactionStatement or CommitTransactionStatement) && !wrapped)
            return "Öffne die Transaktion vor den Änderungen und bestätige sie genau einmal am Ende.";

        foreach (var statement in statements)
        {
            SchemaObjectName? alias = null;
            if (task.Operation == "ForeignKey")
            {
                if (statements.Count != 1 || statement is not AlterTableAddTableElementStatement alter ||
                    !IsObject(alter.SchemaObjectName, "Rueckgaben") || alter.Definition.ColumnDefinitions.Count != 0 ||
                    alter.Definition.Indexes.Count != 0 || alter.Definition.SystemTimePeriod != null ||
                    alter.Definition.TableConstraints is not [ForeignKeyConstraintDefinition])
                    return "Ergänze an Rueckgaben genau eine Fremdschlüsselregel mit ALTER TABLE.";
            }
            else if (statement is not (BeginTransactionStatement or CommitTransactionStatement))
            {
                bool valid = statement switch
                {
                    InsertStatement { InsertSpecification: var insert } => insert.Target is NamedTableReference target &&
                        IsObject(target.SchemaObject, "AusgabeArchiv") && insert.InsertSource is SelectInsertSource or ValuesInsertSource &&
                        insert.OutputClause == null && insert.OutputIntoClause == null && insert.TopRowFilter == null,
                    DeleteStatement { DeleteSpecification: var delete } => IsArchiveDelete(delete, out alias),
                    _ => false
                };
                if (!valid || statement is DataModificationStatement dml && (dml.WithCtesAndXmlNamespaces != null || dml.OptimizerHints.Count != 0))
                    return $"Zeile {statement.StartLine}: In diesem Transfer kannst du in AusgabeArchiv einfügen und aus Ausgaben löschen.";
            }
            bool Allowed(SchemaObjectName name) => name == alias ||
                name.Identifiers.Count == 1 && item.Tables.Contains(name.BaseIdentifier.Value, StringComparer.OrdinalIgnoreCase);
            var safety = new SqlSafetyVisitor(Allowed, $"Zeile {statement.StartLine}: Nutze die kurzen Objektnamen aus dem Fallschema.");
            statement.Accept(safety);
            if (safety.Error != null) return safety.Error;
            statement.Accept(new ScenarioScript.QualifyObjects(item.Tables, alias));
        }

        bool movesInOneStatement = statements.OfType<DeleteStatement>().Any(d => d.DeleteSpecification.OutputIntoClause != null);
        atomic = wrapped && (movesInOneStatement || statements.Any(s => s is InsertStatement) && statements.Any(s => s is DeleteStatement))
            || statements.Count == 1 && movesInOneStatement;
        new Sql160ScriptGenerator().GenerateScript(script, out sql);
        return null;
    }

    private static bool IsArchiveDelete(DeleteSpecification delete, out SchemaObjectName? alias)
    {
        alias = null;
        if (delete.Target is not NamedTableReference target || target.SchemaObject.Identifiers.Count != 1 ||
            delete.OutputClause != null || delete.TopRowFilter != null) return false;
        if (delete.OutputIntoClause != null &&
            (delete.OutputIntoClause.IntoTable is not NamedTableReference dest || !IsObject(dest.SchemaObject, "AusgabeArchiv"))) return false;
        var sources = delete.FromClause?.TableReferences.SelectMany(SqlExamValidator.DirectSources)
            .Where(s => string.Equals(s.Alias?.Value, target.SchemaObject.BaseIdentifier.Value, StringComparison.OrdinalIgnoreCase)).ToList();
        if (sources is not { Count: > 0 }) return IsObject(target.SchemaObject, "Ausgaben");
        if (sources is not [NamedTableReference source] || !IsObject(source.SchemaObject, "Ausgaben")) return false;
        alias = target.SchemaObject;
        return true;
    }

    private static bool IsObject(SchemaObjectName name, string expected) =>
        name.Identifiers.Count == 1 && name.BaseIdentifier.Value.Equals(expected, StringComparison.OrdinalIgnoreCase);
}
