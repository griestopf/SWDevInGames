# Lektion 01 Memory Layout, value vs. reference types, virtual Methoden 

## Memory Layout

### Value-Variablen vs. Reference-Variablen

Value-Variablen enthalten direkt den Wert, während Reference-Variablen eine Referenz auf die Speicherstelle
enthalten, an der der Wert gespeichert wurde. 

Grundlegende Unterschiede 

Aktion            | Beispielcode      |  by Value                             |  by Reference
------------------|-------------------|---------------------------------------|---------------------------------------
Objekterzeugung   | `a = new T()`     | a ist untrennbar mit Objektverbunden | ein neues Objekt wird irgendwo im Speicher erzeugt und a erhält eine Referenz auf dieses Objekte 
Zuweisung     | `a = b`           | Wert wird kopiert               | Referenz wird kopiert, Objekt existiert nach wie vor nur ein mal
Vergleich     | `a == b`          | Wert wird verglichen            | Meist: Referenz wird verglichen. `true` nur bei "ist das selbe Objekt" und nicht bei "hat den gleichen Wert".
Objektzugriff | `a.xyz`           | Änderungen von Eigenschaften / Seiteneffekte wirken sich nur auf `a` aus. | Änderungen von Eigenschaften / Seiteneffekte wirken sich das durch  `a` referenzierte Objekt aus und auf alle anderen Variablen, die das selbe Objekt referenzieren.

Zum Verständnis von Code ist es hilfreich, sich das Speicherlayout schematisch zu visualisieren, z.B. während einer
Debug-Session. Hier eine Herangehensweise:

Zweispaltige Tabelle: Linke Spalte: Identifizierer (Variablen-, Klassenvariablen- od. Parametername), Rechte Spalte: Bei Value-Variablen: Wert, bei Reference-Variablen: Zeiger auf Objekt.

**Beispiel**

Folgender Code

```C#
struct Pair { int one; int two }
class Cat { string name; int age }
...
float a = 2;
Pair p = new Pair {one = 47, two = 11};
Cat c = new Cat { name = "Whiskers", age=5 };

Pair q = p;
Cat d = c;
```

führt zu folgendem Memorylayout:

![Memory Layout](_images/MemLayout.png)

> **TODO**
> - Zeichnet ins Layout ein, was bei folgender Anweisung passiert:
>   ```C#
>     p.one = 48;
>     c.age = 6;
>   ```
> - Wie ändern sich die Werte von `q.one` und `d.age`? 

Bei Arrays, Listen oder anderen Containern wird ebenfalls ein neuer Speicherbereich angelegt, dessen linke Spalte
durch den Index (oder das indizierende Objekt) und die rechte durch die Nutzdaten belegt werden.

> **TODO**
> - Legt jeweils einen Array von `Cat`s und `Pair`s an und zeichnet das Memory-Layout.

In C# hängt es vom Datentyp ab, ob eine Variable by Value oder by Reference gehandhabt wird:

**Value-Typen**

- Alle eingebauten Typen _außer_ `string`: 
  - `bool`
  - `int`, `uint`, `short`, `ushort`, `long`, `ulong`
  - `double`, `float`, `decimal`
- Alle als `struct` selbst definierten Typen

**Reference-Typen**
- `string`, mit "Value-Semantik" beim Vergleich (`==`)
- Alle als `class` selbst definierten Typen
- Array-Typen (der Array selbst, nicht notwendigerweise die darin enthaltenen Daten)
- Delegate-Typen

### Andere Programmiersprachen

C/C++: Bei jeder Variable kann der Benutzer des Typs entscheiden, ob sie "by Value" oder "by Reference" 
verwendet werden soll.

JavaScript: Alle Typen sind (dynamische) Referenz-Typen, für eingebaute Typen existiert eine spezielle Operatoren-Semantik,
die sich wie Value-Typen anfühlen.

Java: Eingebaute Typen außer `String` sind Value-Typen, alles andere sind Referenz-Typen. Es gibt keine Möglichkeit,
eigene Datentypen "by Value" zu verwenden.


## Struct vs. Class

Wann sollte nun ein selbst definierter Typ als `class`, also "by Reference" und wann als `struct`, also "by Value"
definiert werden?

Faustregel: 

- Objekte, die eher als Datencontainer fungieren, die wenig Speicher verwenden und von denen 
  große Mengen, z.B. in Arrays oder Listen angelegt werden, eher als `struct`. Bei C# `struct`s ist
  Vererbung NICHT erlaubt.

- Objekte, mit vielen Methoden und viel Intelligenz, die ggf. viel Speicher verwenden eher als `class`. 
  Vererbung funktioniert nur mit `class`.

Weiterführende Information u.A. im Artikel 
["Choosing Between Class and Struct" auf MSDN](https://msdn.microsoft.com/en-us/library/ms229017(v=vs.110).aspx).

> #### TODO
>
> - Betrachtet die [mathematischen Typen in FUSEE](https://github.com/FUSEEProjectTeam/Fusee/tree/develop/src/Math/Core)
>   wie z.B: `float3`, `float4x4`.
> - Warum sind diese als `struct` angelegt?
> - Betrachtet folgenden Typ
>   ```C#
>    public struct Vertex
>    {
>       public float3 Position;
>       public float3 Normal;
>       public float3 UVW;
>    }
>
>   ``` 
> - Schreibt Beispielcode, der einen großen Array von solchen `Vertex`-Objekten  (z.B. 1 Million Objekte) anlegt,
>   in einer Schleife mit Zufallswerten beschreibt und in einer zweiten Schleife die Werte ausliest. Messt die Zeit,
>   die hierzu benötigt wird.
> - Schreibt eigene **Klassen** `Cfloat3` und `CVertex` (mit `class` statt `struct`) und verwendet diese in o.a. 
>   Test. Vergleicht die gemessenen Zeiten und erklärt die Unterschiede.
> - Wie sähe in beiden Fällen jeweils das Memory-Layout aus (natürlich nicht für 1 Mio. Objekte aufmalen :-))?


## Struct & Class vermischt

Es gibt ein paar Überschneidungen zwischen mit `struct` und `class` definierten Typen, die
selten vorkommen, aber dann zu Verwirrung führen. Ohne allzutief darauf einzugehen, hier
ein paar Sachverhalte:

### Boxing und Unboxing

In C# erben _alle_  Typen vom gemeinsamen Basistyp `object`, der als `class` deklariert ist.
Das gilt allerdings auch mit `struct` definierte und andere Value-Typen.

Durch diese Tatsache ist aber z.B. auch ein `int` Wert eine Instanz vom Typ `object` und kann z.B. 
einer Variablen vom Typ `object` übergeben werden. Dazu muss aber eine Referenz auf das `int`-Objekt
angelegt werden, was widersprüchlich ist, weil `int` als Value-Typ keine Referenzen erlaubt.

Folgender Beispielcode ist gültiger C#-Code:

```C#
  int i = 42;
  object o = i;
  o = 43;
  int j = (int)o;
  Console.WriteLine("i is: " + i + "; o is: " + o + "; j is: " + j);
```
> #### TODO
>
>
> - Führt o.s. Code aus.
> - Lest den Abschnitt zu [Boxing und Unboxing im C# Programmierhandbuch](https://docs.microsoft.com/de-de/dotnet/csharp/programming-guide/types/boxing-and-unboxing)
> - Zeichnet ein Memory-Diagramm vom Zustand nach der Ausführung des o.s. Code.

### Überschreiben von Methoden in `struct`s

Noch eigentümlicher wird die Tatsache, dass jeder mit  `struct` deklarierte Typ von `object`
erbt, wenn man weiß (wie im folgenden Kapitel beschrieben), dass `struct` Vererbung gar
nicht unterstützt. Streng genommen gilt, dass genau diese Vererbung erlaubt ist, weil sie
automatisch vom Compiler erzeugt wird, allerdings darf ein Value-Typ nie als _Basisklasse_
für weitere Typen zur Verfügung stehen. Es kann allerdings passieren und es gibt sinnvolle
Fälle, in denen in einem `struct` einige der in `object` deklarierten Methoden überschreiben
werden. 

> #### TODO
>
> - Lest die Referenz-Dokumentation von 
>   [`ValueType.Equals`](https://msdn.microsoft.com/de-de/library/2dts52z7(v=vs.110).aspx)
>   und versucht zu verstehen, warum der _Tipp_ rät, u.U. diese Methode in eigenen `struct`s zu 
>   überschreiben.

## Further Reading

- [Eric Lippert on Stack and Heap vs Value and Reference](https://blogs.msdn.microsoft.com/ericlippert/2009/04/27/the-stack-is-an-implementation-detail-part-one/)








