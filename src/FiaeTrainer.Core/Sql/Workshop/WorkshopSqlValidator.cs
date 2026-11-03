using Microsoft.SqlServer.TransactSql.ScriptDom;
using FiaeTrainer.Core.Sql.Validation;

namespace FiaeTrainer.Core.Sql.Workshop;

/// <summary> Prüft die erlaubte Werkstattsprache und übersetzt Tabellenbezüge für die Ausführung auf sitzungseigenen Arbeitskopien. </summary>
public sealed class WorkshopSqlValidator
{
    /// <summary> Einzige in der Werkstatt zulässige logische Tabellen. </summary>
    public static IReadOnlyList<string> TableNames { get; } = ["Artikel", "Konten", "Rueckgaben"];

    /// <summary> Prüft Sprache und Lernziel; eine leere Eingabe ist kein erfolgreicher ROLLBACK. </summary>
    public string? Validate(string sql, WorkshopOperation? operation = null)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return "Schreibe zuerst dein SQL-Skript. Jede Ausführung startet mit den unveränderten Ausgangsdaten.";
        if (sql.Length > 12000)
            return "Das Skript ist zu lang. Für die Werkstatt reichen wenige SQL-Anweisungen.";
        TSqlScript? script = Parse(sql, out IList<ParseError> errors) as TSqlScript;
        if (errors.Count > 0 || script == null)
            return SqlSyntaxFeedback.Explain(sql, errors.FirstOrDefault());
        if (script.Batches.Count != 1 || script.Batches[0].Statements.Count > 30)
            return "Verwende ein zusammenhängendes Skript ohne GO und höchstens 30 Anweisungen.";
        IList<TSqlStatement> statements = script.Batches[0].Statements;
        foreach (TSqlStatement statement in statements)
        {
            string? error = CheckStatement(statement);
            if (error != null)
                return error;
        }
        SqlSafetyVisitor safety = new(IsTable, "Verwende nur Artikel, Konten oder Rueckgaben ohne Datenbank-/Schemapräfix und ohne Tabellenhinweise. Jede Tabelle ist eine frische Arbeitskopie.");
        script.Accept(safety);
        if (safety.Error != null)
            return safety.Error;
        string? goalError = operation == null ? null : CheckLearningGoal(statements, operation.Value);
        return goalError ?? SqlWritingRules.Validate(script);
    }

    /// <summary> Übersetzt nur geprüfte Objektbezüge auf lokale temporäre SQL-Server-Tabellen. Texte bleiben unverändert. </summary>
    public string ToSessionTables(string sql)
    {
        TSqlFragment fragment = Parse(sql, out _);
        AliasVisitor aliases = new();
        fragment.Accept(aliases);
        fragment.Accept(new SessionTableVisitor(aliases.Names));
        new Sql160ScriptGenerator().GenerateScript(fragment, out string generated);
        return generated;
    }

    private static TSqlFragment Parse(string sql, out IList<ParseError> errors) => new TSql160Parser(true).Parse(new StringReader(sql), out errors);
    private static bool IsTable(SchemaObjectName name) => name.Identifiers.Count == 1 && TableNames.Contains(name.BaseIdentifier.Value, StringComparer.OrdinalIgnoreCase);

    /// <summary> Begrenzt jede äußere Anweisung auf die unterstützten Schreib-, Struktur- und Transaktionsformen. </summary>
    private static string? CheckStatement(TSqlStatement statement)
    {
        DataModificationSpecification? modification = statement switch
        {
            UpdateStatement update => update.UpdateSpecification,
            InsertStatement insert => insert.InsertSpecification,
            DeleteStatement delete => delete.DeleteSpecification,
            _ => null
        };
        if (modification != null)
        {
            if (modification.Target is not NamedTableReference target || !IsTable(target.SchemaObject) || modification.OutputClause != null || modification.OutputIntoClause != null || modification.TopRowFilter != null)
                return "Ändere direkt eine der Übungstabellen, ohne OUTPUT oder TOP. Fremde Tabellen und Aliasnamen als Änderungsziel sind hier nicht vorgesehen.";
            if (statement is InsertStatement insert && (insert.InsertSpecification.InsertSource is not ValuesInsertSource values || values.RowValues.Count > 20))
                return "Verwende INSERT mit einer Spaltenliste und VALUES für höchstens 20 neue Zeilen.";
            if (statement is DataModificationStatement data && (data.WithCtesAndXmlNamespaces != null || data.OptimizerHints.Count > 0))
                return "WITH und OPTION werden für diese Schreibübungen nicht benötigt.";
            return null;
        }
        if (statement is SelectStatement)
            return null;
        if (statement is CreateTableStatement create)
        {
            if (!IsTable(create.SchemaObjectName) || !string.Equals(create.SchemaObjectName.BaseIdentifier.Value, "Rueckgaben", StringComparison.OrdinalIgnoreCase) || create.SelectStatement != null || create.Options.Count != 0 || create.AsFileTable || create.AsNode || create.AsEdge || create.OnFileGroupOrPartitionScheme != null || create.FileStreamOn != null || create.TextImageOn != null)
                return "Erstelle in dieser Werkstatt ausschließlich die Tabelle Rueckgaben mit einfachen Spalten und Constraints.";
            return CheckDefinition(create.Definition);
        }
        if (statement is AlterTableAddTableElementStatement alter)
            return IsTable(alter.SchemaObjectName) && alter.ExistingRowsCheckEnforcement == default ? CheckDefinition(alter.Definition) : "Verwende ALTER TABLE mit ADD auf einer Übungstabelle, ohne WITH-Zusätze.";
        if (statement is BeginTransactionStatement begin)
            return begin.Distributed || begin.MarkDefined || begin.Name != null ? "Verwende BEGIN TRANSACTION ohne Namen oder weitere Optionen." : null;
        if (statement is CommitTransactionStatement commit)
            return commit.Name != null || commit.DelayedDurabilityOption != default ? "Verwende COMMIT ohne zusätzliche Optionen." : null;
        if (statement is RollbackTransactionStatement rollback)
            return rollback.Name != null ? "Verwende ROLLBACK ohne Namen oder Speicherpunkte." : null;
        return "Erlaubt sind UPDATE, INSERT, DELETE, einfaches CREATE TABLE/ALTER TABLE ADD, SELECT sowie BEGIN TRANSACTION, COMMIT und ROLLBACK. Verwaltungsbefehle sind ausgeschlossen.";
    }

    /// <summary> Prüft unterstützte Spaltentypen und Constraints und schließt erweiterte Tabellenoptionen aus. </summary>
    private static string? CheckDefinition(TableDefinition definition)
    {
        if (definition.ColumnDefinitions.Count > 10 || definition.Indexes.Count > 0 || definition.SystemTimePeriod != null)
            return "Beschränke die Tabelle auf die gefragten Spalten und einfachen Constraints.";
        foreach (ColumnDefinition column in definition.ColumnDefinitions)
        {
            if (column.DataType is not SqlDataTypeReference type || type.SqlDataTypeOption is not (SqlDataTypeOption.Int or SqlDataTypeOption.NVarChar or SqlDataTypeOption.Decimal or SqlDataTypeOption.Bit) || column.ComputedColumnExpression != null || column.DefaultConstraint != null || column.IdentityOptions != null || column.Encryption != null || column.StorageOptions != null || column.Index != null || column.Collation != null)
                return "Nutze einfache Spalten mit int, bit, decimal oder nvarchar. Berechnete Spalten, IDENTITY und Spezialoptionen gehören nicht zu diesen Aufgaben.";
            if (type.SqlDataTypeOption == SqlDataTypeOption.NVarChar && (type.Parameters.Count != 1 || !int.TryParse(type.Parameters[0].Value, out int length) || length is < 1 or > 500))
                return "Gib für nvarchar eine Länge zwischen 1 und 500 an, passend zur Aufgabe.";
            if (column.Constraints.Any(x => x is not (NullableConstraintDefinition or CheckConstraintDefinition or UniqueConstraintDefinition)))
                return "Unterstützt sind NOT NULL, PRIMARY KEY, UNIQUE und CHECK. Fremdschlüssel sind in diesen isolierten Übungen noch nicht enthalten.";
        }
        return definition.TableConstraints.Any(x => x is not (CheckConstraintDefinition or UniqueConstraintDefinition)) ? "Verwende in dieser Werkstatt PRIMARY KEY, UNIQUE oder CHECK als Tabellenconstraint." : null;
    }

    /// <summary> Verlangt den gefragten SQL-Baustein; bei Transaktionen zusätzlich die Reihenfolge der Änderungen und des Abschlusses. </summary>
    private static string? CheckLearningGoal(IList<TSqlStatement> statements, WorkshopOperation operation)
    {
        bool present = operation switch
        {
            WorkshopOperation.Update => statements.Any(x => x is UpdateStatement),
            WorkshopOperation.Insert => statements.Any(x => x is InsertStatement),
            WorkshopOperation.Delete => statements.Any(x => x is DeleteStatement),
            WorkshopOperation.CreateTable => statements.Any(x => x is CreateTableStatement),
            WorkshopOperation.AlterTable => statements.Any(x => x is AlterTableAddTableElementStatement),
            _ => true
        };
        if (!present)
            return "Verwende den in der Aufgabe gefragten SQL-Baustein. Eine reine SELECT-Abfrage verändert den Tabellenstand nicht.";
        if (operation is WorkshopOperation.Commit or WorkshopOperation.Rollback)
        {
            List<TSqlStatement> list = statements.ToList();
            int begin = list.FindIndex(x => x is BeginTransactionStatement);
            int end = list.FindIndex(x => operation == WorkshopOperation.Commit ? x is CommitTransactionStatement : x is RollbackTransactionStatement);
            int firstWrite = list.FindIndex(x => x is UpdateStatement);
            int lastWrite = list.FindLastIndex(x => x is UpdateStatement);
            if (begin < 0 || end < 0 || firstWrite <= begin || lastWrite >= end || list.Count(x => x is BeginTransactionStatement) != 1 || list.Count(x => x is CommitTransactionStatement or RollbackTransactionStatement) != 1 || operation == WorkshopOperation.Commit && list.Count(x => x is UpdateStatement) != 2)
                return "Umschließe die gefragten Änderungen mit genau einer Transaktion: BEGIN TRANSACTION, danach die UPDATE-Anweisungen, zum Schluss COMMIT beziehungsweise ROLLBACK.";
        }
        return null;
    }

    /// <summary> Erhält gültige Tabellenaliase beim Umschreiben ausgeschriebener Spaltenbezüge. </summary>
    private sealed class AliasVisitor : TSqlFragmentVisitor
    {
        /// <summary> Im Skript verwendete Aliase. </summary>
        public HashSet<string> Names { get; } = new(StringComparer.OrdinalIgnoreCase);
        public override void ExplicitVisit(NamedTableReference node)
        {
            if (node.Alias != null)
                Names.Add(node.Alias.Value);
            base.ExplicitVisit(node);
        }
    }

    /// <summary> Objektbezüge werden vor Ausführung auf ausschließlich sitzungseigene Tabellen abgebildet. </summary>
    private sealed class SessionTableVisitor(HashSet<string> aliases) : TSqlFragmentVisitor
    {
        public override void ExplicitVisit(SchemaObjectName node)
        {
            if (IsTable(node))
                node.BaseIdentifier.Value = "#" + TableNames.First(x => string.Equals(x, node.BaseIdentifier.Value, StringComparison.OrdinalIgnoreCase));
            base.ExplicitVisit(node);
        }

        public override void ExplicitVisit(ColumnReferenceExpression node)
        {
            RewriteQualifier(node.MultiPartIdentifier?.Identifiers, 2);
            base.ExplicitVisit(node);
        }

        public override void ExplicitVisit(SelectStarExpression node)
        {
            RewriteQualifier(node.Qualifier?.Identifiers, 1);
            base.ExplicitVisit(node);
        }

        public override void ExplicitVisit(CreateTableStatement node)
        {
            RemoveConstraintNames(node.Definition);
            base.ExplicitVisit(node);
        }

        public override void ExplicitVisit(AlterTableAddTableElementStatement node)
        {
            RemoveConstraintNames(node.Definition);
            base.ExplicitVisit(node);
        }

        /// <summary> Entfernt explizite Constraint-Namen, um Namenskollisionen zwischen parallelen temporären Tabellen zu vermeiden. </summary>
        private static void RemoveConstraintNames(TableDefinition definition)
        {
            // Benannte Constraints temporärer Tabellen können sonst bei parallelen Sitzungen kollidieren.
            foreach (ConstraintDefinition constraint in definition.TableConstraints.Concat(definition.ColumnDefinitions.SelectMany(x => x.Constraints)))
                constraint.ConstraintIdentifier = null;
        }

        /// <summary> Leitet ausgeschriebene Tabellennamen auf Arbeitskopien um und erhält gleichlautende gültige Aliase. </summary>
        private void RewriteQualifier(IList<Identifier>? identifiers, int count)
        {
            if (identifiers?.Count == count && TableNames.Contains(identifiers[0].Value, StringComparer.OrdinalIgnoreCase) && !aliases.Contains(identifiers[0].Value))
                identifiers[0].Value = "#" + TableNames.First(x => string.Equals(x, identifiers[0].Value, StringComparison.OrdinalIgnoreCase));
        }
    }
}
