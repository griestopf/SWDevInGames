using System;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Fusee.Engine.GUI;

namespace FuseeApp
{

    [FuseeApplication(Name = "SuperSimple", Description = "Yet another FUSEE App.")]
    public class SuperSimple : RenderCanvas
    {

        Mesh _mesh;


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
            RC.Model = 
                float4x4.CreateTranslation(0.0f, 0.0f, 0.0f)
                *   float4x4.CreateRotationY(35.0f * M.Pi / 180.0f)
                *   float4x4.CreateRotationX(0.0f * M.Pi / 180.0f)
                *   float4x4.CreateRotationZ(0.0f * M.Pi / 180.0f)
                *   float4x4.CreateScale(1.0f,1.0f, 1.0f);
            
            // Set the camera 30 units along the _negative_ z-axis. 
            // (The View matrix holds the inversion of the camera transformation, 
            // thus we apply a positive 30 Z translation).
            RC.View = float4x4.CreateTranslation(0, 0, 30);
        }

        // RenderAFrame is called once per frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);


            RC.Render(_mesh);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        // Is called when the window was resized
        public override void Resize()
        {
            // Set the new rendering area to the entire new windows size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width/(float) Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 0.01 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 200 (Anything further away from the camera than 200 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 0.01f, 200.0f);
            RC.Projection = projection;
        }
    }
}