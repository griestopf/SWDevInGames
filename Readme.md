# Sommersemester 2023

## Lernziele

 - Erlernen und Verstehen neuerer Sprachkonstrukte in zeitgemäßen Programmiersprachen
 - Anwendung dieser Sprachkonstrukte in medienbezogenen Anwendungen, u.A. in der OpenSource 3D-Library FUSEE, Made in
   Furtwangen 
 - PROGRAMMIEREN KÖNNEN


## Inhalte 

- Generics/Collections/Iterator
- Reflection/DependencyInjection/DoubleDispatch
- Concurrency/Threads/Async&Await
- Events/Delegates/Anonymous methods
- Lambdas & LINQ (ggf.)

## Tools

- [.NET SDK (aktuell v.6.0)](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/download)
- [C# for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp)
- [FUSEE](http://fusee3d.org/)

## Lektionen

### [Lektion 01](01_MemValRef)

 - Value vs. Reference-Typen
 - Memory-Layout zeichnen
 - Boxing & Unboxing

### [Lektion 02](02_Inheritance)

- Vererbung 
- Polymorphie mit `virtual`
- Polymorphie mit `interface`

### [Lektion 03](03_Callback)

- Callback mit `virtual`
- Callback mit `interface`
- Callback mit `delegate`
- Callback mit `event`
- Anonyme methoden, Lambdas und Events

### [Lektion 04](04_Generics)

- Collection-Klasse für `object`
- Collection-Klasse mit generischem Inhalt
- Indexer für Collections
- Enumerator mit `yield`

### [Lektion 05](05_Reflection)

- Reflection
- Decompiler
- Attribute
- Dependency Injection

### [Lektion 06](06_VisitorPattern)

- Anwendungsbeispiel Szenengraph
- Implementierung von GraphicObjects mit direktem polymorphen Rendering
- Trennung von GraphicObjects und Traversierung sowie Traversierungs-"Grund". "Polymorphie" durch Reflection
- "Echte" doppelte Polymorphie (Double-Dispatch)
- Visitor-Pattern mit Reflection "done right".

