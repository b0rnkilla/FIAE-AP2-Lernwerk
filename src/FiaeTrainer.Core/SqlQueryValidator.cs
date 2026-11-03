using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace FiaeTrainer.Core;

/// <summary> Prüft echte T-SQL-Syntax und begrenzt Lernabfragen auf lesende Zugriffe auf das Fallschema. </summary>
public sealed class SqlQueryValidator
{
    /// <summary> Bekannte Tabellen der Ermittlung; unqualifizierte Namen bleiben auf diesen Bestand beschränkt. </summary>
    public static IReadOnlyList<string> TableNames { get; } = ["Sendungen", "Mitarbeiter", "Standorte", "Fahrzeuge", "Zutritte", "Fahrten"];

    /// <summary> Prüft genau eine SELECT-Anweisung und liefert bei Bedarf einen deutschen Hinweis. </summary>
    public string? Validate(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return "Schreibe zuerst eine SQL-Abfrage. Du kannst mit SELECT beginnen.";
        if (sql.Length > 12000)
            return "Die Abfrage ist zu lang. Für diese Übungen reichen wenige Zeilen.";

        TSqlFragment fragment = Parse(sql, out IList<ParseError> errors);
        if (errors.Count > 0)
            return SqlSyntaxFeedback.Explain(sql, errors[0]);
        if (fragment is not TSqlScript script || script.Batches.Count != 1 || script.Batches[0].Statements.Count != 1 || script.Batches[0].Statements[0] is not SelectStatement select)
            return "In der Ermittlungsakte ist genau eine SELECT-Abfrage erlaubt. Datenänderungen und Verwaltungsbefehle gehören nicht in diesen Fall.";
        if (select.Into != null || select.WithCtesAndXmlNamespaces != null || select.OptimizerHints.Count > 0 || select.QueryExpression.ForClause != null)
            return "Verwende eine einfache SELECT-Abfrage ohne INTO, WITH, OPTION oder FOR. Unterabfragen sind erlaubt.";

        SqlSafetyVisitor visitor = new(IsCaseTable, "Verwende eine der angezeigten Tabellen, zum Beispiel Sendungen. Andere Schemas und Tabellenhinweise sind hier nicht erlaubt.");
        fragment.Accept(visitor);
        return visitor.Error ?? SqlWritingRules.Validate(script);
    }

    /// <summary> Löst optionale Schemanamen eindeutig im Fallschema auf, ohne Literale oder Aliase zu verändern. </summary>
    public string ForCaseData(string sql) => WithSchema(sql, "fall");

    /// <summary> Leitet qualifizierte und unqualifizierte Tabellenbezüge auf den zusätzlichen Prüfdatenbestand um. </summary>
    public string ForVerificationData(string sql) => WithSchema(sql, "probe");

    private static string WithSchema(string sql, string schema)
    {
        TSqlFragment fragment = Parse(sql, out _);
        fragment.Accept(new SchemaVisitor(schema));
        new Sql160ScriptGenerator().GenerateScript(fragment, out string generated);
        return generated;
    }

    private static TSqlFragment Parse(string sql, out IList<ParseError> errors) => new TSql160Parser(true).Parse(new StringReader(sql), out errors);

    private static bool IsCaseTable(SchemaObjectName name) => name.Identifiers.Count is 1 or 2 && (name.Identifiers.Count == 1 || string.Equals(name.SchemaIdentifier?.Value, "fall", StringComparison.OrdinalIgnoreCase)) && TableNames.Contains(name.BaseIdentifier.Value, StringComparer.OrdinalIgnoreCase);

    /// <summary> Leitet dieselbe Abfragelogik auf zusätzliche Prüfdaten um. </summary>
    private sealed class SchemaVisitor(string schema) : TSqlFragmentVisitor
    {
        public override void ExplicitVisit(NamedTableReference node)
        {
            if (node.SchemaObject.Identifiers.Count == 1)
                node.SchemaObject.Identifiers.Insert(0, new Identifier { Value = schema });
            else
                node.SchemaObject.SchemaIdentifier.Value = schema;
            base.ExplicitVisit(node);
        }

        public override void ExplicitVisit(ColumnReferenceExpression node)
        {
            RewriteQualifier(node.MultiPartIdentifier?.Identifiers, 3);
            base.ExplicitVisit(node);
        }

        public override void ExplicitVisit(SelectStarExpression node)
        {
            RewriteQualifier(node.Qualifier?.Identifiers, 2);
            base.ExplicitVisit(node);
        }

        private void RewriteQualifier(IList<Identifier>? identifiers, int expectedCount)
        {
            if (identifiers?.Count == expectedCount && string.Equals(identifiers[0].Value, "fall", StringComparison.OrdinalIgnoreCase))
                identifiers[0].Value = schema;
        }
    }
}
