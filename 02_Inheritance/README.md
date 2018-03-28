# Lektion 02 class vs. interface, what about struct

## Weitere Unterschiede zwischen class und struct

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
besonders interessant. Mit Vererbung können unterschiedliche objektorientierte 

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

public B : A
{
    public string AnotherString;
}
```

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

> **TODO**
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

## Interfaces

Existieren in einer Klasse ausschließlich abstrakte Bestandteile, kann statt der Deklaration mit `class` 
eine Deklaration mit `interface` erfolgen. Interfaces haben gegenüber Klassen ein paar markante Unterschiede:

- Eine Klasse kann _mehrere_ Interfaces implementieren, aber nur von _einer_ anderen Klasse erben
- Auch `struct`s können Interfaces implementieren.
- Bestandteile (Methoden) eines Interfaces sind automatisch `virtual`.

