# Abschnitt 07 – Projekte und Wirtschaftlichkeit

Die Quellfassung **1.0.0-beta.1** enthält in Abschnitt 07 **54 Lernaufgaben in neun Kapiteln** und drei Prüfungsfälle. Der Abschnitt ist seit Alpha 16 im veröffentlichten Windows-Paket enthalten.

## Lernweg und Arbeitsformen

Mira König plant mit dem Kulturzentrum Nordhafen einen neuen Online-Kursbereich. Jeder Auftrag enthält alle nötigen Annahmen und ist einzeln bearbeitbar. Ein Übergang zum Vorgänger erscheint nur bei dessen tatsächlichem Abschluss. Neue Erkenntnisse stehen erst nach erfolgreicher Prüfung im Projektbericht. Dieser zeigt bestätigte Schritte bis zur aktuellen Aufgabe und verlinkt offene Zwischenstücke.

| Aufgaben | Kapitel | Eigene Arbeit |
| --- | --- | --- |
| 1–6 | Projektstart und Ziele | Projekt/Routine unterscheiden, Ziele, Umfang, Stakeholder und Verantwortung |
| 7–12 | Vorgehen und Zusammenarbeit | Klassisch/agil, vereinfachtes V-Modell, Scrum, Kanban/WIP |
| 13–18 | Projektstruktur und Aufwand | Drei Strukturpläne aufbauen, ergänzen oder reparieren; Aufwand, Dauer, Dreipunktschätzung und Kapazität |
| 19–24 | Netzpläne und Abhängigkeiten | Eigene Vorgängerbeziehungen, Vorwärts-/Rückwärtsrechnung, Gesamt-/freier Puffer, mehrere kritische Pfade |
| 25–30 | Gantt-Pläne und Ressourcen | Eigene Starts, Endtermin, alternative Reihenfolgen, Kapazitäten und nulltägiger Meilenstein |
| 31–36 | Kosten und Beschaffung | Personalkosten, Einmal-/Folgekosten, TCO, Angebotsvergleich, Make-or-Buy |
| 37–42 | Wirtschaftlichkeit berechnen | Nettoeinsparung, statische Amortisation, kumulierte Zahlungen, Break-even, Kapitalwert, Sensitivität |
| 43–48 | Alternativen bewerten | Eigene Nutzwertmatrix, Muss-Anforderungen, Gewichtungswechsel, getrennte Euro-/Punktebetrachtung und Gleichstand |
| 49–54 | Risiken und Projektsteuerung | Ursachen/Maßnahmen, ausdrückliche Risikopriorität, Soll-Ist/Prognose, Änderungen, Abnahme, Übergabe und Abschluss |

Sechs Arbeitsformen: drei Auswahl-, fünf Review-, 31 strukturierte Tabellen-/Feldaufträge und 15 Planaufträge (drei Projektstruktur-, sechs Netz- und sechs Terminpläne). Vier Aufgaben besitzen eine unveränderliche fehlerhafte Reparaturvorlage. Leere Bauaufträge verlangen eigene Elemente und Verbindungen. Die Beispiele sind eigene Inhalte; keine übernommenen Originalprüfungen.

## Bedienung

- Gewohnte linke `TaskRail`, gemeinsame `LearningModePicker`, Auftrag und Arbeitsbereich in der Mitte, Projektbericht rechts beziehungsweise mobil darunter.
- **Ohne Hilfe lösen** als Standard. Geführtes Lernen behält den Entwurf und öffnet drei Hinweise, Spickzettel oder Musterlösung. Ein Wechsel zurück verlangt bei eigener Bearbeitung die gemeinsame Neustartbestätigung. Erfolge bleiben erhalten.
- Tabellen verwenden ausdrücklich benannte Rechenzellen und Einheiten. Dezimalkomma oder Punkt, keine Tausendertrennzeichen/Einheiten im Zahlenfeld. Eigene Eingaben werden bei jeder Änderung gespeichert.
- Pläne werden mit Hinzufügen/Entfernen, Fachrollen, eigenen Anzeigenamen, Obergruppen oder direkten Vorgängern aufgebaut. Kein Ziehen erforderlich. Löschen eines Elements entfernt seine Verbindungen und löst verwaiste Obergruppen.
- Eigene SVG-Vorschau mit Vergrößerung, seitlicher Tastatur-/Scrollbedienung und vollständiger Textalternative. Die Vorschau zeichnet **ausschließlich eigene Angaben**; keine automatische Ergänzung gesuchter Netzplanwerte oder kritischer Markierungen. Auch unvollständige und zyklische Zwischenstände bleiben bearbeitbar.
- Genau eine Lernrückmeldung ohne Punkte, Prozent oder IHK-Zusatz. Offene Kriterien nennen betroffene Zellen/Planbedingungen, ohne ihre gesuchten Zahlen vorwegzunehmen. Fachliche Erklärung und Muster erst auf ausdrücklichen Hilfenabruf. Keine zusätzlichen Begründungs-, Notiz- oder Reflexionsfelder.

## Fachlicher Planvertrag

Fachrollen sind stabil; Anzeigenamen, Elementreihenfolge und Zeichenpositionen sind nicht bewertet. Jede verlangte Rolle muss genau einmal vorkommen. Beim Projektstrukturplan gelten die ausdrücklich angegebenen direkten Obergruppen; das ist eine Gliederung, kein Ablaufplan.

Netz- und Terminpläne verwenden ganze Arbeitstage ab Tag 0 und direkte Ende-Anfang-Beziehungen ohne Zeitabstand. Die vollständige direkte Vorgängerliste ist Bestandteil des Auftrags: zusätzliche transitive, doppelte, umgekehrte und Selbstverbindungen sind keine zulässigen Ersatzpfeile. Ein Vorgang von Tag 2 mit Dauer 3 belegt **[2, 5)**. Tag 5 ist wieder verfügbar. Keine stillschweigenden Kalender-/Feiertagsannahmen, keine Ressourcenrechnung im CPM-Netzplan.

Vorwärts: FAZ = größtes Vorgänger-FEZ (Startvorgang 0), FEZ = FAZ + Dauer. Frühestes Projektende = größtes FEZ der Endvorgänge. Rückwärts: SEZ = kleinstes Nachfolger-SAZ; alle Endvorgänge erhalten dasselbe früheste Projektende. SAZ = SEZ − Dauer. Gesamtpuffer = SAZ − FAZ. Freier Puffer = kleinstes Nachfolger-FAZ − FEZ; für Endvorgänge gilt das gemeinsame virtuelle Projektende. Alle Vorgänge mit Gesamtpuffer 0 sind kritisch, auch auf mehreren gleich langen kritischen Pfaden. In der Grafik stehen GP/FP für Gesamt-/freien Puffer.

Terminbewertung prüft tatsächliche Starts, Dauern, Vorgängerenden, explizite Zeitachse/Endtermine und Ressourcenbelegung. **Jeder Plan innerhalb dieser Bedingungen wird akzeptiert.** Die Musterlösung ist ein Beispiel, kein verbindlicher Startzeitenvergleich. Nur ausdrücklich früheste Terminierung verlangt die CPM-Startwerte. Ressourcen werden als feste gleichzeitige Kapazität je genanntem Tag betrachtet; nulltägige Meilensteine belegen keine Einheit. Keine Optimierung beliebiger Ressourcen-/Kalenderprobleme.

Speichergrenzen: 16 Elemente, 32 Verbindungen, 64 Antwortfelder, sechs Zeitfelder je Element, Zeitachse höchstens 60 Tage. Eigene Anzeigenamen höchstens 80 Zeichen; numerische Planfelder zwölf, allgemeine Rechenfelder 500 Zeichen. Speicherregeln lassen fachlich falsche/unfertige Antworten zu; Bewertung weist solche Entwürfe mit Rückmeldung zurück. Katalog und DAG-Rechnung prüfen eigene Fachverträge zusätzlich beim Laden.

## Zahlen und Entscheidungen

Erwartete Rechenwerte sind eigene festgelegte Sollwerte, zusätzlich mit unabhängigen Beispielen geprüft. Einheit, Zeitraum, Zahlungsverlauf und Rundung stehen im Auftrag. Keine Livepreise, allgemeine Steuer-/Anlageberatung oder heimliche Zinsannahmen.

Nettoeinsparung berücksichtigt zusätzliche laufende Kosten; bei nichtpositivem Wert keine endliche statische Amortisation. Nur am Jahresende zufließende Beträge werden als diskrete Zahlungen kumuliert. Break-even und erster positiver Gewinn sind getrennt. Kapitalwertaufgaben geben Abzinsungsfaktoren ausdrücklich vor. Nutzwertgewichte, Skala, Muss-Kriterien und Gleichstandsregel sind benannt; Euro und dimensionslose Punkte werden nicht addiert. Risiken verwenden die ausdrücklich vorgegebene Prioritätsregel. Istkosten sind kein Beweis für Fertigstellung; die lineare Kostenprognose benennt ihre vereinfachte Annahme.

## Prüfung und Speicherung

Routen: `/projekte`, `/projekte/{Id}`, `/projekte/spickzettel`, `/projekte/pruefung`, `/projekte/pruefung/runde`.

| Eigener Fall | Teil 1 · 20 Punkte | Teil 2 · 20 Punkte | Teil 3 · 10 Punkte |
| --- | --- | --- | --- |
| Stadtbibliothek | Eigenen Netzplan mit frühesten Zeiten und kritischen Vorgängen bauen | Einführungskosten über 24 Monate | Stakeholderinteressen |
| Sportverein | Eigenen Ressourcen-/Terminplan bauen | Nutzwertmatrix und ausdrücklicher Kostenentscheid bei Gleichstand | Änderungswunsch |
| Museum | Kapazität und Fehlbedarf | Nettoeinsparung und Amortisation | Scrum-Besprechungen |

45 Minuten, drei unabhängige Teilaufträge, 50 **Trainingspunkte**. Uhrstart erst nach Aktion. Pause verbirgt Aufgaben, UTC-Budget bleibt nach Neustart erhalten. Nachschlagen pausiert und kennzeichnet Unterstützung. Kontrollmarkierungen und eigene Entwürfe bleiben gespeichert. Auswertung/Muster erst nach endgültiger bestätigter Abgabe oder Fristablauf; Antworten dann gesperrt, Historie genau einmal mit höchstens 20 Einträgen. Kein vollständiger offizieller Prüfungsbereich; keine amtliche IHK-Note oder behauptete Zeitkalibrierung.

Neue additive Profilfelder `ProjectLessons`, `LastProjectLessonId`, `ProjectExam`, `ProjectHistory`. Inhaltsversion **2026-11-11** ausschließlich für dieses Modul. Atomare gemeinsame Dateisperre, Aufgaben-/Rundenrevision, Schutz gegen überholte Seitenwechsel und unveränderliche Abgabekopien. Verspätete Antworten nach Fristablauf ersetzen rechtzeitig gespeicherte Entwürfe nicht. Sämtliche bisherigen Module und historische unsichtbare `Reasoning`-Felder bleiben erhalten; SQL/Pseudocode werden nicht erweitert.

## Zuständigkeiten und Nachweise

`Core/ProjectManagement/{Models,Content,Evaluation,Practice}` enthält Katalog, eigene Antworten/Pläne, CPM-Rechnung und reine Bewertung. `Web/Components/ProjectManagement` zeichnet/bearbeitet eigene Werte. Lern-/Prüfungsseiten unter `Pages/Training`, Speicherung in `ProgressStore.Project.cs`, Gestaltung in `wwwroot/project-management.css`. Keine neue Abhängigkeit, keine externe Berechnung oder Datenübertragung.

Automatisierte unabhängige Netzplan-, Kosten-, Kapazitäts-, Alternativplan-, Grenz-, Profil-, Zeit- und Komponentenprüfungen in `ProjectManagementTests` und `ProjectComponentTests`. Browserprüfung mit eigenem Profil auf Port 5246. Konkrete Ergebnisse und Grenzen: [Testprotokoll](testprotokoll.md). Ausführliche Nutzererprobung und praktische Zeitkalibrierung bleiben offen.

## Fachliche Grundlagen · Recherche 11.11.2026

- [FIAusbV, insbesondere §§ 12–14](https://www.gesetze-im-internet.de/fiausbv/BJNR025000020.html): Projektplanung und Wirtschaftlichkeit im Ausbildungskontext; die schriftlichen Prüfungsbereiche dauern jeweils 90 Minuten, diese Modultrainings 45 Minuten.
- [Offizieller Scrum Guide 2020, deutsche Übersetzung verfügbar](https://scrumguides.org/download.html): Verantwortungen, Events und Timeboxen.
- [PMI: Critical Path Method Calculations](https://www.pmi.org/learning/library/critical-path-method-calculations-scheduling-8040): Vorwärts-/Rückwärtsrechnung, Puffer und kritische Pfade.
- [BMF: Arbeitsanleitung Wirtschaftlichkeitsuntersuchungen vom 13.01.2026](https://www.verwaltungsvorschriften-im-internet.de/bsvwvbund_13012026_IIA3H100500150006005DOKCOO7005100213785493.htm): wirtschaftliche Vergleiche, Kapitalwert und getrennte nichtmonetäre Nutzwertbetrachtung. Die App verwendet vereinfachte, ausdrücklich vorgegebene Zahlen.
- [IHK Hannover: neue IT-Prüfungskataloge ab 2025](https://www.ihk.de/hannover/hauptnavigation/ausbildung-und-weiterbildung/ausbildung/ausbildung-a-z/neuordnungen/pruefungskataloge-it-berufe-6438900): Orientierung zum Katalogstand, kein Nachweis vollständiger Katalogabdeckung.

Die bereits privat gesichteten alten Teil-1-/Teil-2-Prüfungen dienen ausschließlich der Themenorientierung. Sie werden weder kopiert noch ins Programm oder Release aufgenommen. Teil 3 bleibt unberührt.

## Darstellungsnachlauf 11.11.2026

Auswahl- und Reviewaussagen besitzen eigene umrandete, vollständig anklickbare Zeilen mit Abstand, nicht schrumpfender 20-Pixel-Checkbox, Textumbruch und sichtbarem Tastaturfokus. Diese Regeln stehen ausdrücklich unter project-options in project-management.css; die Markupklasse design-option allein liefert keine globale Zeilengestaltung. Prüfung im dunklen Desktop und in der mobilen Hellansicht auf eigenem Port 5248. Reine Darstellungskorrektur ohne Inhalts-/Profilrevision, geänderte Antworten oder Bewertung. Einzelheiten im Testprotokoll.

## Paketnachweis Alpha 16

Windows-ZIP am 11.11.2026 nach Nutzer-Push aus 1dbcb12 mit unverändertem Anwendungscode gebaut. Native EXE direkt aus dem Archiv mit getrenntem Altprofil auf 5250 geprüft: eigener Strukturplan, alternative Ressourcenfolge, Netzplan, Auswahlkarten und Museumsprüfung 50/50. Pause/Neustart bei 2684 Sekunden, Markierung, Antwortsperre und alle Altprofilfelder erhalten. 1.190 .NET-/31 JavaScript-Prüfungen erfolgreich,20 SQL-Integrationen ausgelassen. 62 Dateien nach Lauf bitgleich; keine privaten/QA-/Originalprüfungsdateien, Lizenzen vollständig. Nachweis artifacts/alpha16-verification.json. Tag v0.1.1-alpha.16 auf 1dbcb12, Veröffentlichung durch Nutzer. ZIP/SHA256 und ältere Pakete unverändert erhalten.
