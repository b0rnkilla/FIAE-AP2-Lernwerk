# Projektauftritt und freiwillige Unterstützung

Stand: **04.11.2026 · Quellfassung 0.1.1-alpha.8**. Die fünf vom Nutzer freigegebenen Punkte sind umgesetzt. Das bestehende Alpha-7-ZIP bleibt unverändert; dieses Änderungspaket erzeugt kein neues Release.

## Übersicht und Fußzeile

Unterhalb der Lernbereiche und der Lernroutine steht eine dezente Unterstützungskarte. Sie benennt den Projektinhaber und bietet einen GitHub-Stern sowie freiwillige Unterstützung per PayPal an. Ein eigener Link führt zu GitHub Issues für Fehler oder Vorschläge. Die Karte unterbricht keine Aufgabe und verändert die Themenreihenfolge nicht; WiSo bleibt an Position 08.

Die Fußzeile ergänzt **Über das Lernwerk**, **Fehler oder Vorschlag melden** und **Unterstützen**. Version und SQL-Einrichtung bleiben erreichbar. Die Projektseite öffnet innerhalb der App, externe Ziele öffnen mit `noopener noreferrer` in einem neuen Tab. Der Lernraum bleibt dadurch im bisherigen Tab bestehen.

Ein GitHub-Stern ist eine freiwillige Anerkennung über „Star“ am Repository, keine Bewertung von einem bis fünf Sternen. Die App fragt den Sternstatus nicht ab und setzt keinen Stern automatisch. Fehlermeldungen und Vorschläge werden auf GitHub angelegt; die App überträgt keine Lernstände oder Anhänge dorthin. Eine kurze Erklärung nennt die hilfreichen Angaben App-Version, Aufgabe und nachvollziehbare Schritte. Auf der Projektseite steht auch, dass GitHub-Meldungen öffentlich sind.

## Urheberseite

Die Route `/ueber-das-lernwerk` nennt **Christopher Weismantel · b0rnkilla** für Konzept und Projektentwicklung. Der Text beschreibt die Entstehung aus der eigenen Prüfungsvorbereitung. Die Wortmarke b0rnkilla wird als Text dargestellt; das vom Nutzer später gelieferte Logo kann sie ergänzen. Es werden keine unbekannten Kontakt- oder Adressangaben ergänzt und keine registrierten Markenzeichen behauptet.

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

Die README führt mit Nutzen, direktem Release-Link, einem großen Übersichtsbild und einer kompakten Galerie durch SQL, Algorithmen und WiSo. Danach folgen drei Startschritte und die Projektaktionen. Entwicklerdetails bleiben in den Fachdokumenten. Die Nutzung wird weiterhin ausschließlich über Releases → Windows-ZIP → Entpacken → EXE beschrieben.

Die Bilder liegen unter `docs/images`: `uebersicht.png`, `sql.png`, `algorithmen.png` und `wiso.png`. Es sind neue Browseraufnahmen der tatsächlichen App mit eigenem synthetischem Testprofil, keine Prüfungsabbildungen und keine Nutzerlernstände. Auf ausdrücklichen Nutzerwunsch beschreibt die README den vollständigen aktuellen Funktionsstand mit 220 Aufgaben als aktuelle App-Fassung; der frühere Vorschau-/Paketvergleich entfällt dort. Der tatsächliche Paketstand bleibt in der technischen Release-Dokumentation nachvollziehbar: Alpha 7 ist unverändert, ein neues ZIP entsteht erst nach der README-Freigabe und dem anschließenden Committext-/Commit-Schritt.

Die Link- und Darstellungsprüfung ist im [Testprotokoll](testprotokoll.md) festgehalten. Grundlage für die GitHub-Sternfunktion: [GitHub-Dokumentation](https://docs.github.com/en/get-started/exploring-projects-on-github/saving-repositories-with-stars).
