using System.Text.RegularExpressions;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace FiaeTrainer.Core.Sql.Validation;

/// <summary> Erklärt Syntaxfehler anhand der betroffenen SQL-Stelle, ohne Lösungen zu verändern oder zu erraten. </summary>
internal static class SqlSyntaxFeedback
{
    /// <summary> Zeigt das tatsächlich beanstandete Token und die Position im abgegebenen Prüfungsentwurf. </summary>
    public static string ExplainExam(string sql, ParseError error, IList<TSqlParserToken> tokens)
    {
        var words = tokens.Where(t => t.TokenType is not (TSqlTokenType.WhiteSpace or TSqlTokenType.SingleLineComment or TSqlTokenType.MultilineComment)).ToList();
        int index = words.FindIndex(t => t.Offset >= error.Offset);
        string Word(int delta) => index >= 0 && index + delta >= 0 && index + delta < words.Count ? words[index + delta].Text ?? "" : "";
        string word = Word(0);
        string location = $"Zeile {error.Line}, Spalte {error.Column}";
        if (word == "-" && Word(-2).Equals("AS", StringComparison.OrdinalIgnoreCase))
            return $"{location}: Der Bindestrich im Alias ist hier ungeschützt. Setze den ganzen Anzeigenamen in eckige Klammern oder verwende einen Namen ohne Bindestrich.";
        static bool IsText(string value) => value.StartsWith('\'') || value.StartsWith("N'", StringComparison.OrdinalIgnoreCase);
        if ((word.Equals("NOT", StringComparison.OrdinalIgnoreCase) && Word(-1).Equals("IS", StringComparison.OrdinalIgnoreCase) && IsText(Word(1))) ||
            (IsText(word) && (Word(-1).Equals("IS", StringComparison.OrdinalIgnoreCase) ||
                Word(-1).Equals("NOT", StringComparison.OrdinalIgnoreCase) && Word(-2).Equals("IS", StringComparison.OrdinalIgnoreCase))))
            return $"{location}: Vergleiche Text mit = oder <>. Mit IS NULL und IS NOT NULL prüfst du, ob ein Wert fehlt oder vorhanden ist.";
        if (index == 2 && Word(-2).Equals("UPDATE", StringComparison.OrdinalIgnoreCase) && (Word(1).Equals("SET", StringComparison.OrdinalIgnoreCase) || word.Equals("AS", StringComparison.OrdinalIgnoreCase)))
            return $"{location}: Nach der UPDATE-Zieltabelle folgt SET. Für einen JOIN gib den Zielalias hinter UPDATE an und definiere ihn später in FROM Tabelle AS Alias. JOIN … ON steht vor WHERE.";
        if (word.Equals("JOIN", StringComparison.OrdinalIgnoreCase) && FollowsFilter(words, index))
            return $"{location}: Diese Verbindung steht hinter dem Filter. Ordne JOIN … ON dem FROM-Teil zu und schreibe WHERE hinter die Tabellenverbindungen.";
        if (FindDelimiterIssue(sql) is string delimiter)
            return $"{location} (Parserposition): {delimiter}";
        if (word.Equals("FROM", StringComparison.OrdinalIgnoreCase) && Word(-1) == ",")
            return $"{location}: Zwischen dem Komma und FROM fehlt ein weiterer Ausgabeausdruck. Ist die Spaltenliste vollständig, entferne das letzte Komma.";
        if (string.IsNullOrEmpty(word) || error.Offset >= sql.TrimEnd().Length)
            return $"{location}: Die Anweisung endet hier unvollständig. Prüfe den letzten Ausdruck vor dem Ende.";
        return $"{location}: Bei „{word}“ kann SQL Server die Anweisung nicht weiterlesen. Prüfe dieses Zeichen oder Wort und den Ausdruck unmittelbar davor.";
    }

    /// <summary> Eine WHERE-Bedingung innerhalb einer abgeschlossenen Unterabfrage bestimmt nicht die äußere JOIN-Position. </summary>
    private static bool FollowsFilter(List<TSqlParserToken> words, int index)
    {
        int depth = 0;
        for (int i = index - 1; i >= 0; i--)
        {
            if (words[i].Text == ")") depth++;
            else if (words[i].Text == "(") { if (depth == 0) break; depth--; }
            else if (depth == 0 && words[i].TokenType == TSqlTokenType.Where) return true;
            else if (depth == 0 && words[i].TokenType is TSqlTokenType.From or TSqlTokenType.Select or TSqlTokenType.Semicolon) break;
        }
        return false;
    }

    /// <summary> Nennt nur Bezeichner aus der sichtbaren Abgabe. Die Zeile des umgeschriebenen Server-SQL wäre irreführend. </summary>
    public static string ExplainBinding(string sql, int number, string serverMessage, string fallback)
    {
        if (number is not (207 or 209 or 4104 or 8120)) return fallback;
        var tokens = new TSql160Parser(true).GetTokenStream(new StringReader(sql), out _);
        foreach (Match quoted in Regex.Matches(serverMessage, """['"„]([^'"“”]+)['"“”]"""))
        {
            string name = quoted.Groups[1].Value.Split('.').Last();
            var matches = tokens.Where(t => t.TokenType is TSqlTokenType.Identifier or TSqlTokenType.QuotedIdentifier
                && string.Equals(Identifier.DecodeIdentifier(t.Text, out _), name, StringComparison.OrdinalIgnoreCase)).ToList();
            if (matches.Count == 0) continue;
            var token = matches[0];
            string where = matches.Count == 1 ? $"Zeile {token.Line}, Spalte {token.Column}, bei „{token.Text}“" : $"Beim Bezeichner „{token.Text}“";
            return $"{where}: {fallback}";
        }
        return fallback;
    }

    /// <summary> Ergänzt Parserfehler um konkrete Hinweise zu offenen Texten, Bezeichnern, Kommentaren und Klammern. </summary>
    public static string Explain(string sql, ParseError? error, IList<TSqlParserToken>? workshopTokens = null)
    {
        string? delimiterIssue = FindDelimiterIssue(sql);
        if (delimiterIssue != null)
            return delimiterIssue;

        int offset = Math.Clamp(error?.Offset ?? sql.Length, 0, sql.Length);
        string context = Excerpt(sql, Math.Max(0, offset - 20));
        if (workshopTokens != null && WorkshopHint(workshopTokens, offset) is string hint)
            return $"Prüfe die Anweisung bei „{context}“. {hint}";
        if (offset >= sql.TrimEnd().Length)
            return $"Deine SQL-Anweisung endet unerwartet bei „{context}“. Prüfe, ob nach einem Operator noch ein Wert, nach FROM ein Tabellenname oder nach einem Komma eine weitere Spalte fehlt.";
        return $"In der Nähe von „{context}“ passt die SQL-Schreibweise noch nicht. Prüfe dort fehlende Spaltennamen, überflüssige Kommas oder einen unvollständigen Vergleich. Die übliche Reihenfolge ist SELECT – FROM – WHERE – GROUP BY – HAVING – ORDER BY.";
    }

    /// <summary> Ordnet den Parserfehler der Anweisung im Werkstattskript zu. Semikolons in Texten oder Kommentaren trennen keine Anweisungen. </summary>
    private static string? WorkshopHint(IList<TSqlParserToken> tokens, int offset)
    {
        TSqlTokenType? command = null;
        for (int i = 0; i < tokens.Count && tokens[i].Offset <= offset; i++)
        {
            TSqlParserToken token = tokens[i];
            if (token.TokenType is TSqlTokenType.WhiteSpace or TSqlTokenType.SingleLineComment or TSqlTokenType.MultilineComment or TSqlTokenType.EndOfFile)
                continue;
            if (token.TokenType == TSqlTokenType.Semicolon)
            {
                if (token.Offset < offset)
                    command = null;
                continue;
            }
            command ??= token.TokenType;
            if (command == TSqlTokenType.Update && token.TokenType == TSqlTokenType.Comma && i > 0 && i + 1 < tokens.Count
                && tokens[i - 1].TokenType == TSqlTokenType.Integer && tokens[i + 1].TokenType == TSqlTokenType.Integer
                && offset <= tokens[i + 1].Offset)
                return $"Meinst du mit „{tokens[i - 1].Text},{tokens[i + 1].Text}“ eine Dezimalzahl? In T-SQL schreibst du sie mit Punkt, hier also „{tokens[i - 1].Text}.{tokens[i + 1].Text}“.";
        }
        return command switch
        {
            TSqlTokenType.Update => "Nach UPDATE steht die Tabelle. Mit SET weist du einer Spalte einen Wert zu. Prüfe auch die Bedingung hinter WHERE, wenn du bestimmte Zeilen ändern möchtest.",
            TSqlTokenType.Insert => "Prüfe die Spaltenliste nach INSERT INTO und die Werteliste nach VALUES. Beide Listen stehen in Klammern. Zu jeder Spalte gehört ein Wert an derselben Position.",
            TSqlTokenType.Delete => "Nach DELETE FROM steht die Tabelle. Wenn du bestimmte Zeilen löschen möchtest, gib hinter WHERE die passende Bedingung an.",
            TSqlTokenType.Create => "Nach CREATE TABLE und dem Tabellennamen stehen die Spaltendefinitionen in Klammern. Gib für jede Spalte Namen und Datentyp an und trenne die Definitionen durch Kommas.",
            TSqlTokenType.Alter => "Prüfe den Tabellennamen nach ALTER TABLE und die Ergänzung nach ADD. Eine neue Spalte braucht einen Namen und einen Datentyp.",
            TSqlTokenType.Begin or TSqlTokenType.Commit or TSqlTokenType.Rollback => "Öffne die Transaktion mit BEGIN TRANSACTION. Nach den Änderungen schließt du sie mit COMMIT oder ROLLBACK ab.",
            TSqlTokenType.Select => null,
            _ => "Prüfe den SQL-Befehl und seine Argumente. Schließe jede vollständige Anweisung im Skript mit einem Semikolon ab."
        };
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
