# Async

Demonstriert die Notwendigkeit, lange dauernde Prozesse in nebenläufige Aktivitäten
auszulagern, um ein ruckelfreies Rendering zu gewährleisten.
Zeigt die drei verschiedenen Patterns, die in C#/.NET existieren: 

- Asynchronous Programming Model (APM)
- Event-based Asynchronous Pattern (EAP)
- Task-based Asynchronous Pattern (TAP)

Siehe auch [Asynchronous Programming Patterns](https://msdn.microsoft.com/en-us/library/jj152938.aspx)
im MSDN.

## Bespiel-Applikatione

Das Code-Beispiel enthält eine FUSEE-Applikation, die eine Zeichenkette in einer Art Ticker über den
Bildschirm laufen lässt. Gleichzeitig kann ein 3D-Modell interaktiv bewegt werden.

Ein Button in der oberen linken Fensterecke lädt eine Textdatei mit langem Inhalt (Shakespear's gesammelte
Werke) aus dem Web, um diesen, sobald er geladen wurde, im Ticker anzuzeigen.

In der Datei [Main.cs](Desktop/Main.cs#63) des Desktop-Build ist ab Zeile 63 die Implementierung des Ladens
der Textdatei als Reaktion auf den Button-Click in drei Varianten implementiert.


###Synchron
Die erste Implementierung ist ein synchroner Download, d.h. Sobald der Button geklickt wurde, 
wird der Download gestartet und erst wenn der Text vollständig heruntergeladen wurde, fährt die
Anwendung mit dem Rendern der visuellen Inhalte fort. Wenn ein Benutzer, z.B. mit den 
Pfeiltasten, das Modell dreht und gleichzeitig das Laden des Textes per Button initiiert,
wird die Bewegung des 3D-Modells für die Zeit des Herunterladens angehalten. 

Gerade in stark interaktiven Umgebungen wie Echtzeit 3D-Visualisierungen sollte das fortwährende
Rendern unbedingt gewährleistet sein. Daher sollten zeitintensive Aktionen wie das Laden von Daten
der umfangreiche Berechnungen in nebenläufige Threads ausgelagert werden.

Neben der Möglichkeit, direkt mit den vom Betriebssystem zur Verfügung gestellten Threads
zu arbeiten, bietet C# bietet eine Reihe von Möglichkeiten an, mit programmiertechnsich einfacherer
Herangehensweise Nebenläufigkeit (die hier auch oft _Asynchronizität_ genannt wird), zu erzeugen. 
Die drei Möglichkeiten sind in oben genanntem Artikel beschrieben.

Ein Großteil von der .NET-Library zur Verfügung gestellten Funktionalität zur Implementierung von 

### Asynchronous Programming Model (APM)
Das APM ist historisch zu betrachten und wird nicht mehr unterstützt, daher ist es in diesem
Code-Beispiel auch nicht implementiert. Aus Anwendersicht muss eine asynchrone Aktion mit
`BeginXYZ()` gestartet werden. Sobald das Ergebnis benötigt wird, kann mit `EndXYZ()`
 

