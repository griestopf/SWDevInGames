# Lektion 05 Reflection, Decompiler, Attribute, Dependency Injection

## Reflection, Definition

Bei "klassischen" Compilern, die aus einer Programmiersprache direkt in eine Maschinensprache
übersetzen, fallen während des Übersetzungsvorgangs viele programmiersprachliche Konstrukte weg.
Klassen, Interfaces,  Strukturen existieren im erzeugten Compilat ebensowenig wie
Hochsprachen-Kontroll-Anweisungen wie z.B. `if`-`else`, `for` oder `while`, stattdessen gibt es nur (bedingte) Sprunganweisungen. Aus diesem Grund
genügt es z.B. in C/C++ nicht, nur den kompilierten Code in Form von Libraries 
(DLLs, Archives, Libs, o.ä.) weiter zu geben. Zusätzlich müssen strukturelle Deklarationen wie 
Klassen oder Structs im Source-Code (in so genannten Header-Dateien (.h)) weiter gegeben werden.

In _managed_ Umgebungen wie Java oder C#/.NET sind auch hochsprachliche strukturelle deklarative
Informationen im erzeugten Kompilat (in .NET in einer_Assembly_, d.h. eine _managed DLL_ oder
_managed EXE_) enthalten.
Somit kann ein Compiler beim Compilieren von Source-Code, der auf bereits compilierten Code in einer
Library zugreift, alle zum Compilieren notwendigen Informationen (wie z.B. Klassen, Methoden,
Signaturen) aus der bereits kompilierten Library extrahieren.

Diese Informationen über in einer Assembly enthaltene Datentypen oder über die in einem Datentyp
enthaltenen Bestandteile können auch zur Laufzeit per Code abgefragt und ausgewertet werden.
Derartige _Meta_-Informationen über Programmcode und der Zugriff darauf zur Laufzeit wird 
***Reflection*** genannt.

Folgende Funktionalität steht Programmieren in einer Programmiersprache/-Umgebung, die Reflection ermöglicht, zur Verfügung:

- Compilezeit-Informationen zur Laufzeit auslesen
  - Welche Namespaces & Typen (Klassen, Strukturen, Enums) sind in 
    einem   Assembly (dll, exe) enthalten?
  - Welche Fields, Properties und Methoden sind in einer 
    Klasse / Struct enthalten?
  - Von welchem Typ ist ein Objekt 
  
- Compilezeit-Aktionen zur Laufzeit ausführen 
  - Instanzen von Typen erzeugen
  - Typisierte Instanzen von Generics erzeugen
  - Properties/Fields beschreiben und auslesen
  - Methoden aufrufen

## Reflection in C# verwenden

Die Reflection-Funktioinalität der .NET-Plattform steht im Namespace
[`System.Reflection`](https://msdn.microsoft.com/en-us/library/system.reflection(v=vs.110).aspx)
zur Verfügung. Der wichtigste Datentyp für den 
Umgang mit Reflection ist allerdings der zum `System`-Namespace
ist die Klasse 
[`System.Type`](https://msdn.microsoft.com/en-us/library/system.type(v=vs.110).aspx). 
Eine Instanz (ein Objekt) vom Typ `Type` bildet eine Sammlung von
zur Laufzeit abfragbaren Meta-Informationen über einen Datentyp.

> #### TODO
>
> - Schaut Euch die Referenzdokumentation zu [`System.Type`](https://msdn.microsoft.com/en-us/library/system.type(v=vs.110).aspx)
>   an und beantwortet folgende Fragen:
>   - Wie bekommt man eine Liste der in einer Klasse definierten Methoden?
>   - Wie bekommt man eine Liste der in einer Klasse definierten Felder /
>     Eigenschaften?

### Einstieg

Um in eigenem Programmcode einen Einstieg in Reflection zu bekommen, 
benötigt man eine Instanz vom Typ `Type` die für den Datentyp, an dem 
man interessiert ist, beschreibende Metainformationen enthält. 

Es gibt zwei grundlegende Möglichkeiten, an Instanzen von `Type` zukommen.

1. Jedes beliebige Objekt in C# kann eine Instanz von `Type` liefern,
   die den Typ des Objektes beschreibt: Auf dem Datentyp `object`
   (der _Mutter aller Typen_) ist die Methode `Type GetType()` 
   deklariert.

2. Eine Assembly (DLL, EXE) kann zur Laufzeit geladen werden und
   dann per Reflection inspiziert werden, indem eine Liste der
   in der Assembly definierten Typen angefordert wird:
   - Assembly Laden: 
     [`Assembly LoadFrom(string path)`](https://msdn.microsoft.com/en-us/library/1009fa28(v=vs.110).aspx)
   - Liste von Typen der Assembly: 
     [`Type[] GetTypes()`](https://msdn.microsoft.com/en-us/library/system.reflection.assembly.gettypes(v=vs.110).aspx)

> #### TODO
>
> - Ruft auf Objekten von primitiven Datentypen (`int`, `string`)
>   die Methode `GetType()` auf und inspiziert den Inhalt der jeweils
>   zurückgegebenen Typ-Instanz mit dem Debugger oder gebt Informationen,
>   die von der Instanz abrufbar sind, per `WriteLine` aus.
> - Probiert das Ganze mit selbst definierten Datentypen. Nicht nur 
>   mit `class` und `struct`, sondern auch mit `enum` und `delegate`.
> - Schreibt eine Beispiel-Assembly (DLL oder EXE), in der ein oder 
>   mehrere Datentypen implementiert werden. Erzeugt ein weiteres 
>   (Konsolen-)Projekt, in dem Ihr die Beispiel-Assembly mit `LoadFrom`
>   (s.o) ladet und die enthaltenen Typen (`GetTypes`) abfragt und
>   von diesen ein paar Eigenschafte (z.B. Name) ausgebt. Tauscht die 
>   Beispiel-Assemblies _ohne_ Source-Code untereinander aus.

### Auslesen / Traversieren des Typsystems

Ausgehend vom `Type`-Objekt, das einen bestimmten Typ beschreibt, können sämtliche Eigenschaften abgefragt werden. Es ergibt sich eine Art Graph/Baum, in dem verwendete Typen, Methoden, Eigenschaften etc. als Knoten auftauchen. 

Beispiel: Eine Klasse enthält eine Methode, diese einen Parameter, dieser ist wiederum von einem anderen Typ, dieser wiederum enthält eine Eigenschaft wiederum eines anderen Typs etc.

Im Folgenden sind die beschreibende Strukturen des Reflection-Systems und deren 
jeweilige Eigenschaften/Get-Methoden aufgelistet, mit denen sich entlang des 
Typ-System-Graphen traversieren lässt.

- `Type`	
  - `FieldInfo[] GetFields()`  /  `FieldInfo GetField(string Name)`
  - `PropertyInfo[] GetProperties()`  / `PropertyInfo GetProperty(string Name)`
  - `MethodInfo[] GetMethods`  / `MethodInfo GetMethod(string Name)`
- `FieldInfo` / `PropertyInfo`
  - `string Name`
  - `Type FieldType`
- `MethodInfo`
  - `string Name`
  - `Type ReturnType`
  - `ParameterInfo[] GetParameters()`
  - `ParameterInfo`
  - `Type ParameterType`

> #### TODO
>
> - Schreibt eine Methode `PrintTypeInfo(object o)`, die zu einem übergebenen Objekt
>   dessen komplette Typ-Informationen ausgibt, also u.A.
> - Ist es ein Delegate, ein Enum, eine Klasse oder ein Struct?
> - Falls Klasse oder Struct
>   - Alle `public` Methoden inkl Signaturen (Rückgabewert und Parameterliste)
>   - Alle `public` Properties und Felder inkl. deren jeweilige Typen.

### Über Reflection Aktionen ausführen

Neben dem reinen "Reflektieren", also Beschreiben, von Typinformationen können per
Reflection auch Aktionen ausgelöst werden. Es kann also aktiv mit den Typen gearbeitet 
werden, die zur Compilezeit gar nicht bekannt waren, und die zur Laufzeit nur über deren
beschreibede Strukturen bekannt sind. Folgende Aktionen sind möglich:

#### Objekte instanziieren
Mit der Methode `ConstructorInfo GetConstructor(Type[] paramTypes)` von `Type` kann ein
passenden Konstruktor gefunden werden. Mit der Methode `object Invoke(object[] parameters)`
der Klasse `ConstructorInfo`
kann dieser dann ausgeführt werden. Der Typ des zurückgegebenen Objektes entspricht dann
dem im ursprünglichen `Type` Objekt reflektierten Typ.

#### Fields/Properties lesen & beschreiben
Mit `object GetValue(object ob)`, deklariert sowohl in `PropertyInfo` als auch in `FieldInfo`
können die Inhalte von Properties oder Felder ausgelesen werden. Dabei ist das Objekt, 
auf dem das jeweilige Property deklariert ist, als Parameter zu übergeben.

Analog kann mit `void SetValue(object ob, object val)` der Wert eines Property oder eines Field
beschrieben werden. Auch hier ist das Objekt, das das zu beschreibende Element enthält,
als erster Parameter anzugeben, während die Methode `SetValue` auf dem reflektierenden
Objekt des Field oder Property aufzurufen ist. Der neu zu setzende Wert ist der zweite
Parameter.

#### Methoden aufrufen
Die Methode `object Invoke(object ob, object[] parameters)` auf Objekten vom Typ `MethodInfo`
erlaubt es, die reflektierte Methode auf dem als ersten Parameter zu übergebenden Objekt
aufzurufen. Die Parameter, die an die Methode übergeben werden sollen, müssen in den `object`-
Array (zweiter Parameter) verpackt werden.

> #### TODO
>
> - Experimentiert mit per Reflection ausgelösten Methoden-Aufrufen und Wert-Zuweisungen an 
>   Felder/Properties. Erweitert z.B. die Methode `PrintTypeInfo(object o)` so, dass
>   sie alle öffentlichen Felder und Properties des übergebenen ausliest und deren Inhalte
>   mit ausgibt.

### Typ-Überprüfungen

Oft soll überprüft werden, ob Objekte oder Typen miteinander in
einer Vererbungs-/Implementierungs-Beziehung stehen. Das geht zwar über Methoden
der Klasse `Type`, z.B. mit der 
[`Type BaseType`](https://msdn.microsoft.com/en-us/library/system.type.basetype(v=vs.110).aspx) 
Eigenschaft und der
[`Type[] GetInterfaces()`](https://msdn.microsoft.com/en-us/library/system.type.getinterfaces(v=vs.110).aspx) 
Methode, erfordert aber u.U. mehrere Zeilen Code, um in der Hierarchie zu traversieren 
oder in der Liste der Interfaces zu iterieren. Um direkt zu überprüfen, ob ein Typ von 
einem anderen abgeleitet ist oder ein bestimmtes Interface implementiert, können folgende
Mechanismen verwendet werden:

- [`is` Operator](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/is).
  Kann verwendet werden, wenn ein konkretes Objekt darauf überprüft
  werden soll, ob es von einem zur Compilezeit bekannten Typ erbt, ist oder implementiert.
  Beispiel: 

  ```C# 
  object a = ...;
  if (a is SomeType)
  {
    Console.WriteLine("o ist vom Typ SomeType");
  }
  ```

- [`Type.IsInstanceOfType()` Methode](https://msdn.microsoft.com/en-us/library/system.type.getinterfaces(v=vs.110).aspx).
  Kann verwendet werden, wenn ein konkretes Objekt darauf 
  überprüft werden soll, ob es von einem mit einer `Type`-Instanz beschriebenen Typ 
  erbt, ist oder implementiert. Der Typ muss somit zur Compilezeit noch nicht feststehen.
  Beispiel:

  ```C# 
  Type typ = ...;
  object a = ...;
  if (typ.IsInstanceOfType(a))
  {
    Console.WriteLine("o ist vom Typ " + typ.Name);
  }
  ```

- [`Type.IsAssignableFrom()` Methode](https://docs.microsoft.com/en-us/dotnet/api/system.type.isassignablefrom?view=netframework-4.7.1#System_Type_IsAssignableFrom_System_Type_).
  Kann verwendet werden, wenn eine Vererbungs-
  oder Implementierungs-Beziehung zwischen zwei jeweils durch `Type`-Instanzen 
  beschriebene Typen überprüft werden soll. Keiner der beiden Typen muss zur Compilezeit
  fest stehen und es muss auch kein konkretes Objekt geben, das überprüft wird.
  **Achtung**: Reihenfolge beachten!
  Beispiel:

  ```C# 
  Type parentType = ...;
  Type childType = ...;
  if (parentType.IsAssignableFrom(childType))
  {
    Console.WriteLine(childType.Name + " implementiert oder erbt von " + parentType);
  }
  ```

## Reflection Einsetzen

### Wann sollte Reflection zum Einsatz kommen?

- So selten wie möglich!
- NIEMALS, wenn klassische Polymorphie (`virtual`) genausogut funktioniert!
- Reflection ist teuer!
- Zugriff erfordert String-Vergleiche, Look-Ups etc. 
- Nichts für zeitkritische inner-Loops!
- Wenn Reflection, dann am Besten nur während Initialisierungs-Routinen, Start-Up, etc.
  Ergebnisse von Reflection-Look-Ups zwischenspeichern.

### Einsatzbeispiele

- Dynamische Bindungen, die nicht mit Polymorphie lösbar sind
- Zur Laufzeit erzeugte Property-Graphen
- Generics-Instanziierungen, wenn T nicht zur Compilezeit bekannt ist
- Double-Dispatch, Multi-Dispatch-Szenarien (`virtual` ist single-Dispatch)
- Dynamische Konfiguration zur Start-Up-Zeit
  - PlugIn Mechanismen
  - Inversion-of-Control Container 




