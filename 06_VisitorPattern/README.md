# Lektion 06 Visitor Pattern

## Anwendung

In dieser Lektion sollen einige der in vorangegangenen Lektionen vorgestellten Techniken in einem anwendungsnahen
Einsatzgebiet gezeigt werden. Das hier vorgestellte ***Visitor Pattern*** findet vor allem in Software im 
Medien- und Games-Bereich an vielen Stellen Anwendung.

Häufig ist in derartigen Anwendungen eine Baum-artige Struktur ein zentraler Bestandteil. In 3D-Applikationen 
enthält der _Szenen-Graph_ die darzustellenden hierarchisch aufgebauten Bestandteile der 3D-Szene, in anderen
Applikationen gibt es grafische User-Interface-Elemente, die hierarchisch angeordnet sind und in entsprechenden
Strukturen und Objekten innerhalb der Software hinterlegt sind.

Im Folgenden ist ein Beispiel für einen sehr einfach aufgebauten Szenengraphen einer fiktiven 3D-Applikation:

```
-- "The Parent Group" 
    |
    +-- "The Child Group"
    |    |
    |    +-- "The Sphere" (Radius: 3)
    |    |
    |    +-- "First Cuboid" (W: 6, L: 7, H: 8)
    |
    +-- "Second Cuboid" (W: 3, L: 4, H: 5)

```

## Implementierung

Folgende Sachverhalte sind zu erkennen:

- Sichtbare Objekte sind eine Kugel mit dem Radius drei
  und zwei Quader mit unterschiedlichen Ausmaßen (Breite =**W**idth, **L**änge/Length und **H**öhe/Height).
- Es drei gibt unterschiedliche Typen für die Objekt-Instanzen: `Group`, `Sphere` und `Cuboid` mit unterschiedlichen Eigenschaften.
- `Group`-Instanzen sind in der Lage, Kind-Objekte zu referenzieren.
- Zum Aufbau der Hierarchie werden hier zwei `Group`-Instanzen verwendet. 
- Alle Objekte haben einen Namen
- Als Kind-Objekte von Gruppen können Instanzen 
  aller vorhandenen Typen verwendet werden.

Es sollen nun die o.a. Typen implementiert werden, zusätzlich dazu ein Mechanismus, der den Baum "traversiert", 
d.h. der den Baum Objekt für Objekt besucht und dabei 
für jedes angetroffene Objekt ein Stück Code ausführt. 
Die Reihenfolge soll dabei "depth-first" sein, das bedeutet, in o.a. Beispiel-Baum werden in der angegebenen
Notation alle Objekte in der Reihenfolge von oben nach unten besucht. 

Als Beispiel-Anwendung soll das Rendern dienen, bei dem jedes Objekt im Baum seinen Beitrag zu leisten. Da es 
verschiedene Typen von Instanzen gibt, soll das Ganze _polymorph_ implementiert sein, d.h. zum Rendern eines Quaders kommt anderer Code zum Einsatz als zum Rendern 
einer Kugel und wieder anderer Code für das Rendern einer
Gruppe.

In den Unterverzeichnissen dieser Lektion finden sich unterschiedliche Implementierungen mit diversen Vor- und
Nachteilen.

## 01 - "No Visitor"

## 02 - "Reflection Bad"
  
## 03 - "Double Dispatch"
