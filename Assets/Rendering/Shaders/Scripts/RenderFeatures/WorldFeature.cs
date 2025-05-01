using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WorldFeature : ScriptableRendererFeature
{
    public LayerMask worldLayerMask;
    public RenderTexture WorldTexture;
    public RenderPassEvent _WorldEvent = RenderPassEvent.AfterRenderingOpaques;
    WorldPass m_WorldPass;
    public Material worldMaterial;


    class WorldPass : ScriptableRenderPass
    {
        private ProfilingSampler m_ProfilingSampler;
        private FilteringSettings m_FilteringSettings;
        private List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
        private RenderTexture target;
        private Material worldMaterial;

        public WorldPass(RenderTexture target, LayerMask layerMask, Material worldMaterial)
        {
            m_ProfilingSampler = new ProfilingSampler("RenderNormals");
            m_FilteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);

            this.target = target;
            m_ShaderTagIdList.Add(new ShaderTagId("DepthOnly")); // Only render DepthOnly pass\
            
            this.worldMaterial = worldMaterial;
        }
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureTarget(target);
            ConfigureClear(ClearFlag.All, Color.black);
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
                return;

            Camera cam = renderingData.cameraData.camera;

            // Step 1: Calculate the bounding box of the camera's frustum in world space
            /* float distance = camera.farClipPlane; // Or any custom distance you'd like
             Vector3 camForward = camera.transform.forward;
             Vector3 camCenter = camera.transform.position + camForward * (distance * 0.5f);

             float frustumHeight = 2.0f * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * distance;
             float frustumWidth = frustumHeight * camera.aspect;
             Vector3 boundsSize = new Vector3(frustumWidth, frustumHeight, distance);

             Bounds cameraBounds = new Bounds(camCenter, boundsSize);

             // Step 2: Set shader global values or pass to material
             Vector3 minBounds = cameraBounds.min;
             Vector3 maxBounds = cameraBounds.max;*/

            // Calculate the minimum and maximum world points visible to the camera
            Vector3 minWorldPoint = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); // Left and bottom edge of the viewport
            Vector3 maxWorldPoint = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); // Right and top edge of the viewport

            Debug.Log("Min World Point: " + minWorldPoint);
            Debug.Log("Max World Point: " + maxWorldPoint);
            Vector3 camCenter = cam.transform.position;
            worldMaterial.SetVector("_CamPos", camCenter);
            // Option A: Set globals for all shaders
            /*Shader.SetGlobalVector("_MinBound", minWorldPoint);
            Shader.SetGlobalVector("_MaxBound", maxWorldPoint);

            // Option B: Set it directly on your material
            worldMaterial.SetVector("_MinBound", minWorldPoint);
            worldMaterial.SetVector("_MaxBound", maxWorldPoint);*/


            SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
            DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);
            drawingSettings.overrideMaterial = worldMaterial;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    /// <inheritdoc/>
    public override void Create()
    {
        m_WorldPass = new WorldPass(WorldTexture, worldLayerMask, worldMaterial);

        // Configures where the render pass should be injected.
        m_WorldPass.renderPassEvent = _WorldEvent;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
            renderer.EnqueuePass(m_WorldPass);
    }
}


