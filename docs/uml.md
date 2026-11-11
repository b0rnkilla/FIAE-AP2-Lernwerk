# UML und Modellierung – Abschnitt 03

Stand: **0.1.1-alpha.13, 09.11.2026**. Nach Vorstellung und Rückfrage zu eigenen Diagrammen ausdrücklich zur Umsetzung freigegeben. Der Lernweg ist im Quellprojekt und im geprüften Alpha-13-ZIP umgesetzt. Die neuere Quellfassung Alpha 14 ergänzt Abschnitt 05 Softwareplanung bei unverändertem UML-Katalog; bestehende Pakete werden dadurch nicht verändert.

## Lernweg und Geschichte

36 eigene Aufgaben begleiten Entwicklerin Mira König im Kulturzentrum Nordhafen. Aus Nutzerzielen entstehen Klassen, Bearbeitungsabläufe, Nachrichten und Lebenszyklen. Vorgänger werden nur nach ihrem gespeicherten Abschluss als bestätigt beschrieben. Andernfalls führt ein Link zum offenen Schritt; jede Aufgabe bleibt unabhängig bearbeitbar. Der Bericht enthält ausschließlich bestätigte Erkenntnisse bis zur aktuellen Aufgabe und kennzeichnet Lücken.

| Aufgaben | Schwerpunkt | Umfang |
| --- | --- | --- |
| 1–4 | Diagrammsprache verstehen | Vier Leseaufgaben zu Funktion, Klasse/Objekt, Kontrollfluss und Ereignis |
| 5–10 | Anwendungsfälle | Sechs Aufgaben: Akteure, beschriftete Systemgrenze, Assoziation, include, extend, Generalisierung |
| 11–18 | Klassen | Acht Aufgaben: Attribute/Methoden, Sichtbarkeit, Parameter und Rückgabe, Multiplizitäten, Aggregation/Komposition, abstrakte Klasse, Interface und Realisierung |
| 19–24 | Aktivitäten | Sechs Aufgaben: Start/Ende, Aktionen, Entscheidung/Merge, Fork/Join und Verantwortungsbereiche |
| 25–30 | Sequenzen | Sechs Aufgaben: Lebenslinien, synchrone Aufrufe/Rückgaben, Reihenfolge sowie alt/loop/opt |
| 31–36 | Zustände | Sechs Aufgaben: Zustände, Ereignisse, Bedingungen, entry/do/exit, Stornierung und Finalknoten |

Sieben Lese-, sechs Reparatur- und 23 eigene Modellierungsaufgaben. Modellierungsaufgaben beginnen mit einer leeren Arbeitsfläche. Reparaturen verwenden eine eigene Kopie der unveränderlichen Vergleichsvorlage. Jede Aufgabe besitzt drei freiwillige Hinweise, Fachklärung, Musterlösung und eine eigene Erkenntnis. Der Spickzettel erklärt alle fünf Diagrammarten und führt zur aktuellen Aufgabe zurück.

## Eigene Diagramme bedienen

Die gemeinsame TaskRail, LearningModePicker und der bestehende Lernrahmen bleiben erhalten: Aufgaben links, Geschichte/Auftrag/Baukasten/Hilfen mittig, bestätigte Planungsschritte rechts beziehungsweise mobil darunter. Neue Versuche beginnen ohne Hilfe. Ein Wechsel zum geführten Lernen erhält den Entwurf und kennzeichnet Unterstützung. Ein frischer Selbsttest verlangt bei tatsächlich geändertem Entwurf die gemeinsame Neustartbestätigung. Frühere Erfolge bleiben erhalten.

Elemente lassen sich hinzufügen, entfernen, einer Fachrolle zuordnen und frei beschriften. Die Elementart wird ausdrücklich ausgewählt. Klassen erhalten Attribute, Methoden, Parameter, Datentypen, Sichtbarkeit und statische/abstrakte Merkmale. Zustände erhalten entry-/do-/exit-Aktionen. Verbindungen besitzen gewählte Endpunkte, passende UML-Arten und gegebenenfalls Mengenangaben, Ereignisse oder Bedingungen. Sequenznachrichten werden mit Früher/Später zeitlich geordnet und einfachen alt-/opt-/loop-Gruppen zugeordnet.

Die automatische SVG-Anordnung verlangt kein Ziehen. Klasseneigenschaften und Operationen stehen in getrennten Fächern. Synchrone Aufrufe haben eine gefüllte Pfeilspitze, Rückgaben eine gestrichelte Linie mit offener Spitze. Aktivierungen werden aus den Aufruf-/Rückgabepaaren abgeleitet; sie sind keine zusätzlichen Antwortfelder. Aktivitätsenden stehen bei verschieden langen Zweigen hinter allen Vorgängern. Vergrößerung und eine ausgeschriebene Textdarstellung ergänzen das Diagramm. Auf schmalen Bildschirmen scrollt die Zeichenfläche intern; Eingabefelder umbrechen.

## Fachliche Bewertung

`Core/Uml` trennt Models, Content, Evaluation und Practice. Eigene JSON-Ressourcen `lessons.json` und `exams.json` enthalten keine Originalprüfungen. `UmlCatalog` prüft Vollständigkeit, alle Modellreferenzen und Prüfungsgewichte beim Laden. `UmlRules` trennt speicherbare unvollständige Entwürfe von fachlich vollständigen Antworten. `UmlEvaluator` bewertet folgende Merkmale:

- Fachrollen, Elementarten, abstrakte Klassen, Klassenmerkmale, Parameteranzahl/-typen, Sichtbarkeit sowie Zustandsaktionen und Verantwortungsbereiche.
- Beziehungen mit ihren fachlichen Endpunkten, Richtungen, Multiplizitäten, Nachrichten-/Ereignisrollen, Bedingungen und Sequenzgruppen. Bei Sequenzen zählt die Nachrichtenreihenfolge.
- Beschriftete Systemgrenze sowie Arten/Bedingungen der benötigten Fragmente.

Anzeigenamen, interne Elementkennungen, Parameternamen und Zeichenkoordinaten sind frei. Die Erstellreihenfolge von Elementen/Merkmalen und die Reihenfolge nicht zeitlicher Beziehungen werden nicht bewertet. Ungerichtete Assoziationen dürfen mit vertauschten Enden und entsprechend mitgetauschten Multiplizitäten entstehen. Aggregation und Komposition zeigen im Baukasten vom Teil zum Ganzen; die Raute liegt am Ziel. Generalisierung zeigt zur allgemeineren Klasse/Rolle, Realisierung zum Interface, include zur eingebundenen Funktion und extend zur Basis.

Integer/Ganzzahl/int, Boolean/Wahrheitswert/bool, Text/Zeichenkette/String und Void/Nichts werden vereinheitlicht. Double und Decimal bleiben verschiedene Typen. Mengenangaben erlauben 1, 0..1, 0..*, 1..*, 2..* sowie die Schreibvarianten 1..1 und n. Ein Interface-Methodenvertrag wird auch mit zusätzlicher abstrakt-Markierung akzeptiert. Aufgabe 20 erlaubt ausdrücklich sowohl direkte Endpfade als auch einen vorgeschalteten Merge; andere Alternativen werden nur akzeptiert, wenn sie als geprüfte Variante hinterlegt sind.

Lernen zeigt **eine Ergebnis-/Erkenntniskarte ohne Punkte, Prozent oder IHK-Zusatz**. Falsche Antworten zeigen konkrete Kriterien und gegebenenfalls Markierungen, ohne die fachliche Mustererklärung aufzudecken. Änderungen an Antwort oder Lernmodus entfernen alte Rückmeldungen. Leere Antworten können nicht geprüft werden. Doppelklicks und überholte Ladeanfragen dürfen keine zusätzlichen Versuche oder falsch zugeordneten Ergebnisse erzeugen.

## Eigene Prüfungen

`/uml/pruefung` liest den Stand, ohne eine Uhr zu starten. Drei unbekannte, eigene Fälle besitzen jeweils drei voneinander unabhängig lösbare Modellaufträge:

| Fallkennung | Fall | Diagrammarten |
| --- | --- | --- |
| proberaum | Proberäume im Vereinszentrum | Anwendungsfälle, Klassen, Aktivitäten |
| ausgabe | Mobile Geräte für die Veranstaltung | Klassen, Sequenzen, Zustände |
| abend | Einlass zur Abendveranstaltung | Anwendungsfälle, Aktivitäten, Sequenzen |

45 Minuten, Gewichtung **20/20/10**, zusammen 50 eigene Trainingspunkte. Die interne Modellprüfung verwendet 20 Anteile für Elemente, 25 für Beziehungen und fünf für Rahmung; die Teilaufgaben werden anschließend ganzzahlig auf ihr Rundengewicht skaliert. Es handelt sich nicht um eine amtliche IHK-Note.

Start erst nach Aktion; offene Runde nur nach zusätzlicher Bestätigung ersetzen. UTC-Frist läuft nach Schließen des Browsers weiter. Pause blendet Aufgaben und Entwürfe aus; Nachschlagen pausiert und kennzeichnet die Hilfenutzung. Kontrollmarkierungen und letzte Teilaufgabe bleiben gespeichert. Abgabebestätigung und Auswertung erhalten sichtbaren Fokus. Endgültige Abgabe friert alle Antworten ein, zeigt Kriterien/Musterlösungen und zählt genau einmal in die Historie. Bei Fristablauf werden nur die zuletzt rechtzeitig gespeicherten Antworten bewertet. Veraltete Fenster dürfen die Runde nicht überschreiben.

## Profil und Kompatibilität

Additive Felder `UmlLessons`, `LastUmlLessonId`, `UmlExam` und `UmlHistory` liegen neben allen bisherigen Profilfeldern. Inhaltsvertrag **2026-11-09**, stabile Lern-IDs 1–36 und stabile Fall-/Options-/Fachrollenkennungen. Fehlende UML-Felder älterer Profile erhalten leere Vorgaben. Keine Umnummerierung oder Migration anderer Module.

`ProgressStore.Uml.cs` benutzt die gemeinsame atomare Dateisperre. Lernversuche und Prüfungsrunden besitzen eigene Revisionen. Erfolge/Versuchszahlen werden aus dem gespeicherten Stand übernommen; vom Editor übergebene Erfolgsflags gelten nicht als Nachweis. Eine einmalige Unterstützung bleibt bis zum frischen Versuch erhalten. Prüfungen speichern Kopien; während eines Schreibens eintreffende Eingaben bleiben im Arbeitsentwurf und werden anschließend mit der aktuellen Revision gespeichert. Historie auf 20 Ergebnisse begrenzt.

Alle Schreibhandler beachten ReadOnly und Grenzen: 16 Elemente, 32 Verbindungen, vier Sequenzgruppen, 16 Merkmale je Element; Texte höchstens 80, Parameter 250 und Mengenangaben zwölf Zeichen. Entfernen eines Elements entfernt seine Verbindungen, Entfernen einer Gruppe löst deren Nachrichtenzuordnungen. Unvollständige Fachzuordnungen bleiben als Entwurf speicherbar.

## Grenzen und Quellen

Ein strukturierter Lernbaukasten mit festem Aufgabenwortschatz; kein allgemeiner UML-Editor, kein XMI-Import/-Export und keine Interpretation beliebiger Freitexte durch KI. Keine verschachtelten Fragmente, asynchronen Signale, Objektfluss-/Petri-Netz-Simulation, hierarchischen Zustände oder vollständige OCL-Prüfung. Ein passender Entwurf entspricht einer ausdrücklich geprüften Variante; die App beweist keine beliebige UML-Äquivalenz. BPMN, PAP/Struktogramme und Abschnitt 05 gehören nicht zu diesem Auftrag. Die 45 Minuten sind noch durch den Nutzer praktisch zu kalibrieren.

Fachliche Orientierung: [OMG UML 2.5.1](https://www.omg.org/spec/UML/2.5.1) und [IHK Hannover: überarbeitete Prüfungskataloge](https://www.ihk.de/hannover/hauptnavigation/ausbildung-und-weiterbildung/ausbildung/ausbildung-a-z/neuordnungen/pruefungskataloge-it-berufe-6438900). Die lokal bereitgestellten Altprüfungen wurden nur zur Themen-/Formorientierung genutzt; keine Originalseiten, Texte oder Lösungsschlüssel werden eingebettet oder verteilt. Eine vollständige Abdeckung des AP2-Katalogs wird nicht behauptet.

Prüfnachweise: [Testprotokoll](testprotokoll.md). Nutzererprobung nach Übergabe ausstehend. Alpha-13-ZIP am 09.11.2026 auf ausdrücklichen Auftrag aus b4224b0 gebaut und aus der nativen EXE geprüft; siehe [lokale Releases](lokale-releases.md). Kein automatischer Commit/Push/Upload.
