# Lektion 01 Memory Layout, value vs. reference types, virtual Methoden 

## Memory Layout

### Value-Variablen vs. Reference-Variablen

Value-Variablen enthalten direkt den Wert, während Reference-Variablen eine Referenz auf die Speicherstelle
enthalten, an der der Wert gespeichert wurde. 

Grundlegede Unterschiede 

Aktion            | Beispielcode      |  by Value                             |  by Reference
------------------|-------------------|---------------------------------------|---------------------------------------
Objekterzeugung   | `a = new T()`     | a ist untrennbar mit Objektverbunden | ein neues Objekt wird irgendwo im Speicher erzeugt und a erhält eine Referenz auf dieses Objketn 
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

JavaScript: Alle Typen sind (dynamische) Referenz-Typen, für eingebaute Typen exisit eine spezielle Operatoren-Semantik,
die sich wie Value-Typen anfühlen.

Java: Eingebaute Typen außer `String` sind Value-Typen, alles andere sind Referenz-Typen. Es gibt keine Möglickeit,
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

> **TODO**
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


## Further Reading

- [Eric Lippert on Stack and Heap vs Value and Reference](https://blogs.msdn.microsoft.com/ericlippert/2009/04/27/the-stack-is-an-implementation-detail-part-one/)






