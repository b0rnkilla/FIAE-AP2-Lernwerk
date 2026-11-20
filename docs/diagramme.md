# Direkte Diagrammbearbeitung ab Beta 6

UML und Datenbankentwurf verwenden denselben lokalen Arbeitsbereich. Der Auftrag bleibt am Desktop neben oder über der Zeichenfläche sichtbar. Werkzeuge bieten allgemeine Symbolarten; Eigenschaften beziehen sich auf die aktuelle Auswahl. Geschichte und bestätigte Planungsschritte können eingeklappt bleiben. Im geführten Lernen steht vor den Hinweisen ein konkretes Beispiel aus einem anderen Zusammenhang. Vollständige Herleitung und Referenz bleiben hinter **Musterlösung**.

## Bedienung

- Element/Tabelle hinzufügen, anklicken und frei beschriften. Doppelklick oder F2 bearbeitet den Namen direkt. Keine Auswahl gesuchter Fachrollen aus Aufgaben-Dropdowns.
- Elemente verschieben; bei Sequenzen Lebenslinien waagerecht, Nachrichten senkrecht. Früher/Später bleibt als Alternative erhalten.
- Anschluss zum Ziel ziehen oder **Verbinden**, Ausgang und Ziel anklicken. Selbstverbindungen sind möglich. Richtung, Beziehungstyp, Bedingungen, FK-Paare und Mengen bestimmt der Lernende.
- Leere Fläche ziehen verschiebt die Ansicht. Zoom und Einpassen ändern keine Daten; **Anordnen** setzt freie Positionen zurück. Rückgängig/Wiederholen enthält bis zu 40 Stände innerhalb der geöffneten Aufgabe, einschließlich Positionen. Beim erneuten Öffnen bleibt der letzte gespeicherte Entwurf, nicht der vorübergehende Verlauf.
- Tastatur: Tab/Enter zur Auswahl, F2 zum Namen, Pfeiltasten zum Verschieben, Entf zum Löschen, Strg+Z/Strg+Umschalt+Z zum Verlauf bei Fokus auf der Zeichenfläche. Eigenschaften und Endpunkte sind auch über beschriftete Felder erreichbar.
- ER-Ergänzungen bleiben feste Vorlagen: Lücke anklicken und ihren Text frei eintragen. PK/FK und Tabellen gehören weiterhin zur relationalen Darstellung.

Die Darstellung verwendet eigene SVG-Komponenten und ein gemeinsames JavaScript-Modul ohne externe Dienste oder zusätzliche Diagrammbibliothek. Die untersuchte Bibliothek konnte in dieser Umgebung nicht über NuGet bezogen werden. Das ist kein Lizenzwechsel.

## Fachliche Zuordnung und Kompatibilität

`DiagramNames` normalisiert ausschließlich Großschreibung, Leerzeichen und deutsche Umlautumschriften. `NameAliases` enthält ausdrücklich erlaubte Varianten. Methoden dürfen ein leeres Klammerpaar im Namen tragen. Andere unbekannte Namen oder fachlich verschiedene Bedingungen werden nicht erraten. Operatoren bleiben unterscheidbar. Eindeutige unbeschriftete Start-/End-/Parallelisierungs-/Synchronisations-/Zusammenführungsknoten erhalten ihre interne Zuordnung nach ihrer Strukturart; Verbindungen und Eigenschaften werden unverändert geprüft.

Bestehende Fachkennungen bleiben gültig, sodass alte frei gewählte Anzeigenamen ihre Bedeutung behalten. Wird ein Name erneut eingegeben, entfällt seine alte Zuordnung und die neue Eingabe wird geprüft. Die Bewertung arbeitet auf einer Kopie und überschreibt keine Eingaben. Freie Tabellen- und Parameternamen bleiben weiterhin möglich. Die App prüft bekannte Modellvarianten, keine beliebige sprachliche oder mathematische Äquivalenz.

`UmlModel.Positions` und `DesignModel.Positions` sind additive Wörterbücher aus Elementkennung und Ganzzahlkoordinaten 0–8000. Sie sind in Kopien/Profilen enthalten und von der Benotung ausgeschlossen. Entfernte Elemente verlieren ihre Position. Bestehende Aufgaben- und Inhaltskennungen, Lernabschlüsse und gespeicherte Prüfungen bleiben erhalten. Autospeichern offener UML-/Entwurfsprüfungen übernimmt Revisions-/Zeitmetadaten, erhält aber die sichtbaren Antwortobjekte. Abgabe und Fristablauf verwenden weiterhin gesperrte gespeicherte Antworten.

## Darstellung

Relationale Tabellen werden anhand ihrer Referenzen in Ebenen angeordnet. Im Auftrag-/Artikel-/Positionsfall liegen die beiden Stammtabellen über der Zuordnung. Linien sind rechtwinklig, Mengen stehen außerhalb der Tabellen an beiden Enden. Die vollständigen Paare zusammengesetzter Fremdschlüssel bleiben in den Eigenschaften und der Textalternative erhalten. Auch optionale Zuordnungen und Selbstreferenzen sind darstellbar. Eine freie manuelle Anordnung kann weiterhin Kreuzungen verursachen; ein allgemeines hindernisfreies Routing beliebiger Graphen wird nicht behauptet.

Aktivitätsbalken überspannen die automatisch angeordneten parallelen Zweige; Entscheidung und Zusammenführung bleiben Rauten. In Lerntexten stehen die deutschen Begriffe zuerst, die UML-Bezeichnungen Fork/Join/Merge werden beim Nachschlagen erklärt. Sequenznachrichten behalten ihre fachliche Zeitordnung unabhängig von der waagerechten Anordnung der Beteiligten.

## Abgleich mit den bereitgestellten Prüfungen

Die lokalen Originalunterlagen wurden zur Aufgabenform und zum Anspruch herangezogen, nicht in die App übernommen:

| Unterlage | Orientierung für eigene Inhalte |
| --- | --- |
| Winter 2023/24, Planen eines Softwareproduktes, Aufgabe 2c, S. 6 | Aktivitätsablauf aus fachlichen Bedingungen und paralleler Arbeit herleiten; Symbolfolge nicht als Bauanleitung vorgeben |
| Sommer 2022, Planen eines Softwareproduktes, S. 5 und 7 | Klassen mit Sichtbarkeiten/Typen; Nachrichten, Antworten und Reihenfolge aus einer Interaktion ableiten |
| Winter 2024/25, Planen eines Softwareproduktes, S. 8 | Ereignisse, Zustände und Bedingungen unterscheiden |
| Sommer 2024, Planen eines Softwareproduktes, Aufgabe 4a, S. 9 | ER-Vorlagen mit fachlichen Beziehungen und fehlenden Beschriftungen ergänzen |
| Winter 2024/25, Entwicklung und Umsetzung von Algorithmen, Aufgabe 3a, S. 6 | Relationaler Entwurf mit PK/FK, Kardinalitäten und fachlicher Mindestzahl |
| Winter 2025/26, Entwicklung und Umsetzung von Algorithmen, Aufgabe 1a, S. 2–3 | Typisierte Eingaben und fachliche Auswahlbedingungen als Orientierung für den zusätzlichen eigenen Ausleihfall |

Keine Originaltexte, Abbildungen oder Lösungsschlüssel werden verteilt. Die Unterrichtsunterlagen `Tag4_Pseudocode` wurden in diesem Änderungspaket nicht erneut ausgewertet; die fachliche Orientierung liegt wie beauftragt auf den vorhandenen Prüfungen. Die neuen Texte verändern die bestehenden Modellanforderungen und Gewichte nicht. Der Pseudocode-Fall ergänzt neue IDs, ohne den bisherigen Zufallspool zu ersetzen.

Nachweise und verbleibende Grenzen der Testumgebung: [Testprotokoll](testprotokoll.md).
