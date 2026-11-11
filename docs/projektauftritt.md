# Projektauftritt und freiwillige Unterstützung

Stand: **11.11.2026 · Quellfassung 0.1.1-alpha.14**. Die fünf freigegebenen Projektaktionen sind unverändert umgesetzt und seit Alpha 8 in Windows-Paketen enthalten. Die README-Galerie bildet alle sechs aktuellen Lernbereiche ab; kein neues Zahlungs-, GitHub- oder Veröffentlichungsverhalten.

## Übersicht und Fußzeile

Unterhalb der Lernbereiche und der Lernroutine steht eine dezente Unterstützungskarte. Sie benennt den Projektinhaber und bietet einen GitHub-Stern sowie freiwillige Unterstützung per PayPal an. Ein eigener Link führt zu GitHub Issues für Fehler oder Vorschläge. Die Karte unterbricht keine Aufgabe und verändert die Themenreihenfolge nicht; WiSo bleibt an Position 08.

Die Fußzeile ergänzt **Über das Lernwerk**, **Fehler oder Vorschlag melden** und **Unterstützen**. Version und SQL-Einrichtung bleiben erreichbar. Die Projektseite öffnet innerhalb der App, externe Ziele öffnen mit `noopener noreferrer` in einem neuen Tab. Der Lernraum bleibt dadurch im bisherigen Tab bestehen.

Ein GitHub-Stern ist eine freiwillige Anerkennung über „Star“ am Repository, keine Bewertung von einem bis fünf Sternen. Die App fragt den Sternstatus nicht ab und setzt keinen Stern automatisch. Fehlermeldungen und Vorschläge werden auf GitHub angelegt; die App überträgt keine Lernstände oder Anhänge dorthin. Eine kurze Erklärung nennt die hilfreichen Angaben App-Version, Aufgabe und nachvollziehbare Schritte. Auf der Projektseite steht auch, dass GitHub-Meldungen öffentlich sind.

## Urheberseite

Die Route `/ueber-das-lernwerk` nennt **Christopher Weismantel · b0rnkilla** für Konzept und Projektentwicklung. Der Text beschreibt die Entstehung aus der eigenen Prüfungsvorbereitung. Seit Alpha 9 zeigt sie das vom Nutzer gelieferte b0rnkilla-Gaming-Logo. Verwendet wird die unveränderte kompakte PNG-Datei mit 400 × 175 Pixeln (61.359 Bytes) unter `wwwroot/images/project/b0rnkilla-gaming.png`. Eine dunkle Fläche hält die transparenten Konturen in beiden Farbmodi lesbar; das Bild skaliert proportional und besitzt einen Alternativtext. Die HD-Fassung mit 1600 × 900 Pixeln wird nicht zusätzlich mitgeliefert. Es werden keine unbekannten Kontakt- oder Adressangaben ergänzt und keine registrierten Markenzeichen behauptet.

Ein eigener Bereich erklärt Rückmeldungen, die Unterstützungskarte bietet GitHub und PayPal an. Am Ende stehen die Einordnung als unabhängiges Lernprojekt, die privaten Nutzungsbedingungen und der Verweis auf die mit Windows-Paketen gelieferten Drittanbieterhinweise. Die Projektseite ist eine Information zur Urheberschaft; aus ihrer Gestaltung wird keine Aussage über eine rechtliche Impressumspflicht abgeleitet. Die Projektlizenz wird durch Unterstützung oder Feedback nicht geändert.

## Technischer Einstieg

| Datei | Aufgabe |
| --- | --- |
| `src/FiaeTrainer.Web/Project/ProjectInformation.cs` | Gemeinsame öffentliche Namen und Linkziele |
| `src/FiaeTrainer.Web/Components/Project/ProjectSupport.razor` | Unterstützungskarte auf Übersicht und Projektseite |
| `src/FiaeTrainer.Web/Components/Pages/About.razor` | Urheberseite, Hinweise zu Rückmeldungen und Nutzung |
| `src/FiaeTrainer.Web/Components/Layout/MainLayout.razor` | Dauerhaft erreichbare Projektlinks |
| `src/FiaeTrainer.Web/wwwroot/app.css` | Projektbereich und responsive Fußzeile in beiden Designs |

Der freigegebene öffentliche PayPal-Link lautet **https://www.paypal.com/paypalme/b0rnkillafpv**. Er ist eine öffentliche Projektangabe und wird bewusst mit dem Programm ausgeliefert. Zahlungsdaten, Zugangsschlüssel und private PayPal-Kontoeinstellungen werden nicht eingebunden. Die Zahlung erfolgt ausschließlich auf PayPal; es gibt kein Zahlungsformular oder SDK in der App und keinen vorgegebenen Betrag.

`ProjectInformation` hält diese Linkziele zentral für die App. Wenn `PAYPAL_URL` leer gesetzt wird, entfallen Zahlungsaktionen statt eines leeren Links. Die README enthält zusätzlich eigene öffentliche Verweise; bei einer späteren Adressänderung diese mitpflegen. Der Issues-Link führt zunächst zur Meldungsübersicht, damit vorhandene Rückmeldungen sichtbar bleiben. Es wurden keine Issue-Vorlagen oder automatischen Meldungen ergänzt.

`ProjectSupport` wird auf zwei Seiten verwendet. Auf der Urheberseite verhindern `ShowAuthor=false` und `ShowFeedback=false` den Rücklink auf dieselbe Seite und einen doppelten Rückmeldungsbereich. Es gibt keine Änderung an Lernmodi, Antworten, Profilformat oder SQL-Datenbank.

## README und Bilder

Die README führt mit Nutzen, direktem Release-Link, einem großen Übersichtsbild und einer kompakten Galerie durch die sechs aktiven Module. Zwei Reihen mit je drei Spalten zeigen SQL, Algorithmen und UML sowie Datenbankentwurf, Softwareplanung/OOP und WiSo. Jedes Modul besitzt dieselben Zeilen für Kurzbeschreibung, Umfang und Vorschau. Sämtliche Vorschaubilder werden 120 Pixel hoch angezeigt und verlinken ihre vorhandene große Ansicht; keine vergrößerten Einzelblöcke für UML/Softwareplanung. Danach folgen drei Startschritte und die Projektaktionen. Entwicklerdetails bleiben in den Fachdokumenten. Der Anwenderweg bleibt Releases → Windows-ZIP → Entpacken → EXE.

Unter `docs/images` liegen `uebersicht.png`, `sql.png`, `algorithmen.png`, `uml.png`, `datenbankentwurf.png`, `softwareplanung.png` und `wiso.png`. Die vorhandenen Bilder aus synthetischen Profilen bleiben unverändert; nur ihre Darstellung in der README wurde angepasst. Keine Originalprüfungsabbildungen oder Nutzerlernstände. Die README beschreibt auf Nutzerwunsch die aktuelle App-Fassung mit 346 Lernübungen; tatsächlicher Paketstand und Historie bleiben in [lokale Releases](lokale-releases.md) nachvollziehbar. Das zuletzt geprüfte Alpha-13-ZIP bleibt unverändert; Alpha 14 folgt auf ausdrücklichen Auftrag erst nach Nutzer-Commit und Push.

Die Link- und Darstellungsprüfung ist im [Testprotokoll](testprotokoll.md) festgehalten. Grundlage für die GitHub-Sternfunktion: [GitHub-Dokumentation](https://docs.github.com/en/get-started/exploring-projects-on-github/saving-repositories-with-stars).
