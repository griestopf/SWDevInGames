# FUSEE Beispiele Szenengraph

## Sehr einfaches FUSEE-Beispiel

Das Unterverzeichnis [SuperSimple](SuperSimple/) enthält den unten beschriebenen Code als komplett lauffähiges Projekt.

### Init-Methode

Die `Init()`-Methode einer FUSEE-App wird von FUSEE ein Mal zum Start einer FUSEE-Applikation aufgerufen.

- Erzeugt einen Shader-Effekt (zum Einfärben von Geometrie) und einen Würfel und setzt diesen als aktuellen Zustand des Render-Context.

- Erzeugt ein würfelförmiges Mesh und speichert dieses in einem Feld der Klasse

- Setzt die Model-Matrix des Render-Context so, dass sämtliche Geometrie mit einem Drehwinkel von 35° um die Y-Achse (FUSEE's Hochachse) gerendert werden.

- Setzt die View-Matrix des Render-Context so, dass die Kamera 30 Einheiten entlang der Z-Achse nach hinten gesetzt wird.


```C#
    public override void Init()
    {
        // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
        RC.ClearColor = new float4(1, 1, 1, 1);

        // Create a shader effect and set it on the render context
        var shaderEffect = SimpleMeshes.MakeShaderEffect(new float3(0.4f, 1.0f, 0.2f), float3.One, 4);
        RC.SetShaderEffect(shaderEffect);

        // Create a mesh representing a cube
        _mesh = SimpleMeshes.CreateCuboid(new float3(5, 5, 5));

        // Set the model matrix to turn all rendered geometry  cube model around 35°
        RC.Model = float4x4.CreateRotationY(35.0f * M.Pi / 180.0f);
        
        // Set the camera 30 units along the _negative_ z-axis. 
        // (The View matrix holds the inversion of the camera transformation, 
        // thus we apply a positive 30 Z translation).
        RC.View = float4x4.CreateTranslation(0, 0, 30);
    }
```

### RenderAFrame-Methode

Die `RenderAFrame()`-Methode einer FUSEE-App wird ständig in einer laufenden FUSEE-App aufgerufen. Jeder Aufruf soll ein Bild erzeugen. 

- Löscht den _BackBuffer_: Jedes Pixel im Hintergrundspeicher des Ausgabefensters wird mit der `ClearColor` gefüllt.

- Rendert das Mesh unter den voreingestellten Parametern `ShaderEffect`, `View`- und `Model`-Matrix in den Hintergrundspeicher.

- Präsentiert dem Betrachter das gerenderte Bild, indem das Bild aus dem HIntergrundspeicher in den sichtbaren Speicherberich des Ausgabefensters kopiert wird.

```C#
    public override void RenderAFrame()
    {
        // Clear the backbuffer
        RC.Clear(ClearFlags.Color | ClearFlags.Depth);


        RC.Render(_mesh);

        // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
        Present();
    }
```

## Position/Orientierung/Skalierung des Würfels

Um den Würfel nicht nur um die Y-Achse zu rotieren, sondern um alle drei Raum-Achsen und zudem den Würfel auch noch skalieren und positionieren zu können, wird üblicherweise ein Kombination unterschiedlicher Transformations-Matrizen als `RC.Model`-Matrix gesetzt:

```C#
    public override void RenderAFrame()
    {
        // Clear the backbuffer
        RC.Clear(ClearFlags.Color | ClearFlags.Depth);

        // Set the model matrix to turn all rendered geometry  cube model around 35°
        RC.Model = 
            float4x4.CreateTranslation(0.0f, 0.0f, 0.0f)
            *   float4x4.CreateRotationY(35.0f * M.Pi / 180.0f)
            *   float4x4.CreateRotationX(0.0f * M.Pi / 180.0f)
            *   float4x4.CreateRotationZ(0.0f * M.Pi / 180.0f)
            *   float4x4.CreateScale(1.0f,1.0f, 1.0f);


        RC.Render(_mesh);

        // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
        Present();
    }
```

## Mehrere Würfel

Die Darstellung mehrer Würfel kann nun so erfolgen, dass das Mesh mehrfach mit unterschiedlich gesetzter `RC.Model`-Matrix gerendert wird.

```C#
    // First render Cube at x = -3
    RC.Model = 
        float4x4.CreateTranslation(-3.0f, 0.0f, 0.0f)
        *   float4x4.CreateRotationY(0.0f * M.Pi / 180.0f)
        *   float4x4.CreateRotationX(0.0f * M.Pi / 180.0f)
        *   float4x4.CreateRotationZ(0.0f * M.Pi / 180.0f)
        *   float4x4.CreateScale(1.0f,1.0f, 1.0f);

    RC.Render(_mesh);

    // Now render Cube at x = +3
    RC.Model = 
        float4x4.CreateTranslation( 3.0f, 0.0f, 0.0f)
        *   float4x4.CreateRotationY(0.0f * M.Pi / 180.0f)
        *   float4x4.CreateRotationX(0.0f * M.Pi / 180.0f)
        *   float4x4.CreateRotationZ(0.0f * M.Pi / 180.0f)
        *   float4x4.CreateScale(1.0f,1.0f, 1.0f);

    RC.Render(_mesh);
```


## Objekthierarchien

Die Eigenschaften mehrerer Transformtaionen lassen sich hintereinanderschalten, in dem die Matrizen miteinander multipliziert werden:

```C#
    // Parent: Cube rotated 35° around x
    RC.Model = 
        float4x4.CreateTranslation(-3.0f, 0.0f, 0.0f)
        *   float4x4.CreateRotationY(35.0f * M.Pi / 180.0f)
        *   float4x4.CreateRotationX(0.0f * M.Pi / 180.0f)
        *   float4x4.CreateRotationZ(0.0f * M.Pi / 180.0f)
        *   float4x4.CreateScale(1.0f,1.0f, 1.0f);

    RC.Render(_mesh);

    // Child: Additionally placed 3 units above parent
    RC.Model *= 
        float4x4.CreateTranslation( 0.0f, 3.0f, 0.0f)
        *   float4x4.CreateRotationY(0.0f * M.Pi / 180.0f)
        *   float4x4.CreateRotationX(0.0f * M.Pi / 180.0f)
        *   float4x4.CreateRotationZ(0.0f * M.Pi / 180.0f)
        *   float4x4.CreateScale(1.0f,1.0f, 1.0f);

    RC.Render(_mesh);
```

## Aufgabe

Klassenbibliothek erzeugen:

- `class GraphicsObject`
    - Elternklasse
- `class Group :  GraphicsObject`
    - Enthält `Children`: Liste von GraphicsObject` 
- `class Transform :  GraphicsObject`
    - Enthält Position Rotation und Scale, jeweils als `float3` (mit x, y, z)
- `class Mesh : GraphicsObject`
    - Enthält ein `Mesh`

Mit `Group` Instanzen lassen sich baumartige Hierarchien bauen. Diese sollen zum Rendern traversiert werden, wobei jede Instanz ihren Beitrag (mit entsprechenden Aufrufen auf `RC`) zum Gesamtbild leistet


