# Lernkonzept und Architektur

## Bedienung und Lernmodus

Neue Aufgaben in Ermittlungsakte und Werkstatt beginnen mit **Ohne Hilfe lösen**. Die letzte Moduswahl wird je Aufgabe gespeichert und bei Rückkehr oder Neuladen wiederhergestellt. Die Auswahl erklärt beide Modi unmittelbar; **Geführt lernen** bietet Erklärungen, drei gestufte Hinweise, Probeausführung und Musterlösung. Der Wechsel zur Unterstützung erhält den vorhandenen Entwurf. Beim Wechsel zum Selbsttest wird nur dann nachgefragt, wenn der Editor tatsächlich Text enthält. Die Kennzeichnung der Unterstützung bleibt im gespeicherten Versuch erhalten, damit ein Neuladen keine falsche Selbstständigkeit erzeugt. Frühere Profilformate bleiben lesbar; eine fehlende alte Moduswahl verwendet den Standard ohne Hilfe. Vorhandene Erfolge bleiben erhalten.

Lernschritte kennzeichnen Auftrag und Editor. **Lösung prüfen** lädt zur Rückmeldung und Verbesserung ein. Die Navigation verwendet die Bezeichnung Aufgabe; die zusätzlichen Sprunglinks zwischen Editor und Tabellen wurden auf Nutzerwunsch entfernt. Der allgemeine Bedienhinweis lässt sich per X dauerhaft im Profil ausblenden und ausschließlich unter Lernplan → Bedienhinweise wieder einschalten. Diese Einstellung gilt für beide SQL-Lernwege und beeinflusst die eigentlichen Aufgabenhilfen nicht. Kräftige serifenlose Überschriften und blaue helle beziehungsweise grau-blaue dunkle Flächen strukturieren die Oberfläche. Die Systempräferenz und die gespeicherte manuelle Designwahl funktionieren unverändert.

## Didaktischer Ablauf

Bei Aufgabenwechseln innerhalb eines SQL-Lernwegs bleibt die linke Liste mit ihrer Scrollposition erhalten. Der Ladehinweis ersetzt nur den Aufgabeninhalt. Im geführten Hilfebereich besitzen Denkanstoß, Spickzettel und Musterlösung denselben Aufbau: links Überschrift mit kurzer Beschreibung, rechts die Aktion. Hinweise öffnen weiterhin einzeln von 1 bis 3. Der Spickzettel öffnet im selben Tab; der gespeicherte Entwurf bleibt beim Zurückkehren erhalten. Bei der Musterlösung wechseln die Aktionen zwischen „Ansehen“ und „Ausblenden“. Das Schließen entfernt keine bereits protokollierte Unterstützung und macht einen Versuch nicht nachträglich selbstständig.

Ein Kapitel erweitert den vorhandenen Wortschatz um wenige SQL-Bausteine. Jede Aufgabe besteht aus Geschichte, kurzer Erklärung, eindeutigem Auftrag, freiem Editor und überprüfbarem Ergebnis. Hilfen werden einzeln auf Wunsch geöffnet. Eine aufgedeckte Musterlösung wird als Unterstützung gespeichert.

Die Geschichte ist ein Anlass zum Abfragen; fachliche Lernziele und präzise Ausgabespalten stehen ausdrücklich im Auftrag. Der Name „Nachtzug“ ist der Deckname des Falls, kein Hinweis auf eine vorhandene Eisenbahntabelle.

| Aufgaben | Fachlicher Schwerpunkt |
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

Die Startseite zeigt den allgemeinen AP2-Themenkompass. Der SQL-Bereich ist eine Karte unter den vorgesehenen Themen, keine dominierende Einstiegsgeschichte mehr. Der Gesamtfortschritt umfasst die 40 SQL-, 30 Algorithmus-, 42 Datenbankentwurfs- und 150 WiSo-Aufgaben, insgesamt 262. Prüfungsaufgaben bleiben von diesem Lernzähler getrennt. Die Kopfnavigation beginnt immer mit Übersicht und Lernplan; danach folgen die Links des besuchten Moduls.

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

## Pseudocode und Algorithmen

Deutsche Formulierungen wie WENN, SONST und ENDE WENN sind sinnvoll und vertraut, aber nicht in einer einzigen allgemein verbindlichen Grammatik festgelegt. Der Lernweg enthält 30 Aufgaben mit einer ausdrücklich beschriebenen ausführbaren Teilmenge. Nach acht Grundlagenaufgaben folgen Kennzahlen, lineare und binäre Suche, Indexänderungen, Sortierung, verschachtelte Listen, Funktionen und Rekursion. Ergänzende Abnahmeaufgaben behandeln Prüfziffern, konkreten Schleifenaufwand und Positionspaare. Freie Codeantworten werden mit mehreren Eingaben und unabhängig hinterlegten Sollausgaben ausgeführt; andere korrekte Lösungswege bleiben möglich. Schreibtischtests prüfen die Reihenfolge tatsächlicher Ausgaben. Zwei Testfallaufgaben prüfen eigene Eingaben und Sollausgaben sowie die verlangten Grenzfallgruppen. Lückentexte sind nicht vorgesehen.

Auf Nutzerwunsch verlangt der Trainer großgeschriebene Schlüsselwörter, Wortoperatoren, Wahrheitswerte und die Längenfunktion; Variablennamen bleiben von Groß-/Kleinschreibung unabhängig, Textwerte unverändert. Das ist die vereinbarte Schreibregel der App, keine Behauptung einer allgemein verbindlichen Pseudocode-Norm. Zeilennummern im Eingabefeld stimmen mit Fehlermeldungen und Schrittansicht überein. Tab und Umschalt+Tab erleichtern das Einrücken, Esc vor Tab erhält einen Tastaturweg aus dem Feld. Für reine Ausgaben und Testfallantworten bleibt Tab eine Navigationstaste.

Funktionsaufgaben nennen einen Einstieg und seine Argumentreihenfolge. Die Lernenden reichen Definitionen ein; der Prüfer ruft die Funktion mit unterschiedlichen Werten auf und prüft ihre Rückgabe. Die Vorschau zeigt den aktiven lokalen Bereich und den Beispiel-Rückgabewert. Rückgabe und Bildschirmausgabe werden ausdrücklich unterschieden. Listen verwenden im Trainer Wertkopien, damit lokale Änderungen und Aufruferwerte klar getrennt bleiben. Das ist eine dokumentierte Sprachkonvention, keine universelle Regel für Listen in allen Sprachen. Rekursion wird mit Basisfall, Fortschritt und getrennten Aufrufebenen trainiert; gemeinsam gelten höchstens 16 aktive Aufrufebenen und 10.000 Rechenschritte. Externe Funktionen bleiben außerhalb des Umfangs. Funktionale Ergebnisprüfung ist keine Bewertung der Laufzeitklasse; ein anderer korrekter Sortieralgorithmus bleibt zulässig.

„Leitstelle im Ausnahmezustand“ ist eine eigenständige Geschichte mit Bezug zum vorherigen Schritt und einem chronologischen Einsatzbericht. Beim Überspringen einer Aufgabe wird deren Ergebnis nicht als bestätigt vorausgesetzt. Ohne Hilfe ist je Aufgabe der Standard. Geführtes Lernen ergänzt Einführung, drei Hinweise, Spickzettel, Musterlösung und eine begrenzte Schrittansicht mit Variablen und Ausgaben. Ein unterstützter Versuch wird erst nach einem ausdrücklichen Neustart wieder unabhängig; beim unveränderten Reparaturcode oder leerem Editor ist keine Löschbestätigung nötig. SQL-Stände bleiben getrennt erhalten. Einzelheiten: [Pseudocode](pseudocode.md).

Auf Nutzerwunsch verwenden alle sichtbaren Pseudocode-Zuweisungen bevorzugt `=`. Der Spickzettel unterscheidet davon Gleichheitsvergleiche mit `==`. Bereits gespeicherte Lösungen mit `←`, `<-` oder `:=` bleiben ausführbar; sie werden nicht automatisch umgeschrieben. Die Themenkarte enthält keinen technischen Hinweis zur SQL-Unabhängigkeit mehr.

## Ruhiger Arbeitsbereich (03.11.2026)

Nach ausgeblendetem Selbsttest-Hinweis folgt die Ermittlung dem Ablauf Lernmodus, Story, Auftrag und SQL-Editor. Allgemeine Moduserklärungen und Speicherhinweise entfallen; fachliche Erklärungen des geführten Lernens stehen unter dem Editor bei den Hilfen. Der notwendige Neustart eines übernommenen Versuchs bleibt als kurze Aktion erreichbar. Die Aufgabenliste lässt sich in beiden Lernwegen ein- und ausklappen. Ihre Anzeige wird im gemeinsamen lokalen Profil gespeichert; der doppelte Rücklink zur Übersicht entfällt. Der Randpfeil zeigt nur < beziehungsweise >; Titel und zugänglicher Name erklären die Aktion. Alle 30 Storytexte erläutern die Rolle der vorherigen Frage und des aktuellen Auftrags. Die Notizen stehen in Aufgabenreihenfolge, markieren ausgelassene Zwischenstücke und zeigen nur freigeschaltete Erkenntnisse. Ergebnisse fremder ungelöster Schritte werden nicht als bereits nachgewiesen vorausgesetzt.

## Schreibregeln, Datum und Lernplan (03.11.2026)

Nach der Sicherheits- und Syntaxprüfung kontrolliert SqlWritingRules die echten Parser-Tokens auf große Schlüsselwörter und die Funktionsknoten auf große Funktionsnamen. Namen, maskierte Bezeichner, Textliterale und Kommentare behalten ihre Schreibweise. Die letzte relevante Tokenposition jeder äußeren Anweisung muss ein Semikolon sein; ein Semikolon in einem Text oder Kommentar zählt nicht. Syntax-/Sicherheitsfehler bleiben von Trainingsregeln unterscheidbar. Bestehende Entwürfe werden nicht automatisch umgeschrieben. Die Regeln sind ausdrücklich Anforderungen dieses Trainers; eine allgemeine verbindliche IHK-Bewertungsregel wird daraus nicht abgeleitet.

Beispiele und Tabellen zeigen Datum/Uhrzeit als YYYY-MM-DD HH:mm:ss, auch Mitternacht. Die Abfragesitzungen setzen DATEFORMAT ymd ausdrücklich, damit die gewünschte Schreibweise mit Leerzeichen im Trainer zuverlässig funktioniert. Ein Integrationstest verwendet eine deutsche Verbindungssprache. Andere SQL-Server-Umgebungen müssen ihre Sitzungseinstellungen berücksichtigen; das datetime2-Format mit T ist davon unabhängig. [Microsoft: datetime2](https://learn.microsoft.com/en-us/sql/t-sql/data-types/datetime2-transact-sql), [Microsoft: SET DATEFORMAT](https://learn.microsoft.com/en-us/sql/t-sql/statements/set-dateformat-transact-sql), [Microsoft ScriptDOM: Token-Klassifikation](https://github.com/microsoft/SqlScriptDOM/blob/main/SqlScriptDom/Parser/TSql/TSqlParserToken.cs).

Der Lernplan gibt keine Wochen vor. Er bietet neue Aufgaben ohne bisherigen Entwurf oder Abgabe, begonnene ungelöste Aufgaben einschließlich reiner Entwürfe sowie abgeschlossene Aufgaben ohne bisherigen selbstständigen Erfolg getrennt an. Vollständig bearbeitete oder vollständig begonnene Kataloge erhalten passende Rückmeldungen. Die Empfehlungen sind aus dem Profil abgeleitete Arbeitsvorschläge, keine Diagnose des Wissensstands.

## Wirtschafts- und Sozialkunde (04.11.2026)

WiSo ergänzt 150 eigene Fälle aus acht Themenbereichen und behält den letzten Platz im ursprünglichen Themenkompass. Themen und Aufgabenformen orientieren sich an den vorgelegten alten Prüfungen; Texte und Abbildungen sind eigenständig, fachliche Aussagen anhand aktueller Primärquellen gepflegt. Es gibt Einfach-/Mehrfachauswahl, Zuordnung, Reihenfolge, Zahlen-, Datums- und Uhrzeitergebnisse. Die Referenzseite bündelt Zusammenhänge und Formeln, statt die Aufgaben durch Lückentexte zu ersetzen. Ein kurzer Einstieg erklärt Unsicherheit gegenüber der reinen Kontrollmarkierung. Quellenlisten verzichten auf wiederholte Tab-Hinweise an jedem Link; ein gemeinsamer Hinweis genügt.

Themenrunden geben Raum für gezieltes Üben. Gemischte Zehnerrunden verbinden neue Aufgaben mit Wiederholung; die Fehlerliste richtet sich nach dem letzten falschen oder unsicheren Versuch. Im Training folgt nach der Antwort eine Erklärung einschließlich typischer Verwechslungen. Aufgedeckte Antworten sind für diese Runde gesperrt, ein neuer Versuch beginnt in einer neuen Runde. Kontrollmarkierungen und Unsicherheit sind getrennte Entscheidungen. Frühere vollständige Erfolge bleiben sichtbar, spätere Fehler werden trotzdem wiederholt.

Die Prüfungsrunde wählt 30 unterschiedliche Aufgaben aus allen acht Themen und allen sieben Formaten. Sie gibt 60 Minuten und zeigt keine vorzeitige Lösung. Pausieren ist ausdrücklich möglich; beim bloßen Verlassen der Seite läuft die gespeicherte Frist weiter. Nachschlagen wird als Unterstützung kenntlich gemacht. Abgabebestätigung und anschließendes Ergebnis erhalten sichtbar den Fokus, damit die Aktion auch vom Seitenende aus unmittelbar nachvollziehbar bleibt. Teilpunkte für Ergebnisfelder, Nullpunkte bei Unbeantwortetem und die Prozentberechnung sind vor der Nutzung nachvollziehbar beschrieben. Es wird keine amtliche IHK-Note oder vollständige Prüfungsprognose daraus abgeleitet.

SQL, Algorithmen und WiSo besitzen getrennte Antworten und Fortschritte innerhalb eines lokalen Profils. Eine offene WiSo-Runde kann fortgesetzt werden; Ersetzen erfordert eine ausdrückliche Bestätigung. Die letzten 20 abgeschlossenen Runden bleiben erhalten. Fachliche Grenzen, Quellenpflege, Dateiverträge und Eingabeformate: [WiSo](wiso.md).

## Projektbereich (04.11.2026)

Unterstützung und Rückmeldungen stehen am Ende der Übersicht und in der Fußzeile. Eine Lernaufgabe wird dadurch nicht unterbrochen. „Über das Lernwerk“ erläutert Urheberschaft, Entstehung und Rückmeldewege. Ein GitHub-Stern dient der Anerkennung; Fehler und Vorschläge erhalten einen getrennten Issues-Link. Die freiwillige PayPal-Unterstützung erfolgt extern. Die interne Projektseite erhält den Modulkontext, externe Links öffnen einen weiteren Tab und lassen die aktuelle Lernseite bestehen. Es gibt keine Auswirkung auf Aufgabenbewertung oder Fortschritt. Details: [Projektauftritt](projektauftritt.md).

## Lokale Windows-Verteilung (03.11.2026)

Die spätere Veröffentlichung verwendet eine eigenständige Windows-x64-EXE mit eingebauter .NET-/ASP.NET-Core-Laufzeit und Browseroberfläche. Der SQL Server bleibt separat. Login, Registrierung, Nutzertabellen und öffentlicher Homeserver-/nginx-Betrieb sind auf Nutzerwunsch entfallen. Das Quellprojekt behält seinen bisherigen Speicherort und optionalen Heimnetz-Modus.

Nur das Publish-Paket enthält appsettings.Release.json als Startkennzeichnung. Es lädt Inhalte unabhängig vom aktuellen Arbeitsverzeichnis aus dem Programmordner; persönliche Einstellungen kommen aus LocalApplicationData/AP2-Lernwerk/appsettings.Local.json. Der Fortschritt liegt standardmäßig daneben unter progress. Umgebungsvariablen und Startparameter können Einstellungen gezielt überschreiben; QA-Starts erhalten dadurch ein eigenes Profil. Die Konfiguration im Paket ist eine allgemeine SQL-Express-Vorlage. Private Einstellungen und Lernstände werden nicht übernommen. Release-erstellen.ps1 erzeugt einen neuen Stagingordner, prüft den Inhalt, fügt Lizenzhinweise hinzu und erzeugt ZIP plus SHA256-Datei.

## Ersteinrichtung, Version und Updates

Die zentrale Version in Directory.Build.props wird über AssemblyInformationalVersion aus dem tatsächlichen Build gelesen, ohne Commitmetadaten angezeigt und für SemVer-Vergleiche verwendet. UpdateCheckWorker startet im Hintergrund; UpdateChecker lädt nur öffentliche Metadaten des konfigurierten GitHub-Repositories, keine Lernstände. Vorab-/Stable-Kanäle, fertiger Windows-Anhang, API-Fehler, Timeout und eine kurze Wiederholungssperre werden berücksichtigt. Fremde Links und Bodytexte der API werden nicht als Handlungsanweisungen übernommen; der Release-Link wird selbst aus geprüftem Repository und Tag gebildet. [GitHub: Releases API](https://docs.github.com/en/rest/releases/releases#list-releases), [SemVer 2.0](https://semver.org/spec/v2.0.0.html).

Die SQL-Erkennung liest registrierte lokale Instanzen und LocalDB-Installationen aus beiden Windows-Registryansichten. Sie verändert keine Dienste und probiert keine willkürlichen Instanzen aus. Die echte Verbindung wird beim Start zur gespeicherten Instanz beziehungsweise bei einer vom Nutzer gewählten Einrichtung geprüft. LocalSettingsStore erzeugt für das Formular eine Windows-Anmeldung mit festem Trainingsdatenbanknamen und schreibt erst nach erfolgreichem Vorbereiten der eigenen Datenbank. Unbekannte oder beschädigte Einstellungsbereiche werden erhalten beziehungsweise nicht überschrieben.

DatabaseState hält die aktuell geprüfte TrainingDatabase-Instanz. Neue SQL-Seiten erhalten darüber eigene Evaluatoren; laufende Auswertungen behalten ihre immutable Verbindung, damit ein Einstellungswechsel keine halbe Bewertung zwischen zwei Servern mischt. Bereits geöffnete Seiten nach einem Wechsel neu öffnen. Konfigurationsänderungen sind im Heimnetz-Modus serverseitig gesperrt, bevor ein neuer Datenbankserver angesprochen wird. [Microsoft: Instanzregistrierung](https://learn.microsoft.com/en-us/sql/sql-server/install/file-locations-for-default-and-named-instances-of-sql-server), [Microsoft: LocalDB-Registrierung](https://learn.microsoft.com/en-us/sql/relational-databases/express-localdb-instance-apis/sql-server-express-localdb-header-and-version-information).

## Orientierung im Quellprojekt

Seit 0.1.1-alpha.2 liegt die SQL-Fachlogik vollständig unter FiaeTrainer.Core/Sql mit getrennten Bereichen für Inhalte, Datenbank, Ermittlung, Ergebnisse, Prüfung und Werkstatt. Allgemeine Webdienste und Lernbedienelemente sind vom SQL-Modul getrennt. Namespaces entsprechen den Ordnern. Die Bewertung, Datenbankidentität und JSON-Lernstände ändern sich dadurch nicht. Der Ressourcenloader SqlContent gehört zum Inhaltsbereich und wird von beiden Katalogen und dem Datenbankzugriff verwendet. Konkrete Einstiegspunkte: [Entwicklung](entwicklung.md).

## Verbindungsabbrüche und Sitzungsfortsetzung

Seit 0.1.1-alpha.3 ersetzt ein statischer deutscher Dialog die Standardanzeige des Frameworks. Er bleibt ohne aktive Blazor-Verbindung bedienbar und verwendet dieselben Flächen-, Text- und Aktionsfarben wie die App. Zustände für Suche, Wiederholungsversuch, Pause, fehlgeschlagenes Fortsetzen und verlorene Sitzung werden getrennt erklärt. Der Nutzer entscheidet selbst über ein Neuladen; die Oberfläche verspricht keinen Erhalt ungesicherter Eingaben. Die allgemeine Fehlerleiste verwendet ausdrücklich die Warnfarben auch für ihren Link. [Microsoft: Verbindungszustand und eigener Dialog](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/signalr?view=aspnetcore-10.0#reflect-the-server-side-connection-state-in-the-ui).

Der SQL-Spickzettel bietet wie der Pseudocode-Spickzettel einen Button „Zurück zu deiner Aufgabe“. Er erhält Ermittlung oder Werkstatt samt Aufgabennummer auch nach einem Neuladen. Der Rücksprung verwendet ausschließlich validierte lokale SQL-Pfade; er startet keinen neuen Versuch und verändert den gespeicherten Entwurf nicht.

## Zeitlich begrenztes Anwenden

Die neuen SQL-/Pseudocode-Prüfungen ergänzen den geführten Lernweg mit eigenen Aufgaben, fester UTC-Frist und Auswertung nach der Abgabe. Prüfungsleistungen bleiben von den bisherigen Lernabschlüssen getrennt. Nachschlagen ist möglich, pausiert und kennzeichnet den Versuch; Musterlösungen und Ausführungswerkzeuge öffnen erst in der Nachbereitung. Endliche Prüfmengen und feste Trainingspunkte ersetzen keine menschliche IHK-Korrektur. Details: [Prüfungsmodus](pruefungsmodus.md).

## Datenbankentwurf

Die Eventagentur Lichtblick gibt dem neuen Lernweg ein gemeinsames Ziel: einen nachvollziehbaren relationalen Entwurf aus verstreuten Unterlagen entwickeln. 42 Aufgaben verbinden Begriffsverständnis, eigene Modellierung, Reparatur, ER-Ergänzungen, Entwürfe aus Unterlagen und fachliche Entscheidungen. Die konkrete Ausgangssituation wird jeweils genannt, ungelöste Vorgänger werden nicht als bewiesene Ergebnisse vorausgesetzt. Bestätigte Planungsschritte enthalten nur bereits gelöste Aufgaben bis zum aktuellen Stand.

Der Baukasten trennt die fachliche Wahl von der Zeichenarbeit. Attribute aus dem Auftrag werden eigenen Tabellen zugeordnet, Schlüssel und Referenzen markiert, Kardinalitäten in beide Richtungen beschrieben. Namen und Reihenfolge sind frei. Die Bewertung folgt getesteten Modellvarianten und ausdrücklich genannten Abhängigkeiten, keinem allgemeinen Normalformbeweiser. Drei unabhängige 45-Minuten-Prüfungsfälle ergänzen den Lernweg. [Bewertungsvertrag](datenbankentwurf.md).

## Lichtblick-Geschichte nach Nutzerfeedback (07.11.2026)

Lea Berger begleitet alle 30 Entwurfsaufgaben bei der Vorbereitung des Herbstforums. Ein konkretes Problem im Büro, der Disposition oder der Personalplanung begründet jeweils den nächsten Auftrag. Spätere Aufgaben greifen bekannte Ausschnitte mit einer neuen Frage wieder auf; abweichende Regeln und absichtlich begrenzte Arbeitsblätter werden ausdrücklich eingeordnet. Die Geschichte bleibt auch beim direkten Einstieg in eine spätere Aufgabe verständlich.

Ein Übergang darf den vorherigen Schritt nur nach tatsächlichem Abschluss als gelöst beschreiben. Sonst nennt die Karte den offenen Vorgänger mit Rücklink. Erzählerische Planungsergebnisse werden erst durch die richtige Lösung freigeschaltet; der chronologische Bericht kennzeichnet ausgelassene Schritte und blendet spätere Erfolge bei früheren Aufgaben aus. Fachliche Lösungserklärung und erzählerisches Ergebnis bleiben getrennt. Aufgabenverträge und vorhandene Leistungen werden nicht neu interpretiert.

## Lernrückmeldung und Prüfungswertung trennen (07.11.2026)

Abschnitt 04 meldet wie SQL/Pseudocode verständlich zurück, ob der Schritt funktioniert. Eine einzige Karte enthält Ergebnis, freigeschaltete Erkenntnis und Einordnung der Hilfenutzung. Bei unvollständigen Modellen erklären einzelne Kriterien die verbleibenden Abweichungen. Trainingspunkte und Prozentwerte gehören zur Prüfung, nicht zur Lernrückmeldung. Eine falsche Auswahl zeigt nicht automatisch die vollständige fachliche Lösungserklärung; freiwillige Hinweise und Musterlösung bleiben dem geführten Lernen zugeordnet. Korrigierte Antworten und Moduswechsel entfernen veraltetes Feedback. Historische Erfolge bleiben beim neuen Versuch erhalten.

## Diagramme lesen, ergänzen und übertragen (07.11.2026)

Aufgaben 31–42 ergänzen Abschnitt 04 auf 42 Aufgaben. Der Arbeitsweg beginnt beim Lesen fachlicher ER-Abbildungen und ihren gezielten Ergänzungen, führt über die Übertragung in relationale Tabellen zu vorbereiteten Modellerweiterungen und anschließend zu Entwürfen aus einer Standliste und einem Leihschein. Beziehungsattribute bleiben von Entitätsattributen unterscheidbar. Konkrete Anomalien schließen den Lernweg ab.

Die ursprüngliche Vorlage bleibt bei relationalen Ergänzungen sichtbar. ER-Ergänzungen erscheinen unmittelbar in der Abbildung; Hinweise und Musterlösung bleiben dem geführten Modus zugeordnet. Textbeschreibungen und beschriftete Felder ermöglichen die Bearbeitung ohne notwendiges Ziehen. Eigene Fälle setzen Leas Geschichte fort; Originalprüfungen werden nicht eingebettet. Die Erweiterung gehört nur zur Quellfassung.

## UML und Modellierung seit Alpha 13

Abschnitt 03 erweitert denselben Lernrahmen um 36 Aufgaben: vier Grundlagen, sechs Anwendungsfälle, acht Klassen sowie je sechs Aktivitäten, Sequenzen und Zustände. Sieben Aufgaben trainieren Lesen, sechs Reparatur und 23 das eigene Modellieren von einer leeren Arbeitsfläche. Mira König begleitet die Entwicklung im Kulturzentrum Nordhafen. Übergänge setzen keine unbestätigten Vorgänger voraus; Erkenntnisse erscheinen erst nach Erfolg und bleiben im chronologischen Bericht nachvollziehbar.

Eigene Diagramme entstehen durch Hinzufügen, Zuordnen und Verbinden von Elementen; Anzeigenamen sind frei, Fachrollen verbindlich. Zeichenpositionen entstehen automatisch und zählen nicht zur Bewertung. Fehlerhafte Entwürfe erhalten Kriterien und Markierungen; im Lernen keine Punkte. Geführtes Lernen, drei Hinweise und eine ausdrücklich aufgedeckte Musterlösung zählen als Unterstützung. Der frische Selbsttest schützt vorhandene Entwürfe mit der gemeinsamen Bestätigung. Drei unabhängige Prüfungsfälle übertragen den Stoff in unbekannte Situationen. Konkrete Notations-/Bewertungsgrenzen: [UML-Vertrag](uml.md).

## Softwareplanung und OOP seit Alpha 14

48 Aufgaben führen Mira Königs Nordhafen-Planung vom präzisen Kundenwunsch zu konkreten Softwareentwürfen. Acht Kapitel trennen Anforderungen, Objektzustände, Erweiterbarkeit, Entwurfsmuster, Architektur, Schnittstellen, Formulare und Werkzeuge. Ausarbeiten und eigene Entwürfe ergänzen Lesen/Reparieren. Klassen- und Architekturbeziehungen werden sichtbar, API-Nachrichten selbst geschrieben und Formulargruppen direkt ausprobiert. Codeunterlagen trainieren konkretes Zustands-/Rückgabeverhalten ohne beliebige Programmausführung.

Zusätzliche Freitextfelder und die zugehörigen Selbstkontrollkarten wurden auf Nutzerwunsch entfernt. Die Lernrückmeldung enthält keine Punkte; drei unabhängige Trainingsfälle übertragen das Wissen. Übergänge setzen nur bestätigte Vorgänger voraus; größere offene Abschnitte bleiben im Bericht gesammelt aufklappbar, statt die Leseführung mit vielen einzelnen Leerstellen zu unterbrechen. [Fachlicher Umfang und Grenzen](softwareplanung.md).

## Abschnitt 06: Vom Entwurf zum belegten Probebetrieb (Alpha 15)

54 neue eigene Aufgaben in neun Kapiteln ergänzen den Quellumfang auf 400 Übungen in sieben Modulen. Mira führt Nordhafen aus der Planung in Test und Qualitätsprüfung. Jeder Auftrag besitzt seine eigenen nötigen Annahmen; Vorgänger werden nur nach tatsächlichem Abschluss als bestätigt bezeichnet. Ein begrenzter Prüfbericht zeigt Erfolge bis zur aktuellen Aufgabe und hält offene Lücken erkennbar.

Eigene Testfälle mit Eingaben und unabhängigen Sollwerten bilden den praktischen Schwerpunkt. Geeignete alternative Klassenvertreter sind erlaubt. Die Auswertung trennt tatsächliche Ausführung, richtige Erwartung, Abdeckung und nachgewiesene Fehlervarianten. Reviewmarkierungen, eigene Berechtigungsmatrix und ohne Ziehen zusammengesetzte Abläufe ergänzen Ausarbeitungen. Sichtbare Reparaturvorlagen bleiben zum Vergleich erhalten. Zusätzliche Freitextfelder und die zugehörigen Selbstkontrollkarten wurden auf Nutzerwunsch entfernt. Ein Lernfeedback ohne Trainingspunkte, freiwillige Hilfen und drei getrennte Prüfungsfälle folgen dem bekannten Ablauf. Fachliche Grenzen und Quellen: [Tests, Qualität und Sicherheit](qualitaet.md). Kein neues Paket; ausführliche Nutzererprobung und Zeitkalibrierung bleiben offen.

## Projekte und Wirtschaftlichkeit – eigener Folgeauftrag seit Alpha 16

54 Aufgaben führen Mira König durch die Planung des neuen Kursbereichs. Struktur-, Netz- und Terminpläne werden selbst gebaut, ergänzt und repariert; Rechentabellen verbinden Zahlen mit begründeten Auswahlentscheidungen. Eigene Beschriftungen sind frei, andere gültige Pläne werden akzeptiert. Die Grafik zeigt nur die eigenen Angaben und berechnet gesuchte Zeiten nicht vor. Gewohnter gemeinsamer Lernrahmen, Hilfen und ein einziges Lernfeedback ohne Punkte; keine zusätzlichen Notiz-/Begründungsfelder. Erkenntnisse und Vorgängerübergänge richten sich nach tatsächlichen Abschlüssen, der Bericht zeigt Lücken. Drei unabhängige Prüfungsfälle ergänzen das Training. Fachvertrag: [Projekte](projekte.md).
