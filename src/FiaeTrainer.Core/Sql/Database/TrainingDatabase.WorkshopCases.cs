using FiaeTrainer.Core.Sql.Content;
using FiaeTrainer.Core.Sql.Results;
using FiaeTrainer.Core.Sql.Workshop;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace FiaeTrainer.Core.Sql.Database;

public sealed partial class TrainingDatabase
{
    /// <summary> Echte, transaktionale Falltabellen für Fremdschlüssel, Sichten und Rollen. Die ungepoolte Sitzung verwirft auch bei Abbruch alle Objekte. </summary>
    internal async Task<WorkshopRun> RunScenarioAsync(WorkshopLesson lesson, List<TSqlStatement> statements, bool verification, CancellationToken request)
    {
        var scenario = WorkshopScenario.Get(lesson.Scenario);
        var builder = GetConnectionBuilder();
        builder.Pooling = false;
        await using SqlConnection connection = new(builder.ConnectionString);
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(request);
        deadline.CancelAfter(TimeSpan.FromSeconds(15));
        var token = deadline.Token;
        Task ExecuteAsync(string sql) => ExecuteWorkCommandAsync(connection, sql, token);
        bool ready = false;
        try
        {
            await OpenWorkSchemaAsync(connection, token);
            await ExecuteAsync(scenario.Seed);
            if (verification) await ExecuteAsync(scenario.Extra);
            foreach (string preparation in lesson.Preparation)
                await ExecuteAsync(preparation);
            if (lesson.Technique is "Grant" or "Revoke")
            {
                await ExecuteAsync(WorkshopScenario.RECEPTION_VIEW);
                if (lesson.Technique == "Revoke")
                    await ExecuteAsync("GRANT SELECT, UPDATE ON Tagesplan TO LernwerkEmpfangRolle;");
            }
            ready = true;
            WorkshopRun run = new();
            foreach (var statement in statements)
            {
                // Der geprüfte BEGIN/COMMIT-Rahmen der Archivaufgabe ist in die Schutztransaktion geschachtelt.
                new Sql160ScriptGenerator().GenerateScript(statement, out string sql);
                if (statement is SelectStatement)
                    run.Selection = await ReadWorkshopTableAsync(connection, sql, token);
                else
                    await ExecuteAsync(statement is GrantStatement or RevokeStatement ? ScenarioScript.PermissionCommand(statement)! : sql);
            }

            foreach (string name in scenario.Objects)
            {
                string qualified = "LernwerkProbe." + name;
                var columns = await ReadWorkshopTableAsync(connection, """
                    SELECT c.name AS Spalte, TYPE_NAME(c.user_type_id) AS Typ, c.max_length AS Bytes,
                           c.is_nullable AS NullErlaubt, CONVERT(bit,IIF(c.default_object_id=0,0,1)) AS Standardwert,
                           o.type_desc AS Objektart
                    FROM sys.columns c JOIN sys.objects o ON o.object_id=c.object_id
                    WHERE c.object_id=OBJECT_ID(@name) ORDER BY c.column_id
                    """, token, qualified);
                if (columns.Rows.Count == 0) continue;
                run.Structures[name] = columns;
                run.Tables[name] = await ReadWorkshopTableAsync(connection, $"SELECT * FROM [LernwerkProbe].[{name}];", token);
            }
            run.ConstraintChecks.Columns.AddRange(["Prüfung", "Ergebnis"]);
            var rules = await ReadWorkshopTableAsync(connection, """
                SELECT OBJECT_NAME(i.object_id) AS Objekt, CONCAT(IIF(i.is_primary_key=1 OR i.is_unique_constraint=1,'Schlüssel',i.type_desc),' PK=',i.is_primary_key,' UQ=',i.is_unique_constraint,' eindeutig=',i.is_unique,' ',
                    STRING_AGG(CONCAT(COL_NAME(ic.object_id,ic.column_id),IIF(ic.is_descending_key=1,' DESC',' ASC')),',') WITHIN GROUP (ORDER BY ic.key_ordinal)) AS Regel
                FROM sys.indexes i JOIN sys.index_columns ic ON i.object_id=ic.object_id AND i.index_id=ic.index_id
                WHERE OBJECT_SCHEMA_NAME(i.object_id)='LernwerkProbe' AND ic.key_ordinal>0
                GROUP BY i.object_id,i.index_id,i.type_desc,i.is_primary_key,i.is_unique_constraint,i.is_unique
                UNION ALL
                SELECT OBJECT_NAME(f.parent_object_id), CONCAT('FK ',COL_NAME(f.parent_object_id,c.parent_column_id),' → ',OBJECT_NAME(f.referenced_object_id),'.',
                    COL_NAME(f.referenced_object_id,c.referenced_column_id),' ',f.delete_referential_action_desc,' ',f.update_referential_action_desc,' geprüft=',1-f.is_not_trusted)
                FROM sys.foreign_keys f JOIN sys.foreign_key_columns c ON c.constraint_object_id=f.object_id
                WHERE OBJECT_SCHEMA_NAME(f.parent_object_id)='LernwerkProbe'
                ORDER BY 1,2
                """, token);
            run.ConstraintChecks.Rows.AddRange(rules.Rows);

            async Task ProbeAsync(string label, string sql)
            {
                await ExecuteAsync("SAVE TRANSACTION Werkprobe;");
                try
                {
                    var result = await ReadWorkshopTableAsync(connection, sql, token);
                    run.ConstraintChecks.Rows.Add([label, result.Rows.FirstOrDefault()?.FirstOrDefault() ?? "erlaubt"]);
                }
                catch (SqlException e) when (e.Number is 515 or 547 or 2601 or 2627 or 229 or 207 or 208)
                {
                    run.ConstraintChecks.Rows.Add([label, e.Number is 2601 or 2627 ? "nicht eindeutig" : e.Number is 229 ? "kein Zugriff" : e.Number is 547 ? "Verweis verletzt" : "nicht möglich"]);
                }
                finally { await ExecuteAsync("ROLLBACK TRANSACTION Werkprobe;"); }
            }
            if (lesson.Technique == "ForeignKey")
            {
                await ProbeAsync("Rückgabe ohne Bestellung", "INSERT Rueckgaben VALUES (901,-1,1);");
                await ProbeAsync("Rückgabe ohne Artikel", "INSERT Rueckgaben VALUES (902,10,-1);");
                await ProbeAsync("Bestellung 10 löschen · verbleibende Rückgaben", "DELETE Bestellungen WHERE Id=10; SELECT COUNT(*) FROM Rueckgaben;");
            }
            if (lesson.Technique == "Default")
                await ProbeAsync("Stufe beim Einfügen ohne Angabe", "INSERT Termine (Id,Titel,Intern,Bestaetigt) VALUES (999,'Probe','intern',0); SELECT Stufe FROM Termine WHERE Id=999;");
            if (run.Tables.ContainsKey("Positionen") && lesson.Technique is "Create" or "Unique")
            {
                string Insert(int id, int order, int article) => lesson.Technique == "Create"
                    ? $"INSERT Positionen (BestellungId,ArtikelId) VALUES ({order},{article});"
                    : $"INSERT Positionen (Id,BestellungId,ArtikelId) VALUES ({id},{order},{article});";
                await ProbeAsync("Zwei verschiedene Artikel je Bestellung", Insert(990,30,1) + Insert(991,30,2));
                await ProbeAsync("Dasselbe Paar ein zweites Mal eintragen", Insert(990,30,1) + Insert(991,30,1));
            }
            if (lesson.Technique is "Grant" or "Revoke")
            {
                await ExecuteAsync("EXECUTE AS USER='LernwerkRezeption';");
                await ProbeAsync("Tagesplan lesen · Zeilen", "SELECT COUNT(*) FROM LernwerkProbe.Tagesplan;");
                await ProbeAsync("Titel im Tagesplan ändern", "UPDATE LernwerkProbe.Tagesplan SET Titel=N'Probe' WHERE Id=1;");
                await ProbeAsync("Interne Termine lesen", "SELECT COUNT(*) FROM LernwerkProbe.Termine;");
                await ProbeAsync("Interne Termine ändern", "UPDATE LernwerkProbe.Termine SET Intern=N'Probe' WHERE Id=1;");
                // Auch ein zusätzlich erteiltes INSERT- oder DELETE-Recht muss in der Bewertung sichtbar werden.
                foreach (string name in new[] { "Tagesplan", "Termine" })
                    foreach (string right in new[] { "INSERT", "DELETE" })
                        await ProbeAsync($"{name} · {right}", $"SELECT HAS_PERMS_BY_NAME('LernwerkProbe.{name}','OBJECT','{right}');");
            }
            return run;
        }
        catch (SqlException e)
        {
            bool technical = !ready || e.Number is -1 or 53 or 26 or 4060 or 18456 or 1205;
            return new WorkshopRun { TechnicalFailure = technical, Error = technical
                ? $"Die isolierte Arbeitskopie konnte nicht bereitgestellt werden (SQL {e.Number}). Dein Versuch bleibt ungewertet."
                : e.Number == 547 ? "Eine Datenregel verhindert diese Änderung. Prüfe vorhandene Kindzeilen und Verweise auf fehlende Elternzeilen." : ExplainError(e.Number,"") };
        }
        catch (OperationCanceledException) when (!request.IsCancellationRequested)
        {
            return new WorkshopRun { TechnicalFailure = !ready, Error = "Die Werkstatt konnte den Versuch innerhalb der Zeitgrenze nicht abschließen." };
        }
    }

    private static async Task OpenWorkSchemaAsync(SqlConnection connection, CancellationToken token)
    {
        await connection.OpenAsync(token);
        using var setup = new StreamReader(SqlContent.Open("workshop-schema.sql"));
        await ExecuteWorkCommandAsync(connection, await setup.ReadToEndAsync(token), token);
        await ExecuteWorkCommandAsync(connection, """
            SET XACT_ABORT OFF;
            SET QUOTED_IDENTIFIER ON;
            BEGIN TRANSACTION;
            DECLARE @lock int;
            EXEC @lock=sys.sp_getapplock @Resource=N'AP2-Werkstattfall', @LockMode='Exclusive', @LockTimeout=6000;
            IF @lock<0 THROW 51000,N'Werkstatt belegt',1;
            IF SCHEMA_ID(N'LernwerkProbe') IS NULL OR EXISTS (SELECT 1 FROM sys.objects WHERE schema_id=SCHEMA_ID(N'LernwerkProbe'))
                THROW 51002,N'Werkstattschema nicht bereit',1;
            """, token);
        await ExecuteWorkCommandAsync(connection, "EXECUTE AS USER='LernwerkWerkautor' WITH NO REVERT; SET LOCK_TIMEOUT 2500; SET ROWCOUNT 201;", token);
    }

    private static async Task ExecuteWorkCommandAsync(SqlConnection connection, string sql, CancellationToken token)
    {
        await using SqlCommand command = new(sql, connection) { CommandTimeout = 8 };
        await command.ExecuteNonQueryAsync(token);
    }
}
