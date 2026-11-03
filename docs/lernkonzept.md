# Lernkonzept und Architektur

## Bedienung und Lernmodus

Neue Aufgaben in Ermittlungsakte und Werkstatt beginnen mit **Ohne Hilfe lösen**. Die letzte Moduswahl wird je Aufgabe gespeichert und bei Rückkehr oder Neuladen wiederhergestellt. Die Auswahl erklärt beide Modi unmittelbar; **Geführt lernen** bietet Erklärungen, drei gestufte Hinweise, Probeausführung und Musterlösung. Der Wechsel zur Unterstützung erhält den vorhandenen Entwurf. Beim Wechsel zum Selbsttest wird nur dann nachgefragt, wenn der Editor tatsächlich Text enthält. Die Kennzeichnung der Unterstützung bleibt im gespeicherten Versuch erhalten, damit ein Neuladen keine falsche Selbstständigkeit erzeugt. Frühere Profilformate bleiben lesbar; eine fehlende alte Moduswahl verwendet den Standard ohne Hilfe. Vorhandene Erfolge bleiben erhalten.

Lernschritte kennzeichnen Auftrag und Editor. **Lösung prüfen** lädt zur Rückmeldung und Verbesserung ein. Die Navigation verwendet die Bezeichnung Aufgabe; die zusätzlichen Sprunglinks zwischen Editor und Tabellen wurden auf Nutzerwunsch entfernt. Der allgemeine Bedienhinweis lässt sich per X dauerhaft im Profil ausblenden und ausschließlich unter Lernplan → Bedienhinweise wieder einschalten. Diese Einstellung gilt für beide SQL-Lernwege und beeinflusst die eigentlichen Aufgabenhilfen nicht. Kräftige serifenlose Überschriften und blaue helle beziehungsweise grau-blaue dunkle Flächen strukturieren die Oberfläche. Die Systempräferenz und die gespeicherte manuelle Designwahl funktionieren unverändert.

## Didaktischer Ablauf

Ein Kapitel erweitert den vorhandenen Wortschatz um wenige SQL-Bausteine. Jede Mission besteht aus Geschichte, kurzer Erklärung, eindeutigem Auftrag, freiem Editor und überprüfbarem Ergebnis. Hilfen werden einzeln auf Wunsch geöffnet. Eine aufgedeckte Musterlösung wird als Unterstützung gespeichert.

Die Geschichte ist ein Anlass zum Abfragen; fachliche Lernziele und präzise Ausgabespalten stehen ausdrücklich im Auftrag. Der Name „Nachtzug“ ist der Deckname des Falls, kein Hinweis auf eine vorhandene Eisenbahntabelle.

| Missionen | Fachlicher Schwerpunkt |
| --- | --- |
| 1–6 | Spalten, Filter, logische Bedingungen, Sortierung, TOP |
| 7–12 | Grenzen, Muster, Listen, fehlende Werte, Eindeutigkeit, Zeitintervalle |
| 13–18 | Schlüsselbeziehungen, innere und linke Verknüpfungen, mehrere Tabellen |
| 19–25 | Zählen, Werte zusammenfassen, Gruppen und Gruppenfilter |
| 26–30 | Unterabfragen, Korrelation, fehlende Partner und kombinierte Beweiskette |

Im Modus ohne Hilfen beginnt eine leere Antwort. Die Bewertung erscheint nach Abgabe. Bereits bekannte Aufgaben sollten zeitversetzt und zusätzlich durch unbekannte Papieraufgaben ergänzt werden. Die Anzeige „ohne Hilfen erneut gelöst“ besagt nur, dass in diesem Modus ein korrektes Ergebnis abgegeben wurde.

## Bewertungsvertrag

* Genau die verlangten Ausgabespalten in der angegebenen Reihenfolge.
* Aliasnamen, Einrückung und Zeilenumbrüche sind frei. SQL-Schlüsselwörter und Funktionen werden groß geschrieben; jede vollständige äußere Anweisung benötigt ein Semikolon. Unterabfragen bekommen kein eigenes Semikolon.
* Ohne Sortierauftrag ist die Zeilenreihenfolge irrelevant.
* Bei Sortieraufgaben ist die Reihenfolge Teil der Lösung; bei Gleichständen verlangt der Auftrag einen eindeutigen zweiten Schlüssel.
* Doppelte Zeilen bleiben erhalten. DISTINCT kann fachlich notwendig oder falsch sein.
* NULL unterscheidet sich von Leertext und dem Text „NULL“.
* Numerisch gleiche Werte unterschiedlicher Zahlentypen können übereinstimmen. Text wird nicht als Zahl akzeptiert.
* Beide Datenbestände müssen zur Referenz passen.

Der Parser akzeptiert keine beliebige T-SQL-Sprache, sondern den dokumentierten Lernumfang. CTEs, externe Datenquellen, Verwaltungsbefehle, Variablenzuweisungen, SELECT INTO und Tabellenhinweise sind in dieser Version ausgeschlossen. Das verhindert keine fachlich sinnvollen alternativen Formulierungen innerhalb der behandelten Themen, etwa NOT EXISTS statt LEFT JOIN / IS NULL.

## Datenfluss

1. Die Blazor-Seite nimmt den SQL-Text entgegen.
2. `SqlQueryValidator` prüft Syntax, Anweisungszahl und zulässige Zugriffe.
3. `TrainingDatabase` öffnet eine eigene Verbindung und wechselt unwiderruflich auf `TrainerReader`.
4. Das begrenzte Ergebnis wird als typisierte Werte einschließlich NULL gelesen.
5. `QueryEvaluator` führt die Referenzlösung aus und vergleicht mit `ResultComparer`.
6. Für die Zusatzprüfung werden echte Tabellenreferenzen von `fall` auf `probe` umgeschrieben und beide Abfragen erneut ausgeführt.
7. Die Seite zeigt Ergebnis und Feedback. `ProgressStore` schreibt den lokalen Lernstand.

Die Core-Bibliothek kennt keine Blazor-Oberfläche. Aufgaben liegen als eingebettete JSON-Ressource vor, Datenbankstruktur und Startdaten als eingebettete SQL-Ressource. Der Webteil steuert Benutzerinteraktion und Dateipersistenz. Es gibt keine externe KI-Bewertung, keine Datenübertragung an KI-Dienste und keine versteckte Nutzungsabrechnung.

## Datenbankpflege

Der Datenbankname ist fest vorgegeben. Eine erweiterte Datenbankeigenschaft kennzeichnet die Zuständigkeit der Anwendung; `dbo.TrainerVersion` kennzeichnet die Schemaversion. Ein vorhandener fremder Datenbestand wird nicht überschrieben. Die Initialisierung legt beim ersten Durchlauf beide Schemas und den eingeschränkten Benutzer in einer Transaktion an. Eine vorhandene Version wird nur erkannt, nicht automatisch migriert oder neu befüllt.

Das Prüfschema wird vom Parser in Nutzereingaben abgewiesen. Der Ausführungsbenutzer besitzt Leserechte darauf, weil der Service die kontrolliert umgeschriebenen Prüfqueries dort ausführt. Die Parsergrenze bleibt deshalb zusätzlich zum SQL-Rechtemodell erforderlich. Öffentliche SQL-Server-Systemmetadaten können unter SQL-Benutzerkontexten sichtbar sein; sie gehören nicht zum erlaubten Lernschema und werden vom Parser abgewiesen.

Bekannte Falltabellen dürfen mit oder ohne Schemapräfix angegeben werden. Der Syntaxbaum löst kurze Namen vor der Ausführung in `fall` beziehungsweise bei der Zusatzprüfung in `probe` auf. Auch Aliase, JOINs und Unterabfragen bleiben korrekt zugeordnet; Textliterale werden nicht ersetzt. Zulässig sind ausschließlich die sechs Falltabellen. Sichtbare Beispiele und der Spickzettel verwenden ausschließlich kurze Tabellennamen; die technische Zuordnung bleibt Aufgabe des Services. [Microsoft: Standardschema und Namensauflösung](https://learn.microsoft.com/en-us/sql/relational-databases/security/authentication-access/ownership-and-user-schema-separation?view=sql-server-ver17).

## Allgemeine Oberfläche und Darstellung

Die Startseite zeigt den allgemeinen AP2-Themenkompass. Der SQL-Bereich ist eine Karte unter den vorgesehenen Themen, keine dominierende Einstiegsgeschichte mehr. Der Gesamtfortschritt umfasst Ermittlungsakte und Werkstatt gemeinsam.

Die Auswahl System/Hell/Dunkel wird rein im Browser angewendet. Ein früh im Dokument geladenes kleines Skript wertet die Systempräferenz und ein validiertes Präferenzcookie aus, bevor die Styles geladen werden. Explizite Auswahl überschreibt die Systempräferenz; die Option System aktiviert deren automatische Nachführung wieder. Ein neu von Blazor eingesetzter Auswahlschalter wird mit der aktuellen Auswahl synchronisiert. Das Cookie enthält ausschließlich einen der drei festgelegten Auswahlwerte.

Semantische CSS-Farbvariablen gestalten alle bestehenden Seiten, Ergebnis- und Hilfebereiche konsistent. Das native `color-scheme`, eine ausdrückliche `caret-color` und ein zweifarbig umrandeter I-förmiger Mauszeiger verhindern schlecht sichtbare Cursor bei abweichendem System- und App-Design. Während der SQL-Ausführung ist der Editor readonly statt disabled, sodass Text und Auswahl erhalten bleiben. [MDN: Systemfarbpräferenz](https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/At-rules/@media/prefers-color-scheme), [MDN: natives Browserfarbschema](https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/Properties/color-scheme).

## SQL-Werkstatt

Die zehn ergänzenden Aufgaben verwenden einen eigenen Kontext mit Artikel- und Kontendaten. CREATE TABLE, ALTER TABLE ADD sowie einfache Constraints und Transaktionen ergänzen die freie Abfrageeingabe. Es gibt keine Lückentexte.

`WorkshopSqlValidator` akzeptiert ausschließlich den vorgesehenen SQL-Umfang. Objektbezüge werden im Syntaxbaum von den logischen Namen auf sitzungseigene temporäre Tabellen abgebildet. Texte und gültige Aliase bleiben erhalten. Ausführung und Tabellenanlage erfolgen unter demselben eingeschränkten Benutzer, der auch die Ermittlung ausführt. Permanente Schreibrechte werden nicht erteilt.

Jeder Versuch verwendet eine neue physische Verbindung und neue Startdaten. COMMIT bestätigt nur Änderungen dieser Sitzung. Die Verbindung wird nach der Prüfung geschlossen. Eine achtsekündige Gesamtgrenze und vier Sekunden pro SQL-Befehl begrenzen versehentlich aufwendige Skripte. Ein Modus- oder Aufgabenwechsel kann laufende Versuche abbrechen.

Bewertet werden sämtliche verbleibenden Tabellenzeilen, Spalten-/Typmerkmale, Schlüssel und ausgewählte Eigenschaften von CHECK-/NOT-NULL-Regeln auf zwei Startdatenbeständen. Einfügeversuche zur Prüfung der Constraints werden stets zurückgenommen. Named Constraints werden intern ohne ihre für das Lernziel irrelevanten Namen angelegt, damit parallele temporäre Tabellen nicht kollidieren.

Für ROLLBACK wird nicht nur der Endzustand geprüft. Der Service erfasst auch Artikel- und Kontenstände unmittelbar vor der Rücknahme und vergleicht sie mit der Referenz. Eine nicht ausgeführte oder falsch berechnete Probeänderung reicht deshalb nicht.

Eine allgemeine semantische Gleichwertigkeitsprüfung sämtlicher denkbarer CHECK-Ausdrücke wird nicht behauptet. Fremdschlüssel werden in diesen temporären Tabellen von SQL Server nicht erzwungen und sind deshalb ausdrücklich nicht als ausführbare Lernaufgabe enthalten. [Microsoft: CREATE TABLE und temporäre Tabellen](https://learn.microsoft.com/en-us/sql/t-sql/statements/create-table-transact-sql?view=sql-server-ver17), [BEGIN TRANSACTION](https://learn.microsoft.com/en-us/sql/t-sql/language-elements/begin-transaction-transact-sql?view=sql-server-ver17), [ROLLBACK](https://learn.microsoft.com/en-us/sql/t-sql/language-elements/rollback-transaction-transact-sql?view=sql-server-ver17).

Vorhandene Fortschrittsdateien werden um einen getrennten Werkstattbereich ergänzt. Das Standardprofil bleibt am bisherigen Ort; ein konfigurierbarer `Training:ProgressDirectory` erlaubt ein vollständig eigenes QA-Profil. Lesende und schreibende Aufgaben überschreiben gegenseitig weder Entwürfe noch Erfolge.

## Zukünftiger Pseudocode

Deutsche Formulierungen wie WENN, SONST und ENDE WENN sind sinnvoll und vertraut, aber nicht in einer einzigen allgemein verbindlichen Grammatik festgelegt. Eine prüfbare Teilmenge kann ausdrücklich beschrieben werden; freie Formulierungen sollten alternativ anhand transparenter Kriterien bewertet oder mit einer nachvollziehbaren Musterlösung verglichen werden. Lückentexte sind nicht als Hauptlernform vorgesehen.

## Ruhiger Arbeitsbereich (03.11.2026)

Nach ausgeblendetem Selbsttest-Hinweis folgt die Ermittlung dem Ablauf Lernmodus, Story, Auftrag und SQL-Editor. Allgemeine Moduserklärungen und Speicherhinweise entfallen; fachliche Erklärungen des geführten Lernens stehen unter dem Editor bei den Hilfen. Der notwendige Neustart eines übernommenen Versuchs bleibt als kurze Aktion erreichbar. Die Aufgabenliste lässt sich in beiden Lernwegen ein- und ausklappen. Ihre Anzeige wird im gemeinsamen lokalen Profil gespeichert; der doppelte Rücklink zur Übersicht entfällt. Der Randpfeil zeigt nur < beziehungsweise >; Titel und zugänglicher Name erklären die Aktion. Alle 30 Storytexte erläutern die Rolle der vorherigen Frage und des aktuellen Auftrags. Die Notizen stehen in Aufgabenreihenfolge, markieren ausgelassene Zwischenstücke und zeigen nur freigeschaltete Erkenntnisse. Ergebnisse fremder ungelöster Schritte werden nicht als bereits nachgewiesen vorausgesetzt.

## Schreibregeln, Datum und Lernplan (03.11.2026)

Nach der Sicherheits- und Syntaxprüfung kontrolliert SqlWritingRules die echten Parser-Tokens auf große Schlüsselwörter und die Funktionsknoten auf große Funktionsnamen. Namen, maskierte Bezeichner, Textliterale und Kommentare behalten ihre Schreibweise. Die letzte relevante Tokenposition jeder äußeren Anweisung muss ein Semikolon sein; ein Semikolon in einem Text oder Kommentar zählt nicht. Syntax-/Sicherheitsfehler bleiben von Trainingsregeln unterscheidbar. Bestehende Entwürfe werden nicht automatisch umgeschrieben. Die Regeln sind ausdrücklich Anforderungen dieses Trainers; eine allgemeine verbindliche IHK-Bewertungsregel wird daraus nicht abgeleitet.

Beispiele und Tabellen zeigen Datum/Uhrzeit als YYYY-MM-DD HH:mm:ss, auch Mitternacht. Die Abfragesitzungen setzen DATEFORMAT ymd ausdrücklich, damit die gewünschte Schreibweise mit Leerzeichen im Trainer zuverlässig funktioniert. Ein Integrationstest verwendet eine deutsche Verbindungssprache. Andere SQL-Server-Umgebungen müssen ihre Sitzungseinstellungen berücksichtigen; das datetime2-Format mit T ist davon unabhängig. [Microsoft: datetime2](https://learn.microsoft.com/en-us/sql/t-sql/data-types/datetime2-transact-sql), [Microsoft: SET DATEFORMAT](https://learn.microsoft.com/en-us/sql/t-sql/statements/set-dateformat-transact-sql), [Microsoft ScriptDOM: Token-Klassifikation](https://github.com/microsoft/SqlScriptDOM/blob/main/SqlScriptDom/Parser/TSql/TSqlParserToken.cs).

Der Lernplan gibt keine Wochen vor. Er bietet neue Aufgaben ohne bisherigen Entwurf oder Abgabe, begonnene ungelöste Aufgaben einschließlich reiner Entwürfe sowie abgeschlossene Aufgaben ohne bisherigen selbstständigen Erfolg getrennt an. Vollständig bearbeitete oder vollständig begonnene Kataloge erhalten passende Rückmeldungen. Die Empfehlungen sind aus dem Profil abgeleitete Arbeitsvorschläge, keine Diagnose des Wissensstands.

## Lokale Windows-Verteilung (03.11.2026)

Die Veröffentlichung verwendet eine eigenständige Windows-x64-EXE mit eingebauter .NET-/ASP.NET-Core-Laufzeit und Browseroberfläche. Der SQL Server bleibt separat. Login, Registrierung, Nutzertabellen und öffentlicher Homeserver-/nginx-Betrieb sind auf Nutzerwunsch entfallen. Das Quellprojekt behält seinen bisherigen Speicherort und optionalen Heimnetz-Modus.

Nur das Publish-Paket enthält appsettings.Release.json als Startkennzeichnung. Es lädt Inhalte unabhängig vom aktuellen Arbeitsverzeichnis aus dem Programmordner; persönliche Einstellungen kommen aus LocalApplicationData/AP2-Lernwerk/appsettings.Local.json. Der Fortschritt liegt standardmäßig daneben unter progress. Umgebungsvariablen und Startparameter können Einstellungen gezielt überschreiben; QA-Starts erhalten dadurch ein eigenes Profil. Die Konfiguration im Paket ist eine allgemeine SQL-Express-Vorlage. Private Einstellungen und Lernstände werden nicht übernommen. Release-erstellen.ps1 erzeugt einen neuen Stagingordner, prüft den Inhalt, fügt Lizenzhinweise hinzu und erzeugt ZIP plus SHA256-Datei.

## Ersteinrichtung, Version und Updates

Die zentrale Version in Directory.Build.props wird über AssemblyInformationalVersion aus dem tatsächlichen Build gelesen, ohne Commitmetadaten angezeigt und für SemVer-Vergleiche verwendet. UpdateCheckWorker startet im Hintergrund; UpdateChecker lädt nur öffentliche Metadaten des konfigurierten GitHub-Repositories, keine Lernstände. Vorab-/Stable-Kanäle, fertiger Windows-Anhang, API-Fehler, Timeout und eine kurze Wiederholungssperre werden berücksichtigt. Fremde Links und Bodytexte der API werden nicht als Handlungsanweisungen übernommen; der Release-Link wird selbst aus geprüftem Repository und Tag gebildet. [GitHub: Releases API](https://docs.github.com/en/rest/releases/releases#list-releases), [SemVer 2.0](https://semver.org/spec/v2.0.0.html).

Die SQL-Erkennung liest registrierte lokale Instanzen und LocalDB-Installationen aus beiden Windows-Registryansichten. Sie verändert keine Dienste und probiert keine willkürlichen Instanzen aus. Die echte Verbindung wird beim Start zur gespeicherten Instanz beziehungsweise bei einer vom Nutzer gewählten Einrichtung geprüft. LocalSettingsStore erzeugt für das Formular eine Windows-Anmeldung mit festem Trainingsdatenbanknamen und schreibt erst nach erfolgreichem Vorbereiten der eigenen Datenbank. Unbekannte oder beschädigte Einstellungsbereiche werden erhalten beziehungsweise nicht überschrieben.

DatabaseState hält die aktuell geprüfte TrainingDatabase-Instanz. Neue SQL-Seiten erhalten darüber eigene Evaluatoren; laufende Auswertungen behalten ihre immutable Verbindung, damit ein Einstellungswechsel keine halbe Bewertung zwischen zwei Servern mischt. Bereits geöffnete Seiten nach einem Wechsel neu öffnen. Konfigurationsänderungen sind im Heimnetz-Modus serverseitig gesperrt, bevor ein neuer Datenbankserver angesprochen wird. [Microsoft: Instanzregistrierung](https://learn.microsoft.com/en-us/sql/sql-server/install/file-locations-for-default-and-named-instances-of-sql-server), [Microsoft: LocalDB-Registrierung](https://learn.microsoft.com/en-us/sql/relational-databases/express-localdb-instance-apis/sql-server-express-localdb-header-and-version-information).
