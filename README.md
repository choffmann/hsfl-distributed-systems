# VS-Hausarbeit
In der Hausarbeit geht es darum, ein Verteiltes System aufzubauen.

## Core
TODO: 

### Master
TODO: 

### Worker
TODO: 

## Plugins
TODO: 

## Logger
Es gibt fünf verschiedene Loglevel mit sieben verschiedenen Methoden, die ausgewählt werden können.

```c#
Logger.Debug("Eine Information die zum Debuggen interessant ist");
Logger.Info("Eine Information");
Logger.Success("Eine Operation wurde erfolgreich ausgeführt");
Logger.SuccessEmoji("Eine Operation wurde erfolgreich ausgeführt und als belohnung gibt es nmoch ein Emoji");
Logger.Waring("Eine Warnung");
Logger.Error("Eine schwerwigender Fehler");
Logger.Exception("Eine Exception, welche detailiert angezeigt wird");
```
