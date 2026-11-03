using System.Text.RegularExpressions;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace FiaeTrainer.Core;

/// <summary> Erklärt Syntaxfehler anhand der betroffenen SQL-Stelle, ohne Lösungen zu verändern oder zu erraten. </summary>
internal static class SqlSyntaxFeedback
{
    /// <summary> Ergänzt Parserfehler um konkrete Hinweise zu offenen Texten, Bezeichnern, Kommentaren und Klammern. </summary>
    public static string Explain(string sql, ParseError? error)
    {
        string? delimiterIssue = FindDelimiterIssue(sql);
        if (delimiterIssue != null)
            return delimiterIssue;

        int offset = Math.Clamp(error?.Offset ?? sql.Length, 0, sql.Length);
        string context = Excerpt(sql, Math.Max(0, offset - 20));
        if (offset >= sql.TrimEnd().Length)
            return $"Deine SQL-Anweisung endet unerwartet bei „{context}“. Prüfe, ob nach einem Operator noch ein Wert, nach FROM ein Tabellenname oder nach einem Komma eine weitere Spalte fehlt.";
        return $"In der Nähe von „{context}“ passt die SQL-Schreibweise noch nicht. Prüfe dort fehlende Spaltennamen, überflüssige Kommas oder einen unvollständigen Vergleich. Die übliche Reihenfolge ist SELECT – FROM – WHERE – GROUP BY – HAVING – ORDER BY.";
    }

    /// <summary> Prüft ausschließlich Begrenzungszeichen; Kommentare und korrekt maskierte Zeichen werden übersprungen. </summary>
    private static string? FindDelimiterIssue(string sql)
    {
        char delimiter = '\0';
        int opening = 0;
        int commentStart = 0;
        int commentDepth = 0;
        int unexpectedClosing = -1;
        bool lineComment = false;
        Stack<int> parentheses = new();
        for (int index = 0; index < sql.Length; index++)
        {
            char current = sql[index];
            char next = index + 1 < sql.Length ? sql[index + 1] : '\0';
            if (lineComment)
            {
                if (current is '\r' or '\n')
                    lineComment = false;
                continue;
            }
            if (commentDepth > 0)
            {
                if (current == '/' && next == '*') { commentDepth++; index++; }
                else if (current == '*' && next == '/') { commentDepth--; index++; }
                continue;
            }
            if (delimiter != '\0')
            {
                char closing = delimiter == '[' ? ']' : delimiter;
                if (current == closing)
                {
                    if (next == closing)
                        index++;
                    else
                        delimiter = '\0';
                }
                continue;
            }
            if (current == '-' && next == '-') { lineComment = true; index++; }
            else if (current == '/' && next == '*') { commentStart = index; commentDepth = 1; index++; }
            else if (current is '\'' or '"' or '[') { delimiter = current; opening = index; }
            else if (current == '(')
                parentheses.Push(index);
            else if (current == ')')
            {
                if (parentheses.Count > 0)
                    parentheses.Pop();
                else if (unexpectedClosing < 0)
                    unexpectedClosing = index;
            }
        }

        string context = Excerpt(sql, opening);
        if (delimiter == '\'')
            return $"Der Text ab „{context}“ ist nicht richtig abgeschlossen. Textwerte müssen mit einem einfachen Anführungszeichen (') beginnen und mit demselben Zeichen enden, zum Beispiel 'Essen'. Ein doppeltes Anführungszeichen (\") schließt diesen Text nicht ab.";
        if (delimiter == '"')
            return $"Bei „{context}“ fehlt das passende schließende doppelte Anführungszeichen (\"). Falls du einen Textwert meinst, verwende auf beiden Seiten einfache Anführungszeichen, zum Beispiel 'Essen'. Doppelte Anführungszeichen kennzeichnen Spalten- und Tabellennamen.";
        if (delimiter == '[')
            return $"Der Name ab „{context}“ ist nicht abgeschlossen. Ergänze die schließende eckige Klammer ], zum Beispiel [Zielort].";
        if (commentDepth > 0)
            return $"Der Kommentar ab „{Excerpt(sql, commentStart)}“ ist nicht abgeschlossen. Ein Kommentar mit /* am Anfang benötigt */ am Ende.";
        if (unexpectedClosing >= 0)
            return $"Bei „{Excerpt(sql, Math.Max(0, unexpectedClosing - 20))}“ steht eine schließende Klammer ) ohne passende öffnende Klammer. Prüfe das Klammerpaar.";
        if (parentheses.Count > 0)
            return $"Zur öffnenden Klammer bei „{Excerpt(sql, parentheses.Peek())}“ fehlt die schließende Klammer ). Schließe das Klammerpaar, bevor du die Anweisung beendest.";
        return null;
    }

    /// <summary> Zeigt einen kurzen Ausschnitt des eingegebenen SQL statt einer schwer auffindbaren Spaltennummer. </summary>
    private static string Excerpt(string sql, int start)
    {
        string text = Regex.Replace(sql[start..], @"\s+", " ").Trim();
        return text.Length > 64 ? text[..64] + "…" : text;
    }
}
