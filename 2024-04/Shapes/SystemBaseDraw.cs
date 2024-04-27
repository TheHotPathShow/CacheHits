using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace TMG.Shapes
{
    // Classes that inherit from SystemBaseDraw can  use the Draw class from the Shapes library
    public abstract partial class SystemBaseDraw : SystemBase
    {
        protected override void OnStartRunning() => RenderPipelineManager.beginCameraRendering += DrawShapesSRP;
        protected override void OnStopRunning()  => RenderPipelineManager.beginCameraRendering -= DrawShapesSRP;
        private void DrawShapesSRP( ScriptableRenderContext ctx, Camera cam ) => OnCameraPreRender( cam );
        
        protected abstract void OnDraw(Camera cam);
        
        void OnCameraPreRender( Camera cam ) {
            switch( cam.cameraType ) {
                case CameraType.Reflection:
                    return; // Don't render in reflection probes in case we run this script in the editor
            }

            // Don't draw Immediate Mode Shapes to cameras that cull the Shapes layer
            var myLayer = LayerMask.NameToLayer("Shapes");
            if ((cam.cullingMask & 1 << myLayer) != 0)
            {
                OnDraw(cam);
            }
        }
    }
}