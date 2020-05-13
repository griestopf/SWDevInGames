# Lektion 02 Classes, Structs, Interfaces

## Weitere Unterschiede zwischen `class` und `struct`

Der bedeutendste Unterschied zwischen `class` und `struct` ist sicherlich, dass 
mit `class` deklarierte Typen Referenz-Typen sind und mit `struct` deklarierte
Typen Werte-Typen.

Es existieren eine Reihe von weiteren Unterschieden. Manche davon sind direkte
Folgen der Werte-/Referenz-Typ Kategorisierung. Andere Unterschiede sind
bewusst von den Designern der Programmiersprache so gewählt worden.

Eigenschaft                  |  `class`   |  `struct` | 
-----------------------------|:----------:|:---------:|
Vererbung                    | ✓         | -         |
Mehrfach-Referenzierung      | ✓         | -         |
Variablen können `null` sein | ✓         | -         |
`virtual` Methoden           | ✓         | -         |
Können Interfaces implementieren| ✓         | ✓      |
Können Methoden enthalten    | ✓         | ✓      |

## Vererbung

Die Möglichkeit der Vererbung macht Klassen in einigen Anwendungsfällen
besonders interessant. Mit Vererbung können unterschiedliche 
[objektorientierte Prinzipien](https://de.wikipedia.org/wiki/Prinzipien_objektorientierten_Designs)
realisiert werden

## Vererbung zur Erweiterung

Ein Typ A besitzt bereits eine Reihe von Eigenschaften (Methoden, Fields und Properties).
Um weitere Eigenschaften zu implementieren, ohne den ursprünglichen Typ A zu verändern,
kann ein weiterer Typ B implementiert werden, der von A erbt.

```C#
public class A
{
    public string SomeString;
    public int SomeInt;
}

public class B : A
{
    public string AnotherString;
}
```

Klasse `B` besitzt somit alle Eigenschaften, die `A` hat.

## Vererbung zur Polymorphie

Ein Typ A enthält bereits eine Reihe von Eigenschaften (u.a. Methoden). Eine vererbte Klasse B soll
die meisten Eigenschaften erben, allerdings müssen die Implementierungen einiger Methoden geändert werden.

Die Methoden selbst sollen allerdings unter gleichem Namen und gleicher Signatur wie bisher aufgerufen
werden können, um die neue Klasse B im selben Kontext wie die Original-Klasse A verwenden zu können.

### Virtual Methoden / Late Binding

In C# können Methoden mit dem reservierten Wort `virtual` versehen werden. Damit können diese in vererbten Typen
überschrieben werden (`mittels override`. C# implementiert auf diese Weise Polymorphie. Es steht erst zur 
Laufzeit fest, welche Methode aufgerufen wird (die der Basisklasse oder die der geerbten Klasse). Diese hängt vom 
Typ des Objektes ab.

Bei Methoden, die nicht `virtual` sind, wird zur Compile-Zeit festgelegt, welche Methode aufgerufen wird. Hier hängt es
vom Typ der Variablen (der Referenz) ab, welche Methode aufgerufen wird.

> #### 👨‍🔧 TODO
>
> - Erzeugt eine Klasse `A`  mit einer Methode `DoSomething()` (NICHT virtual) und eine Klasse `B`, die von `A` erbt.
> - Überschreibt `DoSomething()` in `B`.
> - Erzeugt eine Referenz auf ein `A`-Objekt aber initialisiert diese mit einem `B`-Objekt und ruft die Methode auf:
>   ```C#
>   A someA = new B();
>   someA.DoSomething();
>   ```
> - Welche Methode wird aufgerufen: `A.DoSomething()` oder `B.DoSomething()`?

Um im Falle von `virtual` Methoden zur Laufzeit entscheiden zu können, welche Methode aufgerufen wird, erzeugt der
Compiler an jedem Objekt eine so genannte "Virtual Function Table". Hier sind Verweise (Referenzen) auf die Methode 
(siehe Delegates), die dann aufzurufen ist. Man kann virtuelle Methoden als eigene Daten-Einträge in der 
oben eingeführten Memory-Layout-Notation eintragen.

## Implementierungsloser Elterntyp 

Manchmal kann es sein, dass in einer Basisklasse bereits festgelegt sein soll, dass eine bestimmte Methode
existiert, allerdings ist es in der Basisklasse nicht sinnvoll, bereits eine Implementierung anzugeben. Dann
kann eine Rumpf-lose Methodendeklaration angegeben werden, die mit dem Schlüsselwort `abstract` dekoriert 
wird. Das geht allerdings nur in Klassen, die ebenfalls mit `abstract` dekoriert wurden. Abstrakte Methoden 
werden deklariert, um einen "Vertrag" zwischen Aufrufer und Implementierer zu schließen: Die Deklaration
einer Abstrakten Methode garantiert dem Anwender der Klasse, dass er eine Instanz eines Typs hat, der diese
Methode implementiert - daher kann sie aufgerufen werden. 

> #### 👨‍🔧 TODO
>
> - Erzeugt eine Basisklasse `Shape2D` und fügt dieser ein paar allgemeine Eigenschaften hinzu
>   (z.B. einen Namen).
> - Fügt der Basisklasse eine abstrakte Methode `double CalculateArea()` hinzu.
> - Erzeugt die Klassen `Rect` und `Circle`. Implementiert sinnvolle Eigenschaften (Länge und Breite bei `Rect`. 
>   `Radius` bei `Circle`) und überschreibt jeweils die Methode `CalculateArea`.
> - Referenziert jeweils eine Instanz von `Rect` und `Circle` über eine Referenz vom Typ `Shape2D`,
>   ruft über diese Referenz die polymorphe (=vielgestaltige) Methode `CalculateArea` auf. Überzeugt
>   Euch, dass jeweils die "passende" Methode aufgerufen wird.

## Interfaces

Existieren in einer Klasse ausschließlich abstrakte Bestandteile, kann statt der Deklaration mit `class` 
eine Deklaration mit `interface` erfolgen. Interfaces haben gegenüber Klassen ein paar markante Unterschiede:

- Eine Klasse kann _mehrere_ Interfaces implementieren, aber nur von _einer_ anderen Klasse erben
- Auch `struct`s können Interfaces implementieren.
- Bestandteile (Methoden) eines Interfaces sind automatisch `virtual`.

> #### 👨‍🔧 TODO
>
> - Wandelt oben implementierten abstrakten Typ `Shape2D` in ein Interface um.

## Pattern Matching

Oft kommt es vor, dass man Klassen verwendet, deren Deklaration/Implementierung man nicht ändern kann, weil man eine bereits kompilierte DLL verwendet (die z. B. über [NuGet](https://www.nuget.org/) eingebunden wird). Soll eigener Code dann polymorph, also abhängig vom konkreten Typ eines Objektes, ausgeführt werden, kann man nicht einfach eine `virtual`-Methode oder ein gemeinsames Interface hinzufügen. Für diesen Anwendungsfall gibt es in C# seit Version 7.0 eine mächtige Erweiterung der `switch/case`-Anweisung, mit der man objekte auf vielerlei Bedingungen, u. a. darauf, ob sie von einem bestimmten Datentyp abgeleitet sind, überprüfen kann: Das so genannte [Pattern Matching](https://docs.microsoft.com/en-us/dotnet/csharp/pattern-matching).

Der Beispielcode im Projekt [InterfaceVsPattern](InterfaceVsPattern/Program.cs) stellt die Ansätze "Polymorphie durch Interface" und "Polymorphie mit Pattern Matching" gegenüber.

> #### 👨‍🔧 TODO
>
> - Erzeugt zwei oder mehrere konkrete Klassen, die von einer gemeinsamen Basisklasse ableiten, in der eine Methode polymorph mit `virtual` implementiert wurde. 
> - Ruft die Methode auf unterschiedlichen Instanzen sehr oft auf und messt die Zeit.
> - Implementiert den polymorphen Aufruf statt mit `virtual` mit Pattern Matching. Wie verhält sich dann die Laufzeit? Erklärt einen ggf. vorhandenen Unterschied.


## Further Reading

- In der Community wird oft darüber gerätselt, warum von `struct`s nicht geerbt werden kann 
  (sie sind _sealed_),
  insbesondere, wenn `struct`s  doch aber `interface`s implementieren können. Hier ist eine detaillierte 
  [Erklärung von Konrad Rudolph](https://stackoverflow.com/questions/1769306/why-are-net-value-types-sealed/1769336#1769336),
  die mit Beispielen verdeutlicht, was passieren würde, wenn die C#-Designer Vererbung bei `struct`s 
  zugelassen hätten.

- Mit der Version 8 der Sprache C# wurden [Default Interface Methoden](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#default-interface-methods) eingeführt. Welchen Vorteil bieten diese gegenüber virtuellen Methoden in gemeinsamen Basisklassen? 



