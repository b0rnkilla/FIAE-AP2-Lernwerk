using Microsoft.SqlServer.TransactSql.ScriptDom;
using FiaeTrainer.Core.Sql.Validation;

namespace FiaeTrainer.Core.Sql.Workshop;

/// <summary> Prüft Anweisungen für den jeweiligen Fall vor der Ausführung im isolierten Schema. </summary>
internal static class ScenarioScript
{
    public static string? Prepare(WorkshopLesson lesson, string sql, bool grade, out List<TSqlStatement> statements)
    {
        statements = [];
        if (string.IsNullOrWhiteSpace(sql) || sql.Length > 12000)
            return "Schreibe ein Skript mit höchstens 12.000 Zeichen.";
        var script = (TSqlScript)new TSql160Parser(true).Parse(new StringReader(sql), out var errors);
        if (errors.Count > 0)
            return SqlSyntaxFeedback.ExplainExam(sql, errors[0], script.ScriptTokenStream);
        if (script.Batches.Count != 1 || script.Batches[0].Statements.Count is < 1 or > 10)
            return "Verwende ein Skript ohne GO mit höchstens zehn Anweisungen.";
        var objects = WorkshopScenario.Get(lesson.Scenario).Objects;
        statements = [.. script.Batches[0].Statements];
        foreach (var statement in statements)
        {
            bool Allowed(SchemaObjectName name) => name.Identifiers.Count == 1 && objects.Contains(name.BaseIdentifier.Value, StringComparer.OrdinalIgnoreCase);
            bool valid = statement switch
            {
                SelectStatement => true,
                UpdateStatement u => u.UpdateSpecification.Target is NamedTableReference && u.UpdateSpecification.OutputClause == null && u.UpdateSpecification.OutputIntoClause == null,
                DeleteStatement d => d.DeleteSpecification.Target is NamedTableReference && d.DeleteSpecification.OutputClause == null && d.DeleteSpecification.OutputIntoClause == null,
                InsertStatement i => i.InsertSpecification.Target is NamedTableReference && i.InsertSpecification.InsertSource is ValuesInsertSource or SelectInsertSource && i.InsertSpecification.OutputIntoClause == null && i.InsertSpecification.OutputClause == null,
                CreateViewStatement v => lesson.Technique == "View" && Allowed(v.SchemaObjectName) && v.ViewOptions.Count == 0,
                GrantStatement or RevokeStatement => PermissionCommand(statement) != null && lesson.Technique is "Grant" or "Revoke",
                CreateTableStatement or AlterTableAddTableElementStatement or AlterTableAlterColumnStatement or CreateIndexStatement => SimpleDefinition(statement, objects),
                BeginTransactionStatement b => !b.Distributed && !b.MarkDefined && b.Name == null,
                CommitTransactionStatement c => c.Name == null && c.DelayedDurabilityOption == default,
                _ => false
            };
            if (!valid)
                return $"Zeile {statement.StartLine}: Diese Anweisungsform gehört nicht zu den Tabellen und Rechten dieses Falls.";
            NamedTableReference? alias = null;
            if (statement is UpdateStatement update && update.UpdateSpecification.Target is NamedTableReference target && update.UpdateSpecification.FromClause is {} sources)
            {
                var matches = sources.TableReferences.SelectMany(Exams.SqlExamValidator.DirectSources)
                    .Where(source => string.Equals(source.Alias?.Value, target.SchemaObject.BaseIdentifier.Value, StringComparison.OrdinalIgnoreCase)).ToList();
                if (matches.Count == 1 && matches[0] is NamedTableReference source && Allowed(source.SchemaObject) && target.SchemaObject.Identifiers.Count == 1)
                    alias = target;
            }
            SqlSafetyVisitor visitor = new(name => ReferenceEquals(name, alias?.SchemaObject) || Allowed(name), $"Zeile {statement.StartLine}: Nutze die Falltabellen {string.Join(", ", objects)} ohne Schemapräfix.");
            statement.Accept(visitor);
            if (visitor.Error != null)
                return visitor.Error;
            if (statement is DataModificationStatement modification && (modification.WithCtesAndXmlNamespaces != null || modification.OptimizerHints.Count > 0))
                return "Nutze die Falltabellen ohne WITH oder OPTION.";
            statement.Accept(new QualifyObjects(objects, alias?.SchemaObject));
        }
        bool wrapped = statements is [BeginTransactionStatement, .., CommitTransactionStatement]
            && statements.Count(s => s is BeginTransactionStatement or CommitTransactionStatement) == 2;
        if (statements.Any(s => s is BeginTransactionStatement or CommitTransactionStatement) && !wrapped)
            return "Öffne die Transaktion vor den Änderungen und bestätige sie genau einmal am Ende.";
        if (grade && lesson.Technique == "Archive" && !(wrapped && statements.OfType<InsertStatement>().Any() && statements.OfType<DeleteStatement>().Any()))
            return "Kopieren und Löschen gehören in dieselbe bestätigte Transaktion.";
        var operators = new SetOperators();
        script.Accept(operators);
        bool goal = lesson.Technique switch
        {
            "Union" => operators.Found.Contains((BinaryQueryExpressionType.Union, false)),
            "UnionAll" => operators.Found.Contains((BinaryQueryExpressionType.Union, true)),
            "Except" => operators.Found.Contains((BinaryQueryExpressionType.Except, false)),
            "Intersect" => operators.Found.Contains((BinaryQueryExpressionType.Intersect, false)),
            "UpdateJoin" => statements.OfType<UpdateStatement>().Any(u => u.UpdateSpecification.FromClause?.TableReferences.Any(t => t is JoinTableReference or JoinParenthesisTableReference) == true),
            "InsertSelect" => statements.OfType<InsertStatement>().Any(i => i.InsertSpecification.InsertSource is SelectInsertSource),
            _ => true
        };
        if (grade && !goal)
            return "Verwende die im Auftrag genannte Arbeitstechnik: " + lesson.Topic + ".";
        if (grade && lesson.Technique is "Query" or "UnionAll" or "Union" or "Except" or "Intersect" && statements is not [SelectStatement])
            return "Gib die geforderte Liste mit einer einzelnen SELECT-Abfrage aus.";
        return SqlWritingRules.Validate(script);
    }

    /// <summary> Freie Namen für Schlüssel und Indizes, sonst nur die einfachen Strukturbausteine der drei Fälle. </summary>
    private static bool SimpleDefinition(TSqlStatement statement, string[] objects)
    {
        var vocabulary = new HashSet<string>(("CREATE ALTER ADD TABLE COLUMN CONSTRAINT INT NULL NOT REFERENCES FOREIGN PRIMARY KEY UNIQUE ON DELETE UPDATE NO ACTION CASCADE DEFAULT FOR NONCLUSTERED CLUSTERED INDEX ASC DESC WITH CHECK " +
            "Id Name Kunde BestellungId ArtikelId Titel Intern Bestaetigt Freigabe Stufe Menge Bestand Gezaehlt").Split(' '), StringComparer.OrdinalIgnoreCase);
        vocabulary.UnionWith(objects);
        string previous = "";
        foreach (var item in Tokens(statement))
        {
            string token = Decode(item);
            if (!vocabulary.Contains(token) && token is not ("(" or ")" or "," or ";") && !int.TryParse(token, out _) &&
                !(previous is "CONSTRAINT" or "INDEX" && item.TokenType is TSqlTokenType.Identifier or TSqlTokenType.QuotedIdentifier))
                return false;
            previous = token.ToUpperInvariant();
        }
        SchemaObjectName target = statement switch
        {
            CreateTableStatement c => c.SchemaObjectName,
            AlterTableStatement a => a.SchemaObjectName,
            CreateIndexStatement i => i.OnName,
            _ => throw new InvalidOperationException()
        };
        return target.Identifiers.Count == 1 && objects.Contains(target.BaseIdentifier.Value, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary> Rechtebefehle werden ausschließlich aus den zugelassenen Werten neu aufgebaut. </summary>
    internal static string? PermissionCommand(TSqlStatement statement, params string[] objects)
    {
        var words = Tokens(statement).Select(Decode).Where(w => w != ";").Select(w => w.ToUpperInvariant()).ToList();
        int on = words.IndexOf("ON");
        if (on < 2) return null;
        if (words.Skip(on + 1).Take(2).SequenceEqual(["OBJECT", "::"])) words.RemoveRange(on + 1, 2);
        if (words.Count != on + 4 || words[^1] != "EMPFANG") return null;
        string action = words[0], direction = words[^2];
        if ((action, direction) is not (("GRANT", "TO") or ("DENY", "TO") or ("REVOKE", "FROM") or ("REVOKE", "TO"))) return null;
        var permissions = words.GetRange(1, on - 1);
        if (permissions.Count % 2 != 1 || permissions.Where((_, n) => n % 2 == 0).Any(p => p is not ("SELECT" or "UPDATE" or "INSERT" or "DELETE")) ||
            permissions.Where((_, n) => n % 2 == 1).Any(p => p != ",")) return null;
        if (objects.Length == 0) objects = ["Tagesplan", "Termine"];
        string? target = objects.FirstOrDefault(n => n.Equals(words[on + 1], StringComparison.OrdinalIgnoreCase));
        return target == null ? null : $"{action} {string.Join(" ", permissions)} ON LernwerkProbe.[{target}] {direction} LernwerkEmpfangRolle;";
    }

    private static IEnumerable<TSqlParserToken> Tokens(TSqlStatement statement) => statement.ScriptTokenStream
        .Skip(statement.FirstTokenIndex).Take(statement.LastTokenIndex - statement.FirstTokenIndex + 1)
        .Where(t => t.TokenType is not (TSqlTokenType.WhiteSpace or TSqlTokenType.SingleLineComment or TSqlTokenType.MultilineComment));

    private static string Decode(TSqlParserToken token) => token.TokenType == TSqlTokenType.QuotedIdentifier
        ? Identifier.DecodeIdentifier(token.Text, out _) : token.Text;

    /// <summary> Bindet echte Objekte auch bei noch nicht angelegten Tabellen an das Arbeitsschema. Ein UPDATE-Zielalias bleibt ein Alias. </summary>
    internal sealed class QualifyObjects(string[] names, SchemaObjectName? alias) : TSqlFragmentVisitor
    {
        public override void ExplicitVisit(SchemaObjectName node)
        {
            if (node != alias && node.Identifiers.Count == 1 && names.Contains(node.BaseIdentifier.Value,StringComparer.OrdinalIgnoreCase))
                node.Identifiers.Insert(0,new Identifier { Value = "LernwerkProbe" });
            base.ExplicitVisit(node);
        }
    }

    private sealed class SetOperators : TSqlFragmentVisitor
    {
        public HashSet<(BinaryQueryExpressionType, bool)> Found { get; } = [];
        public override void ExplicitVisit(BinaryQueryExpression node)
        {
            Found.Add((node.BinaryQueryExpressionType, node.All));
            base.ExplicitVisit(node);
        }
    }
}
