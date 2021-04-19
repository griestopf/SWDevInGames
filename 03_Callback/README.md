# Lektion 03 Callbacks, Delegates, Events, Anonymous methods, Lambdas

Klassische Konsolen-Programme laufen in der Regel sequenziell in einer zur Compilezeit feststehenden Reihenfolge ab.
Variationen im Programmablauf, die erst zur Laufzeit entschieden werden, bsp. auf Grund von Eingabedaten oder
Benutzer-Eingaben, können in ausreichendem Maß mit den klassischen Programmiersprachenkonstrukten wie 
Verzweigung (z.B. mit `if`-`else` / `switch`) oder Schleifen (z.B. mit `foreach` / `while`) implementiert werden.

In vielen zeitgemäßen Programmier-Situationen besteht die Notwendigkeit, den in bestimmten Situationen 
auszuführenden Code stärker zu strukturieren. Das ist besonders dann notwendig, wenn die Ausführung
bestimmten Codes nicht nur von einem bestimmten Zustand während des ansonsten klar strukturierten
Ablaufes abhängt, sondern zu einem beliebigen, zur Compilezeit nicht festzulegenden Zeitpunkt notwendig
wird.

Mit `if` etc. kann diese Strukturierung nicht geschehen, da sämtliche Möglichkeiten untrennbar 
in einer Methode stehen. Das Ziel ist, den bei Bedarf auszuführenden Code in Methoden zu bündeln 
und zu hinterlegen. Derartige Methoden werden allgemein  _Callbacks_ genannt, also Methoden, die 
Hinterlegt werden können, um beim Eintreten bestimmter Bedingungen ausgeführt zu werden. Dabei
sollen aufrufender und aufgerufener Code nichts von einander wissen müssen - ggf. soll diese Verbindung
auch erst zur Laufzeit herstellbar sein.


### Beispiel Progress-Anzeige

Eine Klassenbibliothek enthält Code für eine langwieriege Berechnung. Benutzer der Klassenbibliothek
sollen Benutzern im Kontext der Anwendung Auskunft geben können, wie weit die Berechnung schon fortgeschritten
ist. Die Entwickler der Klassenbibliothek wissen aber nichts über diesen Kontext, wie z.B.
- Läuft die Anwendung interaktiv oder als Service -> sollen Benachrichtigungen auf dem Bildschirm
  oder in einem Log-File stattfinden?
- Läuft die Anwendung in einer Konsole oder in einem UI-System, wenn UI-System in welchem? -> Sollen
  Benachrichtigungen als Text-Nachricht oder als grafische Progress-Bar dargestellt werden? Wenn Progress-Bar:
  In welchem Betriebssystem / mit welcher UI-Library ist die Progress-Bar realisiert?
- In welcher Sprache soll die Ausgabe erfolgen?

Daher ermöglichen die Entwickler der Klassenbibliothek es den Anwendern, _Callback-Code_ beim Aufruf der 
Berechnung zu übergeben, der zu bestimmten Zeitpunkten während der Berechnung aufgerufen wird, um eine
Benachrichtigung im Kontext der aufrufenden Applikation zu ermöglichen.


## Callbacks mit `virtual`

Eine Möglichkeit, in Abhängigkeit einer Vorbedingung eine Methode ausführen zu lassen, liefert C# bereits 
mit `virtual` Methoden, die Polymophie erlauben. Damit lassen sich Aufrufer implementieren, die nur die
Basisklasse kennen und wissen, dass es eine bestimmte `virtual` (Callback-)Methode gibt, die bei Eintreten einer
Bedingung (die der Aufrufer feststellt) aufzurufen ist. Aufrufende Methoden können von der Basisklasse
ableiten und die Callback-Methode implementieren.

[X03_virtual](X03_virtual/Program.cs) zeigt o.g. Beispiel mit diesem Ansatz.


## Callbacks mit Interfaces

Die Funktionalität, die `virtual` und `override` ermöglichen, lässt sich mit Interfaces noch stärker strukturieren.
Interfaces lassen sich als Klassen verstehen, bei denen _alle_ Methoden `virtual` sind und bei denen
die Basisklasse _keine_ Implementierung erlaubt. Somit legt die Basisklasse nur die Informationen für den Aufrufer
fest. 

[X03_interface](X03_interface/Program.cs) zeigt o.g. Beispiel mit diesem Ansatz.

In Java werden Callbacks häufig über Interfaces implementiert. Die Möglichkeit in Java, Interfaces und deren
Methoden anonym -inline- dort zu implementieren, wo sie benötigt werden, begünstigt diesen Ansatz. Es bleibt
die Notwendigkeit, dass für jede Callback-Situation zunächst ein Interface deklariert werden muss, das den
"Vertrag" zwischen aufrufendem und aufgerufenem Code beschreibt. Somit entstehen viele Interfaces mit jeweils 
nur einer Methode.


## Delegates 

In C# wurde schon früh ein anderer Ansatz gewählt, nämlich der eher Methoden-orientierte Ansatz über 
Delegates, bzw. Events. Hier muss noch nicht mal ein Interface (mit ggf. nur einer Methode) deklariert
werden, sondern mit Hilfe des Schlüsselwortes `delegate` kann ein eigener Datentyp deklariert werden,
dessen Instanzen Methoden sind. Ein Datentyp der Kategorie `delegate` und somit der "Vertrag" zwischen Aufrufer
und Aufrufendem besteht dabei nur aus der Methodensignatur, d.h. den Parametern und dem Rückgabewert. 
Explizit NICHT festgelegt wird:
- Methodenname
- Zugriffs-Modus (`public`, `protected`, `private`, `internal`)
- Klassen- oder Instanz-Methode (`static` oder nicht)

Das vereinfacht ist die Implementierung einer Callback-Methode erheblich, denn Entwickler können sich 
die o.g. Eigenschaften der als Callback zu verwendenden Methode aussuchen und von Fall zu Fall
entscheiden.

[X03_delegate](X03_delegate/Program.cs) zeigt o.g. Beispiel mit diesem Ansatz.

Wie bereits oben beschrieben, ist ein häufiger - aber nicht der einzige - Anwendungsfall für `delegate`
die Implementierung von Callback-Mechanismen für das Auftreten von _Ereignissen_. Beispiele sind

- Fortschrittsmeldungen (wie in unserem Beispiel)
- Benutzer-Interaktionen (Maus bewegt, Taste gedrückt)
- Resultat-Benachrichtigung langwieriger Aktionen, wie Berechnungen, 
  Ladevorgänge (z.B. aus dem Netz), Datenbankabfragen

Nicht in allen Fällen genügt es - wie bei unserem `Calculator` - die Callback-Methode als Parameter
eines Vorgangs mitzuliefen. In komplexeren Szenarien kann es notwendig sein, dass ein Callback über
den Aufruf einer Methode hinaus gespeichert werden soll.

[X03_delegateThreaded](X03_delegateThreaded/Program.cs) ist eine Abwandlung des Beispiels. Hier wird
die Berechnung _asynchron_ angestoßen, d.h. sie läuft in einem zweiten Thread - quasi zeitgleich - zur
Bearbeitung der Methode `Main`. Da die Methode `StartSomeLengthyCalculation()` sofort nach dem 
Aufruf die Kontrolle wieder an den Aufrufer zurückgibt, muss die Callback-Methode in einer Klassenvariablen
des `Calculator` gespeichert werden. Zudem wurde ein zweiter Callback hinzugefügt, dem das Endergebnis
übergeben wird. 

In diesem Beispiel wird die _Nebenläufigkeit_ (Asynchronizität) verwendet, um die Notwendigkeit zu 
demonstrieren, Callback-Methoden als Instanz-Variablen zu speichern.

## Events

Im letzten Beispiel sind die beiden Callbacks für den Fortschritt und das Endergebnis als Instanz-Variablen
der Klasse `Calculator` implementiert. Stellen wir uns ein erweitertes Szenario vor:

Nicht nur ein Observer (in unserem Fall `ReportProgress`), sondern zwei oder noch mehr Instanzen sollen bei Fortschritt
oder Ergebnis der Berechnung informiert werden.

Beispiele für solche Szenarien sind z. B. komplexe Programme mit unterschiedlichen Views
auf die selben zu grunde liegenden Daten: Sobald sich die Daten ändern, sollen alle Views ihre Ansichten aktualisieren.
Die Klassen, die die Daten halten müssen also für ein bestimmtes Ereignis mehrere Callbacks unterschiedlicher
Views aufrufen.

Zudem kann es eine Forderung sein, dass die unterschiedlichen Implementierer der Callbacks unabhängig voneinander 
entwickelt werden können: Es soll nicht möglich sein, dass ein Callback-Implementierer einfach die Callback-Methode
eines anderen löscht oder sogar aufruft. Um derartige Szenarien zu implementieren, müssten Callback-Aufrufer eine 
Menge an "Buchhaltungscode" implementieren: 

- Statt nur eines Callback müsste pro Ereignis eine Liste (ein Array) von Callbacks gehalten werden.
- Es müsste sichergestellt werden, dass nur die Instanz, die ein Callback registriert, dieses auch wieder löschen darf
- Es müsste sichergestellt werden, dass ein registriertes Callback von niemand anderem als dem Callback-Aufrufer
  aufgerufen werden kann.

Für diese Zwecke gibt es in C#  _Events_. Deren Verwendung ist denkbar einfach und es werden damit alle drei o.g. 
Forderungen implementiert.

[X03_eventThreaded](X03_eventThreaded/Program.cs) enthält folgende Änderungen:

- Die Deklaration der Callback-Klassenvariablen 
  ```C#
    public ProgressReporter PR;
    public ResultReceiver RR;
  ```
  enthält nun zusätzlich das Schlüsselwort `event`:
  ```C#
    public event ProgressReporter PR;
    public event ResultReceiver RR;
  ```

- Die "Registrierung" der Callbacks geschieht nun nicht über den Zuweisungsoperator `=`, sondern
  über den speziell für Events mit eigener Semantik versehenen `+=` Operator:
  ```C#
    calc.PR += ReportProgress;
    calc.PR += OtherProgressReporter;
    calc.RR += ReceiveResult;
  ```

- Um zu demonstrieren, dass ein Event nun mehr als nur eine Callback-Methode handhaben kann, wurde der 
  `ReportProgress` Event mit zwei Callbacks (nun _Event-Listener_ genannt) versehen.

Events sind, neben Methoden, Feldern und Properties also eine vierte Möglichkeit, Klassen mit Eigenschaften
auszustatten. Eine Klasse kann einen `public event` deklarieren, um anderen Code-Stellen die Möglichkeit
zu geben, auf bestimmte Ereignisse während der Lebenszeit einer Instanz dieser Klasse zu reagieren.

## Anonyme Methoden / Closures

Aus Sicht von Entwicklern, die Event-Listener, also Callback-Methoden, implementieren, kann es mühselig sein,
den Code, der bei Eintreten eines Events ausgeführt werden soll, in eine eigene Methode auszulagern. Stattdessen
kann über _anonyme Methoden_ der Code gleich dorthingeschrieben werden, wo der Event-Listener (mit `+=`) dem 
Event zugewiesen wird. Anonyme-Methoden werden mit dem `delegate`-Schlüsselwort deklariert, gefolgt von der
durch die Delegate-Deklaration festgelegte Parameterliste, sowie einem Methodenrumpf in geschweiften Klammern.

Eine syntaktische Variante von anonymen Methoden sind so genannte Lambdas, die die Schreibweise für 
anonyme Methoden noch weiter verkürzen. Insbesondere bei ein-elementigen Parameterlisten und bei einzeiligen
Methodenrümpfen ergibt sich hier eine extrem kurze Schreibweise. Lambdas erkennt man über den "Wird-Abgebildet-Auf"
Operator - in Anlehnung an mathematische Funktionen `=>`

[X03_eventLambda](X03_eventLambda/Program.cs) zeigt, wie die drei Callbacks nun als anonyme Methoden bzw. 
Lambda implementiert werden.

Neben einer verkürzten Schreibweise (gegenüber expliziten Methodendeklarationen) bieten die beiden impliziten
Varianten (anonyme Methode oder Lambda) einen weiteren Vorteil: _Closures_. Kurz gesagt bieten diese die 
Möglichkeit, aus dem Kontext der inline deklarierten Methode auf den Kontext der umgebenden Methode zuzugreifen.
Im Beispiel wird davon Gebrauch gemacht, indem das Ergebnis der Berechnung wieder in eine lokale Variable `theResult`
der umgebenden Methode `Main()` geschrieben wird.


## Anwendungsbeispiele in FUSEE

In Fusee wird an verschiedenen Stellen von den o.g. Mechanismen Gebrauch gemacht. Ein zukünftiges 
Einsatzgebiet werden (noch zu implementierende ) UI-Bausteine sein, die auf Benutzereingaben
reagieren sollen. Diese können, im Falle von Benutzeraktionen, Element-spezifische Events 
implementieren (z.B. `ButtonClicked` für UI-Buttons, `TextChanged` für Eingabefelder o.ä.)


### Eingabegeräte

Die von FUSEE unterstützten Eingabegeräte wie z.B. `Mouse`,  `Keyboard` und `Touch` werden in den 
FUSEE-Beispielen meistens
im "polling"-Modus verwendet, d.h. es wird pro Frame deren Zustand abgefragt. Alle Eingabegeräte in FUSEE
sind aber von der Basisklasse
[`InputDevice`](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Engine/Core/InputDevice.cs#L18)
abgeleitet, die  einen Event für Werteänderungen  
an den von einem Eingabegerät exponierten 
[Achsen](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Engine/Core/InputDevice.cs#L298)
(wie Mouse-Achsen, Mouse-Geschwindigkeit)
sowie einen Event für Zustandsänderungen an von einem Eingabegerät exponierten 
[Buttons](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Engine/Core/InputDevice.cs#L390)
(Mouse-Buttons, Tastatur-Keys)
implementiert. FUSEE-Applikationen können auch -statt Polling- per Events/Callbacks auf Benutzereingaben reagieren.


### Traversierung/Double Dispatch

Beim Traversieren von Szenengraphen wird das Visitor-Pattern verwendet. Wir werden eine Implementierung
kennen lernen, die die Entscheidung, welche Methode beim "Besuch" eines Knoten aufzurufen ist in Abhängigkeit
sowohl des Knotentyp als auch des Traversierungsgrundes (z.B "Rendern", "Picking", ) mit Hilfe eines 
verschachtelten Aufrufs zweier `virtual`-Methoden (`Visit()` und `Accept()`) realisiert. 

In FUSEE wurde ein anderer Weg gegangen. Die Basisklasse `SceneVisitor` hält für jeden Traversierungsgrund 
(=Ableitungen von `SceneVisitor`) ein Dictionary von Methoden, die in der jeweiligen Ableitung mit dem Attribut
[`[VisitorMethod]`](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Xene/Visitor.cs#L12)
 ausgezeichnet sind. 

Diese Methoden werden zu einem Initialisierungszeitpunkt zusammengesammelt und in einem 
[Dictionary](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Xene/Visitor.cs#L95)
nach Komponententyp indiziert eingetragen. Damit diese Methoden überhaupt als Objekte von einem 
bestimmten Datentyp handhabbar sind, gibt es den
[`delegate`-Datentyp `VisitComponentMethod`](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Xene/Visitor.cs#L18). 

In der Methode 
[`DoVisitComponent()`](https://github.com/FUSEEProjectTeam/Fusee/blob/8c3a93afe8ddd1085ab151c9812c0d43fe2d29de/src/Xene/Visitor.cs#L522)
 wird dann das für den Komponententyp passende Delegate-Objekt aus dem für den Traversierungsgrund passende
Dictionary herausgesucht und 
[aufgerufen](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Xene/Visitor.cs#L5410)


### BitBlt inner loop

Beim Umgang mit zweidimensionalen Pixeldaten gibt es die klassische Operation _BitBlockTransfer_, kurz _BitBlt_
oder _Blt_, gesprochen _Blit_. Dabei sollen aus Speicherblöcken, die Pixeldaten enthalten, rechteckige Bereiche
in andere Speicherblöcke übertragen werden. Da zweidimensionale Pixeldaten zeilenweise linear im
eindimensionalen Speicher abgelegt werden, spielen - neben der Größe des zu übertragenden 
rechteckigen Bereichs, sowie der (x,y)-Positionen im Quell und im Zielbild - auch noch die Höhe und Breite 
des Quell-, sowie des Ziel-Bildes eine Rolle. Zudem soll 
u.U. noch mit unterschiedlichen Pixelformaten in Quell- und Zielbild umgegangen werden, also z.B. aus einem 
Bild OHNE Alpha-Kanal in ein Bild MIT Alpha-Kanal kopiert werden, oder ein Teil eines Quell-Graustufenbild in
einen bestimmten Kanal eines RGB-Zielbildes übertragen werden.

In FUSEE gibt es den Datentyp 
[`ImageData`](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Base/Core/ImageData.cs#L10),
mit dem sich rechteckige Pixelbilder mit unterschiedlichen Pixelformaten realisieren lassen.

Dieser Datentyp implementiert die Methode 
[`Blt`](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Base/Core/ImageData.cs#L107),
die eine solche Übertragungsfunktionalität eines rechteckigen Blocks von einem `src` Image in 
ein `dst` Image realisiert.

Nachdem in der Methode zunächst ggf. der gewünschte zu übertragende Bereich auf die tatsächlich in Quell-
und Zielbild vorhandene Größe angepasst wurde (Clipping), ist die eigentliche Übertragung als
Schleife über die zu übertragenden Zeilen realisiert. Pro Zeile ein (äußerer) Schleifendurchlauf, 
der die jeweilige Zeile kopieren soll.

Was allerdings zum Kopieren einer Zeile geschehen muss, hängt davon ab, ob die gesamte Zeile en-bloc
kopiert werden kann, oder ob innerhalb einer Zeile pro Pixel vorgegangen werden muss. 
Das blockweise Kopieren der Zeilen kann dann erfolgen, wenn Quell- und Zielbild dieselben Pixelformate haben
(z.B. beide RGB ohne Alpha). 

Da C# optimierte Methoden zum blockweisen Kopieren von Daten im Speicher enthält, ist der Aufruf einer 
solchen Methode dem pixelweisen Kopieren vorzuziehen - wenn es denn geht. Die Entscheidung, ob es geht, 
steht allerdings schon mit dem gesamten Bild fest (und muss nicht pro Zeile oder gar pro Pixel
getroffen werden).

Um nun zu vermeiden, dass innerhalb der Zeilen-Schleife für jede Zeile erneut eine Abfrage
feststellt, ob Quelle und Ziel das selbe Pixelformat haben, hätte `Blt()` so implementiert werden können,
dass zunächst diese Entscheidung getroffen wird und dann in eine von zwei unterschiedlichen 
Implementierungsmethoden verzweigt wird. Das hätte aber Code-Kopie (zumindest des Schleifenkopfes) bedeutet.

Stattdessen wird im Schleifenrumpf zeilenweise ein in der lokalen Variable
[`copyLine`](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Base/Core/ImageData.cs#L77)
gespeichertes `delegate`
aufgerufen, das den Code zum Kopieren einer Zeile enthält. Diese lokale Variable kann dann _vor_ der 
Schleife mit der passenden Funktionalität (blockweise oder pixelweises Kopieren einer Zeile) 
initialisiert werden.

Exakt das selbe Prinzip wird verwendet, wenn festgestellt wurde, _dass_ pixelweise kopiert werden muss:
Die Implementierung für das pixelweise Kopieren einer Zeile besteht wiederum aus einer Schleife, die
über alle Pixel der zu kopierenden Zeile iteriert. Dabei muss pro Pixel eine Konvertierungsmethode
aufgerufen werden. Diese ist abhängig von Quell- und Ziel-Pixelformat: Beim Weglassen des Alpha-Kanal
pro Pixel muss z.B. etwas anderes geschehen, als beim Umwandeln von RGB- in Graustufen-Daten.

Die Entscheidung, welche Konvertierungsmethode zum Einsatz kommt, steht ebenfalls pro Aufruf von 
`Blt()` fest und muss nicht für jedes zu kopierende Pixel erneut innerhalb der inneren Schleife
ermittelt werden. Daher wird - ebenfalls vor dem gesamten Kopiervorgang - die für jedes Pixel
zu verwendende Konvertierungsfunktion in der lokalen Variable
[`CopyPixel`](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Base/Common/ImageData.cs#L225)
als `delegate` gespeichert und dann pro Pixel aufgerufen.

## Further Reading

- Der Unterschied zwischen Java's Callback-Ansatz mit Hilfe so genannte anonymer Klassen und dem 
  Gegenentwurf von C# mit Events, Delegates und der Möglichkeit, anonymer Methoden oder Lambdas
  wird in der Community häufig diskutiert. 
  [Googeln nach diesen Schlagworten](https://www.google.de/search?q=c%23+delegates+vs+java+anonymous+classes)
  bringt ein paar Beispiele.

- Alle hier oben aufgeführten Code-Beispiele lassen sich als Implementierungen des 
  [_Observer Pattern_](https://de.wikipedia.org/wiki/Beobachter_(Entwurfsmuster))
  auffassen, einem bekannten Software-Entwurfsmuster. Die Sprachunterstützung durch die
  `event` Syntax und die Bezeichnung _Event Listener_ stellen sogar
  den direkten Bezug her.






