# Lektion 04 Generics, Collections, Indexer, Enumerator

## Generics & Collections

Wie die meisten aktuellen objektorientierten Sprachen erlaubt es C#, die Deklaration von Datentypen so 
offen zu halten, dass in der Deklaration verwendete andere Datentypen noch nicht konkret bekannt sein
müssen, sondern als Typ-Parameter verwendet werden können.

Solche -noch nicht ganz fertige- Datentypen, die andere Datentypen verwenden, sich hierbei aber noch nicht
ganz festlegen, welche Typen das denn sein sollen, nennt man in C# (und in Java) _Generics_.  In C++ werden 
solche Konstrukte _Templates_ genannt, weil es sich hierbei genau genommen noch nicht um die Deklaration
von fertigen Datentypen handelt, sondern um _Schablonen_, wie ein neuer Typ zu erstellen ist, wenn denn
die verwendeten Datentypen festgelegt wurden.

In vielen Sprachen (C#, Java, C++) werden solche Konstrukte durch Verwendung von Spitzklammern `<>` gekennzeichnet.

### Der häufigste Anwendungsfall: Collections-Klassen

Zunächst soll die Frage geklärt werden, wo solche Konstrukte überhaupt benötigt werden. Der wohl häufigste
Anwendungsfall, und in den meisten Sprachen auch der Grund, warum es solche Konstrukte wie Generics
überhaupt gibt, sind so genannte [Container, bzw. Collection-Klassen](https://de.wikipedia.org/wiki/Container_(Informatik)).
Inzwischen setzt sich der Begriff _Collection-Klassen_ durch, vermutlich um Verwechslungen mit leichtgewichtigen Virtualisierungen von Rechner- (v. a. Server-)Installationen rund um Technologien wie z. B. Docker oder Kubernetes zu vermeiden. Diese Bedeutung des Begriffs _Container_ wird in dieser Lektion _nicht_ behandelt. 

Dabei handelt es sich um Klassen, die in der Lage sind, Mengen von Objekten anderer Datentypen abzuspeichern.
Typischerweise gibt es unterschiedliche Collections-Klassen für unterschiedliche Speicher- und 
Zugriffsstrategien. Hier einige Beispiele:
- Listen: Ähnlich wie Arrays, aber mit zur Laufzeit veränderlicher Speicherkapazität
- Verlinkte Listen: Für schnelles Ein- und Ausfügen an beliebigen Positionen, dafür nur 
  eingeschränktem (sequenziellem statt wahlfreiem) Zugriff
- Stacks: Last-in-first-out Speicher
- Dictionaries / Hash-Tables: Indizierung mit beliebigen Schlüssel-Werten (nicht nur Integer-Indizes wie bei
  Arrays oder Listen).

Die Implementierung einer solchen Klasse, wie z.B. `List`, sollte dabei völlig losgelöst vom Typ der 
Objekte, die in der Liste gehalten werden, sein. So soll es möglich sein, eine Liste mit strings, 
eine Liste mit double-Werten und eine Liste mit selbst definierten Datentypen (ggf. sogar Delegates)
zu verwenden.

Erste Versionen von C# (und auch Java) boten hier nur die Möglichkeit, entweder jeweils eine Collections-Klasse
für den ganz konkreten Datentyp zu implementieren (also eine Listenklasse für string, eine für double, ...)
oder eine Collection-Klasse zu implementieren, die Objekte von der "Urklasse" `object` enthält. Da in C# und
Java alle Datentypen von `object` erben (anders als z.B. in C++), ist diese Möglichkeit gegeben. 

Hier ein sehr einfaches Beispiel für eine Collection-Klasse, die eine dynamisch wachsende Zahl an 
Elementen enthalten kann. Die Elemente werden in einem Array von `object` Elementen gespeichert.
Sobald dessen Kapazität erreicht ist, wird die Anzahl der Array-Einträge verdoppelt

```C#
    public class MyCollection
    {
        private object[] _theObjects;
        private int _n;

        public MyCollection()
        {
            _theObjects = new object[2];
            _n = 0;
        }

        public void Add(object o)
        {
            // If necessary, grow the array
            if (_n == _theObjects.Length)
            {
                object[] oldArray = _theObjects;
                _theObjects = new object[2 * oldArray.Length];
                Array.Copy(oldArray, _theObjects, _n);
            }

            _theObjects[_n] = o;
            _n++;
        }

        public object GetAt(int i)
        {
            return _theObjects[i];
        }

        public int Count
        {
            get { return _n; }
        }
    }
```

> #### 👨‍🔧 TODO
>
> - Legt mehrere Instanzen der obigen Klasse für die Speicherung unterschiedlicher 
>   Typen an, vor allem auch mit einer selbst geschriebenen Klasse.
> - Was muss beim (lesenden) Zugriff auf Elemente passieren?


Diese Implementierung auf Basis von `object` hat aber den Nachteil, dass beim Zugriff auf im Collection
gespeicherte Objekte jeweils ein Cast des konkreten Typs von und nach `object` erfolgen muss. Dieser
ist mit einem Laufzeit-Check verbunden, der erstens Zeit kostet und zweitens zur Laufzeit zu Fehlern
führen kann, die eigentlich der Compiler schon zur Compilezeit hätte checken können: Diese Art der 
Collection-Implementierung ist nicht typsicher.

Daher wurde bereits früh (jeweils mit Version 2 der Sprachen) in C# und Java die Möglichkeit von 
Generics eingeführt, mit deren Hilfe Datentypen deklariert werden können, die mit anderen Datentypen
parametrisiert werden können.

Erwähnenswert hierbei ist in C#, dass das Konzept nicht nur bei der Deklaration von Klassen
funktioniert, sondern bei sämtlichen Kategorien von selbst deklarierten Typen (außer enums)
wie z.B.

- Klassen (`class`)
- Interfaces (`interface`)
- Strukturen (`struct`)
- Delegates (`delegate`)

Schließlich können auch einzelne Methoden einer Klasse generisch deklariert werden, d.h.
wenn nur für einzelne Methoden Datentypen als Parameter benötigt werden, kann das bei der
Deklaration der Methode erfolgen und muss nicht auf die gesamte Klasse ausgedehnt werden.

### Beispiele

Die oben gezeigte Collection-Kklasse als Generic:

```C#
    public class MyCollection<T>
    {
        private T[] _theObjects;
        private int _n;

        public MyCollection()
        {
            _theObjects = new T[2];
            _n = 0;
        }

        public void Add(T o)
        {
            // If necessary, grow the array
            if (_n == _theObjects.Length)
            {
                T[] oldArray = _theObjects;
                _theObjects = new T[2 * oldArray.Length];
                Array.Copy(oldArray, _theObjects, _n);
            }

            _theObjects[_n] = o;
            _n++;
        }

        public T GetAt(int i)
        {
            return _theObjects[i];
        }

        public int Count
        {
            get { return _n; }
        }
    }
```

Statt der allgemeinen Klasse `object` wird nun der generische Parameter `T` verwendet (`object` wurde
durch `T`) ersetzt. Dieser Parameter wurde in der Generics-Parameter-Liste am Klassennamen 
in Spitzklammern "deklariert".

Erst bei der Verwendung der Collection-Klasse muss festgelegt werden, von welchem Typ die zu speichernden
Objekte sind. Hier ein Beispiel für einen Collection, der `int`-Werte enthält:

```C#
  MyCollection<int> container = new MyCollection<int>();
```
Man kann sich vorstellen, dass nun der Platzhalter `T` nun durch den Typ `int` ersetzt wird. In 
C++ funktionierten die ersten "Template"-Implementierungen tatsächlich durch einen Text-Ersatz im Source-Code.

In C# passiert unter der Haube mehr: Der generische Typ `MyCollection<T>` ist als solches Konstrukt auch 
im compilierten .NET-Code abgebildet. Zur Laufzeit wird dann durch die Spezialisierung auf `int` bei
der Verwendung der konkrete Typ `MyCollection<int>` erzeugt.

### Einschränkungen mit `where`

Manchmal werden an die als Parameter zu verwendenden Typen (also die Typen, die `T` ersetzen sollen)
bestimmte Anforderungen gestellt. Man stelle sich eine Collection-Klasse vor, die ihre Elemente in sortierter
Anordnung enthält.

Dazu muss dem Compiler mitgeteilt werden, dass nur bestimmte Kategorien von Typen für den Ersatz 
von `T` erlaubt sind, nämlich solche, die sich miteinander vergleichen lassen. Das geht mit der
Einschränkung mit dem Schlüsselwort `where`:

In folgendem Beispiel wird gefordert, dass der Datentyp, der für T verwendet wird, das Interface
`IComparable` implementieren muss:

```C#
    public class MyCollection<T> where T:IComparable
    {
        //...
    }
```

In der 
[C#-Sprach-Referenz](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/where-generic-type-constraint)
 sind sämtliche Möglichkeiten der Einschränkung beschrieben, die `where`
bietet, wie z.B:

- `T` muss eine Klasse sein
- `T` muss eine Struct sein
- `T` muss ein bestimmtes Interface implementieren oder von einer bestimmten Basisklasse erben
- `T` muss einen parameterlosen Konstruktor besitzen
- `T` muss ein nicht-nullable Typ sein

> #### 👨‍🔧 TODO
>
> - Erweitert die Collection-Klasse `MyCollection` so, dass bei Aufruf von `Add` das übergebene
>   Objekt sortiert in den Array eingefügt wird. Dazu muss:
>   - Die richtige Einfügestelle gefunden werden: Es muss dass neu hinzuzufügende Objekt
>     mit den bereits im Array stehenden Objekten verglichen werden. 
>   - Dazu muss `T` das Interface [`IComparable`](https://msdn.microsoft.com/de-de/library/system.icomparable(v=vs.110).aspx) implementieren. Eingebaute Typen wie int, string, double tun das bereits!
>   - Wurde die richtige Einfügestelle gefunden, muss Platz für das neue Objekt geschaffen werden, 
>     indem alle nachfolgenden Array-Einträge einen Platz nach hinten kopiert werden.
>   - Dann kann das Objekt eingefügt werden.

### Kleiner Ausflug: Ein möglicher Anwendungsfall: Mathematische Klassen

Mathematische Gebilde wie Körper, Ringe, 
[Vektorräume](https://de.wikipedia.org/wiki/Vektorraum)
etc. sind immer auf einem Grundtyp definiert. So gibt es z.B. Vektorräume über reellen Zahlen, über komplexen
Zahlen aber auch über Funktionen. Die Voraussetzung ist, dass auf den Grundmenge eine Addition und
eine Multiplikation definiert ist, also bestimmte Operationen vorhanden sind. 

Man könnte sich nun - analog zu den Collection-Typen - vorstellen, dass eine Klasse `Vektorraum` als 
Generic implementiert wird, und der als Grundtyp zu verwendende Typ zunächst als generischer Parameter
`T` deklariert wird, so dass im Nachhinein, der Vektorraum mit allen möglichen Grundtypen verwendbar ist:

```C#
public Vektorraum<T> where T: ...
```

Während dies in C++ sehr gut möglich ist und es ganze Bücher zu dem Thema "Implementierung Mathematischer
Konstrukte mit Templates" gibt, fehlt in C# (und in Java sowieso) eine nützliche  Voraussetzung:

Es ist weder mit `where` noch in Interfaces-Deklarationen möglich zu fordern, dass bestimmte Operatoren
(wie `+` und `*` deklariert sein müssen). Somit wird es schwierig die `...` in obiger Deklaration
mit Sinnvollem zu füllen. Die einzige Möglichkeit ist, ein Interface zu deklarieren, in dem 
Multiplikation und Addition nicht als Operatoren sondern als ganz normale Methoden deklariert sind.

Das macht aber Lesbarkeit von Ausdrücken, in denen diese Operationen vorkommen, schwierig. Da es in Java
die Möglichkeit überladener Operatoren sowieso gar nicht gibt, stellt sich hier dieses Problem erst gar nicht...

## Indexer

Arrays (als sehr simple Form von Collections) bieten einen direkten (wahlfreien) Zugriff auf einzelne
Elemente durch die bereits in die Sprache eingebaute Indizierungs-Schreibweise mit eckiger Klammer (`[]`).

In Collection-Klassen kann ein wahlfreier Zugriff zunächst über Methoden wie z.B. `SetAt(T o, int index)` oder
`T GetAt(int i)` realisiert werden

> #### 👨‍🔧 TODO
>
> - Erweitert die (nicht-sortiert speichernde) Klasse `MyCollection` um die 
>   Methode `SetAt(T o, int index)`. `GetAt` existiert ja bereits).

Um selbst definierten Collections, die mit einem Index einen wahlfreien Zugriff auf die enthaltenen Elemente
ermöglichen sollen, die gleiche elegante Eckige-Klammer-Syntax wie bei Arrays zu eröffnen, kann einer
Collection-Klasse ein Indexer hinzugefügt werden werden. Ein Indexer ist dabei eine spezieller Form einer
"Eigenschaft" (Property), also ein Klassen-Bestandteil, der nach außen aussieht wie ein Feld, aber 
eine Set- und eine Get-Methode deklariert, die bei Zuweisung oder beim Auslesen aufgerufen werden. 

Das [C#-Programmierhandbuch](https://docs.microsoft.com/de-de/dotnet/csharp/programming-guide/indexers/index)
enthält die allgemeine Deklaration eines Indexers - hier schon mit dem generischen Parameter T:

```C#
   public T this[int i]
   {
      get { return ...; }
      set { ... = value; }
   }
```

> #### 👨‍🔧 TODO
>
> - Erweitert die Klasse `MyCollection` um einen Indexer nach obigem Beispiel, der in `get` die Methode `GetAt()`
>   und in `set` die Methode `SetAt()` aufruft.
> - Ändert den Beispiel-Code in `main()` so ab, dass der Indexer sowohl lesend als auch schreibend verwendet 
>   wird.
> - Setzt Breakpoints in `SetAt()` und `GetAt()` um zu sehen, wie beim Zugriff über den Indexer diese
>   Methoden aufgerufen werden. 
> - Macht euch klar, dass der Datentyp `int` der hier als Index-Typ verwendet wird, auch durch andere Typen
>   implementiert werden kann. Somit sind assoziative Collection wie Dictionaries und Hash-Tables möglich,
>   die mit beliebigen Datentypen indizierbar sind.

## Enumerator

Mit dem Indexer lassen sich nun Collections bequem innerhalb von Schleifen beschreiben und auslesen. 
Dazu muss die Schleife mit einer Zählvariablen konstruiert worden sein:

```C#
  for (int i= 0; i < container.Count; i++)
  {
      // mach was mit container[i]
  }
``` 

Nicht immer sollen Collections aber wahlfreien Zugriff (über einen Index) bieten. Sehr oft genügt
es (oder es ist gar nicht anders möglich), sequenziellen Zugriff auf die Inhalte zu ermöglichen. 
Dabei können  Benutzer in einer von der Collection vorgegebenen Reihenfolge (und nicht in einer selbst
definierten Reihenfolge, wie sämtliche in der Collection
gespeicherten Inhalte in einer Schleife durchlaufen können.

Diese gegenüber dem wahlfreien Zugriff "abgespeckte" Form, kann in C# mit dem speziellen `foreach`-
Schleifenkonstrukt realisiert werden:

```C#
  foreach (var element in container)
  {
      // Mach was mit element
  }
``` 

Damit das funktioniert, muss die Collection-Klasse das spezielle Interface 
[`IEnumerable`](https://msdn.microsoft.com/de-de/library/9eekhta0(v=vs.110).aspx) 
implementieren.
Sie muss "enumerierbar" sein. Dieses Interface verlangt, dass eine einzige Methode implementiert wird, 
nämlich `GetEnumerator()`. Diese liefert ein Objekt zurück, dass wiederum ein Interface, nämlich
`IEnumerator` implementiert. Dieses Objekt kann man sich als "Ersatz" für die Zählvariable i vorstellen.

`IEnumerator` verlangt das Vorhandensein von drei Bestandteilen:

- Der Eigenschaft `Current` mit der auf das aktuelle Element zugegriffen werden kann.
- Der Methode `MoveNext()` mit der der Enumerator um ein Element weiter geschaltet werden kann.
- Der Methode `Reset()` mit der der Enumerator auf "Initialstellung" zurückgesetzt werden kann, 
  nämlich eine Position _vor_ dem ersten Element in der Collection.

Der Compiler generiert dann aus einer `foreach`-Schleife eine "ganz normale" `for` Schleife.
Die oben gezeigte `foreach`-Schleife ist somit nur syntaktischer Zucker der folgenden
Implementierung:

```C#

  for (var enumerator = container.GetEnumerator(); enumerator.MoveNext(); )
  {
      // Mach was mit enumerator.Current
  }
```

Der Umgang mit Enumeratoren ist also aus Anwendersicht durch die Compiler-Unterstützung
mit `foreach` sehr angenehm. Für Entwickler, die ihre selbst implementierte Collection-Klasse 
um die Enumerierbarkeit erweitern wollen, ist es ein mittlerer Alptraum, denn

- Sie müssen ihre Collection-Klasse das Interface `IEnumerable` implementieren lassen, d.h. die 
  Methode `GetEnumerator()` hinzufügen.
- Deren Rückgabewert muss ein neu zu erstellender Datentyp sein, der das Interface `IEnumerator` 
  implementiert. Dieser Datentyp muss implementiert werden. 
- Die Implementierung dieses Datentyps muss oben genannte Bestandteile `Current`, 
  `MoveNext()` und `Reset()` enthalten.
- Diese Methoden müssen irgendwie auf das ursprüngliche Collection-Objekt zugreifen.
- Zu allem Übel müssen die Interfaces `IEnumerable` und `IEnumerator` auch noch 
  in zwei Geschmacksrichtungen implementiert werden: Einmal ohne generischen Parameter 
  und einmal mit dem generischen Parameter unseres Inhaltstyps `T`.

Die gute Nachricht ist: neben der Compiler-Unterstützung für die _Anwendung_ von Enumerierbarkeit
(in Form der `foreach`-Schleife), gibt es auch Compiler-Unterstützung für die _Implementierung_
von Enumerierbarkeit: C# bietet das Schlüsselwort `yield`, bzw. `yield return` mit dem sich
so genannte Co-Routinen implementieren lassen. Kurz gesagt sind das Methoden,
die sich an einer bestimmten Stelle unterbrechen lassen und dann beim nächsten Aufruf
ihre Arbeit dort fortsetzen, wo sie unterbrochen wurden.

Statt also oben angegebene Liste abzuarbeiten, kann unsere Klasse `MyCollection` auch 
auf folgende Art enumerierbar gemacht werden

- Die Klasse `IEnumerable<T>` implementieren lassen:
  ```C#
    public class MyCollection<T> : IEnumerable<T>
    {
        ...
  ```
- Die Methoden `IEnumerator<T> GetEnumerator()` und `IEnumerator IEnumerable.GetEnumerator()` 
  implementieren (einmal generisch und einmal nicht generisch), dabei die nicht
  generische Version einfach durch einen Aufruf der generischen implementieren:
  ```C#
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
  ```
- Die generische Methode `IEnumerator<T> GetEnumerator()` kann nun, statt wie in der Deklaration
  gefordert, ein Objekt vom Typ `IEnumerator<T>` zurückzuliefern, die eigentliche Traversierung
  der einzelnen Objekte direkt als Co-Routine implementieren und dabei das aktuelle Objekt
  jeweils mit `yield return` zurückliefern:

  ```C#
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _n; i++)
        {
            yield return _theObjects[i];
        }
    }  
  ```

> #### 👨‍🔧 TODO
>
> - Erweitert `MyCollection` um die Enumerierbarkeit und verwendet eine
>   `foreach`-Schleife um auf die Inhalte zuzugreifen.

### Wo geht `yield` nicht/schlecht?

Leider bietet die komfortable Möglichkeit, mit `yield return` Enumerierbarkeit zu implementieren,
nicht bei allen Formen der internen Speicherung von Element-Daten eine gute Lösung.

Ein Beispiel hierfür ist die Speicherung in Bäumen: Werden Elemente z.B. in einem 
binären (oder auch anders strukturierten) Baum gehalten, gibt es irgendwo eine 
rekursive Struktur (Knoten im Baum können wiederum Knoten oder Listen von Knoten enthalten)

Es gibt zwar grundsätzlich die Möglichkeit, mit `yield return` auch über mehrere
Rekursionsstufen hinweg zu traversieren, dies führt aber zu wenig performantem Code.
Leider ist die direkte Implementierung eines Enumerators noch komplexer, da hier 
überhaupt nicht mit Rekursion gearbeitet werden kann. Stattdessen muss der aktuelle 
Traversierungs-Stand (welcher Knoten in welcher Hierarchiestufe wird gerade traversiert)
explizit z.B. in einer Stack-artigen Struktur gehalten werden.


## Anwendungsbeispiele in FUSEE

Viele Beispiele für oben genannte Anwendungen finden sich in FUSEE rund um 
das Traversieren von Szenengraphen. Zunächst muss beim Traversieren oft Informationen
in Stack-Daten gehalten werden, da beim Traversieren eines Szenengraphen über 
Hierarchie-Ebenen hinweg Informationen zu jeder einzelnen gerade besuchten 
Hierarchie-Ebene gehalten werden muss. 

Hierzu gibt es folgende Collection-Klassen:

- [`StateStack<T>`](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Xene/VisitorState.cs#L42)
  kann Instanzen beliebiger Typen als Stack speichern. Die Operation
  `Push()` kopiert den Inhalt des _Top-of-Stack_ und lässt somit den Stack um eine
  Ebene wachsen. Die Operation `Pop()` löscht den _Top-of-Stack_ und kehrt zum vorigen
  _Top-of-Stack_ zurück. Die Eigenschaft `Tos` erlaubt lesenden und schreibenden Zugriff
  auf den _Top-of-Stack_.

- [`CollapsingStateStack<T>`](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Xene/VisitorState.cs#L131)
  bietet die gleichen Optionen wie `StateStack<T>`. Diese Implementierung eignet sich allerdings
  für Daten-Elemente mit hohem Datenvolumen, bei denen der Kopieraufwand vergleichsweise hoch ist, bei
  gleichzeitigem Einsatz in Situationen, in denen nicht in jeder Hierarchiestufe der Top-of-Stack verändert
  wird. Bei Folgen von `Push()`-Operationen wird zunächst Buch geführt, wie oft `Push()` aufgerufen
  wurde, bevor der `Tos` verändert wurde. Erst dann wird eine physische Kopie angelegt. Bei `Pop()`
  Operationen wird symmetrisch verfahren, so dass Kopien nur dann angelegt werden, wenn sie wirklich
  notwendig sind.

Das Traversieren eines Szenengraphen selbst kann auch zum Ziel haben, dass eine Reihe von 
Ergebnissen als Enumeration zurückgeliefert werden sollen. Die Basisklasse `SceneVisitor` verwendet
für den Einsatz als klassisches Visitor-Pattern eine rekursive Traversierung. 

Soll allerdings während der Traversierung an bestimmten Nodes oder Komponenten enumerierbare Elemente
zurückgegeben werden, kann nicht klassisches rekursives Visitor-Pattern betrieben werden.
Hierzu implementiert die `Visitor`-Klasse Bausteine, mit denen sich  
die für das Interface `IEnumerator<T>` notwendigen Bestandteile implementieren lassen. Diese 
Implementierung kann dann an beliebigen Stellen während der Traversierung unterbrochen werden
und durch Wiedereinstieg (re-entrant fähig) an exakt der Stelle weiter traversieren, wo zuvor 
die Traversierung unterbrochen wurde.
 
Diese Bausteine sind

- [`EnumInit()`](https://github.com/FUSEEProjectTeam/Fusee/blob/master/src/Xene/Visitor.cs#L226).
  Initialisiert die nicht-rekursive re-entrant-fähige Enumerierung
- [`EnumMoveNext()`](https://github.com/FUSEEProjectTeam/Fusee/blob/master/src/Xene/Visitor.cs#L256).
  Kernstück der nicht-rekursive re-entrant-fähigen Enumerierung. Hält den aktuellen Traversierungszustand
  (Welche Node in welcher Hierarchiestufe und welche Komponente) in geeigneten Datenstrukturen und bewegt
  den Traversierungszusatand zum jeweils nächsten Objekt (Node oder Component) im Szenengraphen.

Ein Beispiel für die Anwendung einer solchen Traversierung ist das Picking. Der Szenengraph soll traversiert
werden und während der Traversierung sollals Resultat eine Enumeration von 
[`PickResult`](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Engine/Core/ScenePicker.cs#L13)-Objekten 
entstehen (die auch gleich während der Traversierung enumerierbar sein soll).

Hierzu gibt es die von `Viserator` abgeleitete Klasse 
[`ScenePicker`](https://github.com/FUSEEProjectTeam/Fusee/blob/develop/src/Engine/Core/ScenePicker.cs#L155),
die als solche das `IEnumerator<PickResult>` Interface implementiert und für die dort geforderten
Eigenschaften `Current`, `Reset()` und `MoveNext()` oben genannte Implementierungen verwendet.

