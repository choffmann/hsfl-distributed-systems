# VS-Hausarbeit

In der Hausarbeit geht es darum, ein Verteiltes System aufzubauen.

## Aufbau des Projektes

Das gesamte Projekt besteht aus 6 Solutions. Zu den gehören der `Server`, `Client`, `Logger`, `PluginContract`, und die
zwei Plug-ins: `Primfaktorzerlegung` und `Hashkollision`. Für eine hübsche und bessere Textausgabe in der Konsole, wird
das externe Framework [Spectre.Console.CLI](https://spectreconsole.net/) verwendet. Dieses Framework ermöglicht es, die
Textausgabe in der Konsole zu verbessern und auch um eine Command-App zu entwickeln, was beim Client verwendet wurde.

## Logger

Die Solution `Logger` beinhaltet die Logger-Funktionalität. Bedeutet, die Solution ist dafür zuständig, die
Informationen in die Konsole sowie in eine Logdatei zu schreiben. Dabei gibt es 6 verschiedene
Log-Level: `Debug`, `Info`, `Success`, `Warning`, `Error` und `None`. Es kann ein Log-Level definiert werden, um das
Verhalten in der Ausgabe zu beeinflussen. Der Logger verfügt über mehrere Methoden, um die Informationen auszugeben. Die
Methoden sind im Interface `ILogger` ersichtlich. Die Log-Datei vom Server wird in den Ordner Logs geschrieben. Der
Ordner befindet sich an derselben Stelle, wo die Server.exe gestartet wird. Die Log-Datei vom Master wird
als `server.log` gespeichert. Die Log-Dateien von den Worken haben deren ID im Namen voraus (
z.B. `67db310f-8e27-44e5-94ce-8439c9ad7d80server.log`)

## Server

Der Server kann zwei Zustände annehmen. Entweder fungiert der Server als Master und dirigiert die Arbeit oder der Server
fungiert als Worker und wartet auf Anweisungen vom Master. Wird das Programm gestartet, versucht das Programm, einen
bereits laufenden Server bzw. einen existierenden Master im Netzwerk zu suchen. Wird ein Master gefunden, startet der
Server als Worker und meldet sich beim Master an. Wird kein Master gefunden, wird der gestartete Server zum Master und
bereitet alle Funktionalitäten des Masters vor. Zum automatischen Verbinden wird ein Discovery Proxy von WCF verwendet.
Der Master startet den Discovery Proxy und wartet, bis sich ein Worker verbindet. Dasselbe Verfahren wird auch bei der
automatischen Verbindung vom Client verwendet. Sowohl der Client als auch ein Worker schauen, ob Sie sich mit dem
entsprechenden `ServiceContract` verbinden können. Sowohl der Server als auch der Client haben eine eindeutige ID.

Wird nun ein Server gestartet und es befindet sich ein Master im Netzwerk, übermittelt der Worker seine ID an den
Master. Die ID wird vom Master abgespeichert und ein `CallbackChannel` wird aufgebaut, sodass der Master jederzeit
Funktionen auf den Worker ausführen kann.
Wird ein Plugin vom Client hochgeladen, wird dies zur Laufzeit zuerst bei dem Master implementiert. Sind aktuell Worker
im System vorhanden, wird das Plugin vom Master zu allen aktiven Workern weitergegeben, welche dieses Plugin dann auch
zur Laufzeit implementieren. Der Client kann nun ein Kommando absetzte, ein Plugin auszuführen. Dazu verfügt jedes
Plugin über ein `CommandName`, welcher vom Client an den Server geschickt wird. Der `CommandName` wird mit den Plugins
verglichen. Hat ein Plugin diesen einen `CommandName`, wird dieses Plugin auf einen Worker ausgeführt. Dabei sucht der
Master sich einen Worker aus, welcher aktuell in dem Status `IDLE` ist. Der Worker startet das Plugin in einen eigenen
Thread und setzte seinen Status auf `WORKING`. Somit kann der Worker auf weitere Anfragen, wie die Abfrage zu seinen
aktuellen Status antworten, nimmt aber keine weiter Aufgabe entgegen. Wenn der Client nun wieder eine Anfrage schickt,
der Worker aber noch beschäftigt ist, sendet der Master die Anfrage an einen anderen Worker. Stehen aktuell keine Worker
zur Verfügung, wird die Meldung ausgegeben, das gerade keine Worker bereitstehen.

Sollte sich ein neuer Worker im System anmelden und der Master diesen Worker zur Bearbeitung des Plugins auswählt, hat
der neue Worker natürlich keine Ahnung, um welches Plugin es sich handelt. In diesen Fall wird das Plugin vom Master an
den Worker weitergegeben, der Worker implementiert das Plugin und führt die Aufgabe aus.

Die .dll Datei der Plugins werden ähnlich wie die Log-Dateien, in den Ordner Plugins gespeichert, welche im selben
Verzeichnis erstellt wird, wo das Programm ausgeführt wird. Bei jeden starten des Servers wird überprüft, ob sich
bereits ein Plugin in diesen Ordner befindet. Ist dies der Fall, wird das Plugin beim Starten geladen und der Client
muss das Plugin nicht nochmal hochladen

### Rückmelde Codes

Der Server antwortet mit Zahlen. Folgende Zahlencodes wurden angelegt:

| Code | Beschreibung                                        |
|------|-----------------------------------------------------|
| 0    | OK                                                  |
| 10   | Keine Worker zur bearbeitung verfügbar              |
| 20   | Der Plugin `CommandName` wurde nicht gefunden       |
| 28   | Unerwarteter Fehler beim Registrieren eines Plugins |
| 29   | Unerwarteter Fehler beim Ausführen eines Plugins    |

## Plugins

Es wurden zwei Plugins erstellt. Zum einen die Primfaktorzerlegung und zum anderen eine Hash-Kollision. Alle Plugins
erweitern die Basisklasse `PluginContract.Plugin`. Diese Basisklasse müssen alle erstellten Plugins implementieren,
damit diese vom Server erkannt werden. Die Basisklasse enthält Methoden, welche zur Laufzeit vom Server abgerufen
werden. Hier werden Funktionen vom interface `ILifecycle` zur Verfügung gestellt,
wie `OnServerInit()`, `OnServerStartup()` und `OnServerStop()`. Diese Methoden werden immer vom Server, wie der Name
bereits deuten lässt, bei der Initialisieren, beim Starten und beim Stoppen des Servers aufgerufen. Zudem erhält die
Basisklasse eine abstrakte Methode `OnServerExecute(string[] args)`. Diese Methode wird vom Server aufgerufen, wenn der
Client dieses Plugin startet. In dem Parameter `args` sind Argumente enthalten, welcher der Benutzer im Client angeben
kann. Somit können also beliebig viele Argumente über den Client an ein Plugin übergeben werden. Das Plugin muss diese
Argumente jedoch in der gewissen Reihenfolge interpretieren und gegebenenfalls Konvertieren (z.B. Argument soll vom
Typ `Int` sein, übergeben werden aber nur `Strings`). Darüber hinaus gibt es zwei Properties, welche von den Plugins
überschrieben werden sollten. Zum einen die Property `Name` und zum anderen die Property `CommandName`. Die
Property `Name` gibt den Namen des Plugins an. Dieser wird verwendet, um den Namen im Logger auszugeben. Durch die
Property `CommandName` wird die Eigenschaft gesetzt, wie das Plugin vom Client aufgerufen wird. Der Client übergibt den
Server nur einen String mit den entsprechenden Argumenten, welches Plugin aufgerufen werden soll. Um das Plugin zu
identifizieren, welches ausgeführt werden soll, werden die Plugins mit der Angabe `CommandName` verglichen.
Zudem überschreibt die Basisklasse den Logger und gibt den Namen des Plugins in jeder Log-Ausgabe mit aus.

### Plugin Primfaktorzerlegung

Dieses Plugin bestimmt Primzahlen, welche zusammen multipliziert eine entsprechende Zahl ergeben. Diese Zahl wird durch
den Client bzw. dem Benutzer angegeben. Der CommandName dieses Plugins lautet prime. Das Plugin kann somit über den
Client über folgenden Befehl gestartet werden:

```bash
Client.exe plugin run prime 300
```

Der Server wird das Plugin Primfaktorzerlegung mit der Zahl 300 aufrufen. (Zur Simulation wird nach der Ausführen 30
Sekunden gewartet, da ich hier getestet habe, ob der Worker den Status auf WORKING setzt und dieser trotzdem abgefragt
werden kann, wenn der Worker beschäftigt ist.)

### Plugin HashKollison

Dieses Plugin nimmt eine Datei, ermittelt den Hash-Wert und versucht einen anderen Hash-Wert zu ermitteln. Dabei wird
das ByteArray von der angegebenen Datei um eine Zufällige Anzahl geschiftet und mit dem originalen Hash-Wert verglichen.
Dabei wird nach einem gewissen Timeout die Überprüfung gestoppt. Standardmäßig liegt der Timeout bei 5 Minuten, kann
aber über den Client angepasst werden. Der CommandName dieses Plugins lautet hash. Das Plugin kann somit über den Client
über folgenden Befehl gestartet werden:

```bash
Client.exe plugin run hash <FILE_PATH> 6
```

Der Client liest die Datei unter dem Pfad als ByteArray und sendet dies zu dem Server. Der Server wird das Plugin
HashKollision ausführen und übergibt dem Plugin die Argumente. In diesen Fall bricht das Plugin den Vergleich nach 6
Minuten ab. Alternativ kann dieses Argument auch weggelassen werden, dann würde das Plugin nach den Standard 5 Minuten
den Vergleich abbrechen.

## Client

Der Client wurde als Command-App entwickelt, so dass der Client entsprechende Parameter vom User übergeben bekommt,
womit sich der Client mit dem Server verbindet, und entsprechende Funktionen ausführen kann. Hier wurde auch das
Framework [Spectre.Console.CLI](https://spectreconsole.net/) verwendet. Der Client versucht, ohne Angaben von
Informationen des Servers, sich mit dem
Master zu verbinden. Der Client verfügt über 2 Haupt-Kommandos. Zum einen um den Status von den Servern abfragen und zum
anderen, um Plugins hochzuladen und mit diesen zu interagieren.

### Kommando Status

Um den aktuellen Status vom Server-System zu erhalten, kann der Client den Server nach dem aktuellen Stand von dem
Master, als auch alle registrierten Worker abfragen. Mit dem folgenden Befehl wird die Abfrage gestartet.

```bash
Client.exe status
```

Der Client versucht sich nun mit dem Master zu verbinden. Als Antwort wird eine Tabelle auf der Konsole mit den
Informationen zum Status ausgegeben (Siehe Abbildung 6).

### Kommando Plugin

Der Client verfügt über ein Plugin Kommando, um alle installierten Plugins auf dem Server abzurufen und in einer Tabelle
anzuzeigen. Zudem bietet das Kommando die Funktion, ein Plugin auf den Server zu installieren oder auch um mit einem
Plugin zu interagieren und auf den Server auszuführen.

### Verbose Modus

Zu jeden Befehl kann der Parameter `-v` oder `--verbose` mit angegeben werden. Ist diese Option aktiviert, gibt der
Client Informationen zu den Aktivitäten in der Konsole aus.

### Befehle im Überblick

| Befehl                                        | Beschreibung                                                                                                                                                                 |
|-----------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Client.exe status`                           | Zeigt Status von allen Knotenpunkten an                                                                                                                                      |
| `Client.exe plugin status`                    | Zeigt alle verfügbaren Plugins auf den Server an                                                                                                                             |
| `Client.exe plugin load <PFAD ZU PLUGIN.DLL>` | Lädt die angegebene Plugin dll zum auf den Server hoch. Der Master verteilt diese an alle Worker                                                                             |
| `Client.exe plugin run prime 300`             | Dies ist ein Beispiel, wie die Plugins gestartet werden können. Der Begriff prime/hash sind dabei die CommandoNamen von den Plugins, welche in den Plugins definiert werden. |