using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace FiaeTrainer.Core;

/// <summary> Prüft die vereinbarten Schreibregeln für Lernabfragen anhand der geparsten SQL-Tokens. </summary>
internal static class SqlWritingRules
{
    /// <summary> Verlangt große Schlüsselwörter und Funktionen sowie ein Semikolon je äußerer Anweisung. </summary>
    public static string? Validate(TSqlScript script)
    {
        foreach (TSqlParserToken token in script.ScriptTokenStream)
            if (token.IsKeyword() && token.Text != token.Text.ToUpperInvariant())
                return $"Schreibe SQL-Schlüsselwörter in diesem Training groß: „{token.Text}“ wird zu „{token.Text.ToUpperInvariant()}“. Tabellen, Spalten, Aliase und Textwerte bleiben unverändert.";

        FunctionCaseVisitor functions = new();
        script.Accept(functions);
        if (functions.Error != null)
            return functions.Error;

        int number = 0;
        foreach (TSqlStatement statement in script.Batches.SelectMany(x => x.Statements))
        {
            number++;
            int last = statement.LastTokenIndex;
            while (last >= statement.FirstTokenIndex && script.ScriptTokenStream[last].TokenType is TSqlTokenType.WhiteSpace or TSqlTokenType.SingleLineComment or TSqlTokenType.MultilineComment)
                last--;
            if (last < statement.FirstTokenIndex || script.ScriptTokenStream[last].TokenType != TSqlTokenType.Semicolon)
            {
                string command = script.ScriptTokenStream[statement.FirstTokenIndex].Text.ToUpperInvariant();
                return $"Beende Anweisung {number} ({command}) mit einem Semikolon (;). Es gehört hinter die vollständige Anweisung, vor einen abschließenden Kommentar. Eine Unterabfrage in Klammern bekommt kein eigenes Semikolon.";
            }
        }
        return null;
    }

    /// <summary> Unterscheidet Funktionsnamen von gleichlautenden Spalten und Aliasnamen. </summary>
    private sealed class FunctionCaseVisitor : TSqlFragmentVisitor
    {
        /// <summary> Erste Funktion mit abweichender Großschreibung. </summary>
        public string? Error { get; private set; }

        public override void ExplicitVisit(FunctionCall node)
        {
            string name = node.FunctionName.Value;
            if (node.FunctionName.QuoteType == QuoteType.NotQuoted && name != name.ToUpperInvariant())
                Error ??= $"Schreibe SQL-Funktionen in diesem Training groß: „{name}“ wird zu „{name.ToUpperInvariant()}“. Spaltennamen und Aliase dürfen ihre Schreibweise behalten.";
            base.ExplicitVisit(node);
        }

        public override void ExplicitVisit(CastCall node) { CheckCall(node); base.ExplicitVisit(node); }
        public override void ExplicitVisit(TryCastCall node) { CheckCall(node); base.ExplicitVisit(node); }
        public override void ExplicitVisit(IIfCall node) { CheckCall(node); base.ExplicitVisit(node); }

        /// <summary> Diese eingebauten Funktionen sind eigene Syntaxknoten statt gewöhnlicher FunctionCall-Knoten. </summary>
        private void CheckCall(TSqlFragment node)
        {
            string name = node.ScriptTokenStream[node.FirstTokenIndex].Text;
            if (name != name.ToUpperInvariant())
                Error ??= $"Schreibe SQL-Funktionen in diesem Training groß: „{name}“ wird zu „{name.ToUpperInvariant()}“. Spaltennamen und Aliase dürfen ihre Schreibweise behalten.";
        }
    }
}
