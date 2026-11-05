using FiaeTrainer.Core.Sql.Validation;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace FiaeTrainer.Core.Sql.Exams;

/// <summary> Begrenzt Prüfungsantworten auf die Falltabellen und schreibt sie auf sitzungseigene Arbeitskopien um. </summary>
public static class SqlExamValidator
{
    /// <summary> Prüft genau eine Anweisung einschließlich Schreibregeln und sicherer Tabellenziele. </summary>
    public static string? Prepare(SqlExamCase item, SqlExamTask task, string answer, out string sessionSql)
    {
        sessionSql = "";
        if (string.IsNullOrWhiteSpace(answer) || answer.Length > 12000) return "Eine Antwort mit höchstens 12.000 Zeichen ist erforderlich.";
        TSqlFragment fragment = new TSql160Parser(true).Parse(new StringReader(answer), out IList<ParseError> errors);
        if (errors.Count > 0) return SqlSyntaxFeedback.Explain(answer, errors[0]);
        if (fragment is not TSqlScript script || script.Batches.Count != 1 || script.Batches[0].Statements.Count != 1) return "Verwende genau eine vollständige SQL-Anweisung.";
        bool Allowed(SchemaObjectName name) => name.Identifiers.Count == 1 && item.Tables.Contains(name.BaseIdentifier.Value, StringComparer.OrdinalIgnoreCase);
        TSqlStatement statement = script.Batches[0].Statements[0];
        bool operationAllowed = task.Operation switch
        {
            "Query" => statement is SelectStatement,
            "Update" => statement is UpdateStatement,
            "Insert" => statement is InsertStatement,
            "Alter" => statement is AlterTableAddTableElementStatement,
            _ => false
        };
        if (!operationAllowed) return "Verwende die im Auftrag geforderte Anweisungsart.";
        if (statement is DataModificationStatement modification)
        {
            DataModificationSpecification specification = modification switch { UpdateStatement update => update.UpdateSpecification, InsertStatement insert => insert.InsertSpecification, _ => throw new InvalidDataException() };
            if (specification.Target is not NamedTableReference target || !Allowed(target.SchemaObject) || !target.SchemaObject.BaseIdentifier.Value.Equals(task.Target, StringComparison.OrdinalIgnoreCase) || specification.OutputClause != null || specification.OutputIntoClause != null || specification.TopRowFilter != null || modification.WithCtesAndXmlNamespaces != null || modification.OptimizerHints.Count != 0)
                return "Ändere direkt die gefragte Falltabelle, ohne OUTPUT, TOP oder Verwaltungszusätze.";
            if (statement is InsertStatement insertion && (insertion.InsertSpecification.InsertSource is not ValuesInsertSource values || values.RowValues.Count > 20))
                return "Verwende INSERT mit VALUES für höchstens 20 Zeilen; Prozeduraufrufe und externe Datenquellen sind ausgeschlossen.";
        }
        if (statement is AlterTableAddTableElementStatement alter)
        {
            if (!Allowed(alter.SchemaObjectName) || !alter.SchemaObjectName.BaseIdentifier.Value.Equals(task.Target, StringComparison.OrdinalIgnoreCase) || alter.ExistingRowsCheckEnforcement != default || alter.Definition.ColumnDefinitions.Count != 1 || alter.Definition.TableConstraints.Count != 0 || alter.Definition.Indexes.Count != 0 || alter.Definition.SystemTimePeriod != null)
                return "Ergänze genau eine einfache Spalte der gefragten Tabelle mit ALTER TABLE ADD.";
            ColumnDefinition column = alter.Definition.ColumnDefinitions[0];
            if (column.DataType is not SqlDataTypeReference type || type.SqlDataTypeOption is not (SqlDataTypeOption.Int or SqlDataTypeOption.Bit or SqlDataTypeOption.NVarChar or SqlDataTypeOption.Decimal) || column.ComputedColumnExpression != null || column.IdentityOptions != null || column.DefaultConstraint != null || column.Encryption != null || column.StorageOptions != null || column.Index != null || column.Collation != null || column.Constraints.Any(constraint => constraint is not NullableConstraintDefinition))
                return "Nutze eine einfache Spalte mit int, bit, decimal oder nvarchar ohne weitere Eigenschaften außer NULL/NOT NULL.";
        }
        SqlSafetyVisitor safety = new(Allowed, "Verwende ausschließlich die kurzen Tabellennamen dieses Prüfungsfalls, ohne Schemas, fremde Tabellen oder Tabellenhinweise.");
        script.Accept(safety);
        string? error = safety.Error ?? SqlWritingRules.Validate(script);
        if (error != null) return error;
        AliasVisitor aliases = new();
        script.Accept(aliases);
        script.Accept(new TemporaryTableVisitor(item.Tables, aliases.Names));
        new Sql160ScriptGenerator().GenerateScript(script, out sessionSql);
        return null;
    }

    /// <summary> Verändert nur echte Tabellen- und ausgeschriebene Spaltenbezüge, niemals Texte oder Aliase. </summary>
    private sealed class TemporaryTableVisitor(string[] names, HashSet<string> aliases) : TSqlFragmentVisitor
    {
        public override void ExplicitVisit(NamedTableReference node) { node.SchemaObject.BaseIdentifier.Value = "#" + node.SchemaObject.BaseIdentifier.Value; base.ExplicitVisit(node); }
        public override void ExplicitVisit(AlterTableAddTableElementStatement node) { node.SchemaObjectName.BaseIdentifier.Value = "#" + node.SchemaObjectName.BaseIdentifier.Value; base.ExplicitVisit(node); }
        public override void ExplicitVisit(ColumnReferenceExpression node)
        {
            if (node.MultiPartIdentifier?.Identifiers is { Count: 2 } identifiers && !aliases.Contains(identifiers[0].Value) && names.Contains(identifiers[0].Value, StringComparer.OrdinalIgnoreCase)) identifiers[0].Value = "#" + identifiers[0].Value;
            base.ExplicitVisit(node);
        }
        public override void ExplicitVisit(SelectStarExpression node)
        {
            if (node.Qualifier?.Identifiers is { Count: 1 } identifiers && !aliases.Contains(identifiers[0].Value) && names.Contains(identifiers[0].Value, StringComparer.OrdinalIgnoreCase)) identifiers[0].Value = "#" + identifiers[0].Value;
            base.ExplicitVisit(node);
        }
    }
    /// <summary> Ein Alias darf wie die echte Tabelle heißen und wird dann nicht zum temporären Tabellennamen umgeschrieben. </summary>
    private sealed class AliasVisitor : TSqlFragmentVisitor
    {
        /// <summary> Tatsächliche Aliasnamen müssen bei Spaltenqualifizierungen erhalten bleiben. </summary>
        public HashSet<string> Names { get; } = new(StringComparer.OrdinalIgnoreCase);
        public override void ExplicitVisit(NamedTableReference node) { if (node.Alias != null) Names.Add(node.Alias.Value); base.ExplicitVisit(node); }
    }
}
