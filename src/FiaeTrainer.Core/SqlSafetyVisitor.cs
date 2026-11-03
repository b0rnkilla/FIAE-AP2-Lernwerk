using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace FiaeTrainer.Core;

/// <summary> Gemeinsame Grenzen für erlaubte Tabellennamen, skalare Funktionen und externe Datenquellen. </summary>
internal sealed class SqlSafetyVisitor(Func<SchemaObjectName, bool> tableAllowed, string tableHint) : TSqlFragmentVisitor
{
    /// <summary> Erste nicht erlaubte Konstruktion. </summary>
    public string? Error { get; private set; }

    public override void ExplicitVisit(NamedTableReference node)
    {
        if (!tableAllowed(node.SchemaObject) || node.TableHints.Count > 0 || node.TemporalClause != null)
            Error ??= tableHint;
        base.ExplicitVisit(node);
    }

    public override void ExplicitVisit(FunctionCall node)
    {
        string[] allowed = ["COUNT", "SUM", "AVG", "MIN", "MAX", "ROUND", "ABS", "LEN", "LOWER", "UPPER", "ISNULL", "YEAR", "MONTH", "DAY", "DATEDIFF", "DATEADD"];
        if (node.CallTarget != null || !allowed.Contains(node.FunctionName.Value, StringComparer.OrdinalIgnoreCase))
            Error ??= "Diese Funktion gehört noch nicht zum unterstützten Lernumfang. Nutze die in der Aufgabe erklärten SQL-Funktionen.";
        base.ExplicitVisit(node);
    }

    public override void ExplicitVisit(VariableReference node) => Error ??= "Variablen sind in diesen Aufgaben nicht erforderlich.";
    public override void ExplicitVisit(GlobalVariableExpression node) => Error ??= "Servervariablen sind in diesen Aufgaben nicht erforderlich.";
    public override void ExplicitVisit(SelectSetVariable node) => Error ??= "Variablenzuweisungen sind hier nicht erlaubt.";
    public override void ExplicitVisit(OpenRowsetTableReference node) => RejectExternal();
    public override void ExplicitVisit(OpenQueryTableReference node) => RejectExternal();
    public override void ExplicitVisit(AdHocTableReference node) => RejectExternal();
    public override void ExplicitVisit(BulkOpenRowset node) => RejectExternal();
    public override void ExplicitVisit(SchemaObjectFunctionTableReference node) => RejectExternal();
    public override void ExplicitVisit(OpenXmlTableReference node) => RejectExternal();
    public override void ExplicitVisit(OpenJsonTableReference node) => RejectExternal();
    public override void ExplicitVisit(DataModificationTableReference node) => RejectExternal();
    public override void ExplicitVisit(FullTextTableReference node) => RejectExternal();
    public override void ExplicitVisit(SemanticTableReference node) => RejectExternal();

    public override void ExplicitVisit(SelectStatement node)
    {
        if (node.Into != null || node.WithCtesAndXmlNamespaces != null || node.OptimizerHints.Count > 0 || node.QueryExpression.ForClause != null)
            Error ??= "Verwende SELECT ohne INTO, WITH, OPTION oder FOR. Unterabfragen sind erlaubt.";
        base.ExplicitVisit(node);
    }

    private void RejectExternal() => Error ??= "Verwende ausschließlich die angezeigten Übungstabellen. Externe Quellen und Tabellenfunktionen sind hier nicht erlaubt.";
}
