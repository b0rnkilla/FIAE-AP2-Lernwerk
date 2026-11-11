# Projektauftritt und freiwillige Unterstützung

Stand: **11.11.2026 · Quellfassung 0.1.1-alpha.16**. Die fünf freigegebenen Projektaktionen sind unverändert umgesetzt und seit Alpha 8 in Windows-Paketen enthalten. Die README-Galerie bildet alle acht aktuellen Lernbereiche ab; kein neues Zahlungs-, GitHub- oder Veröffentlichungsverhalten.

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

Die README führt mit Nutzen, direktem Release-Link, einem großen Übersichtsbild und einer kompakten Galerie durch die acht aktiven Module. Vier gleich breite Plätze je Tabellenreihe zeigen zuerst SQL, Algorithmen, UML und Datenbankentwurf, dann Softwareplanung/OOP, Qualitätssicherung, Projekte/Wirtschaftlichkeit und WiSo. Beschreibungen, Umfang und Bilder besitzen eigene Zeilen. Alle acht Vorschaubilder sind 120 Pixel hoch und zur großen Ansicht verlinkt; keine großen Einzelblöcke. Danach folgen drei Startschritte und Projektaktionen. Anwenderweg bleibt Releases → Windows-ZIP → Entpacken → EXE; Entwicklerdetails stehen in Fachdokumenten.

Unter `docs/images` liegen die Übersicht sowie eigene Modulbilder einschließlich `qualitaet.png` und `projekte.png`. Bilder entstehen mit getrennten synthetischen Profilen, ohne Originalprüfungsabbildungen oder Nutzerlernstände. Die README beschreibt auf Nutzerwunsch die aktuelle App mit 454 Übungen. Das vom Nutzer veröffentlichte Alpha-15-ZIP bleibt unverändert bei 400; Paketstand und Historie stehen unter [lokale Releases](lokale-releases.md). Für Alpha 16 ist ausdrücklich kein neues ZIP beauftragt.

Die Link- und Darstellungsprüfung ist im [Testprotokoll](testprotokoll.md) festgehalten. Grundlage für die GitHub-Sternfunktion: [GitHub-Dokumentation](https://docs.github.com/en/get-started/exploring-projects-on-github/saving-repositories-with-stars).

## README mit acht Modulbildern seit Alpha 16

Die kurze öffentliche README beschreibt die aktuelle App-Fassung mit 454 Übungen und allen acht aktiven Themen. Zwei Tabellen mit jeweils vier gleich großen Vorschaubildern (120 Pixel Höhe), Reihenfolge 01–04 und 05–08. Das eigene Netzplanbild `docs/images/projekte.png` und die erneuerte Übersicht stammen aus dem getrennten synthetischen Profil auf 5246, ohne private Lernstände. Aktueller Paketstand bleibt Alpha 15 mit 400 Übungen; kein Alpha-16-ZIP beauftragt.
