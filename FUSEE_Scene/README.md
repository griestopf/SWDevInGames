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




## Mehrere Würfel

## Objekthierarchien


