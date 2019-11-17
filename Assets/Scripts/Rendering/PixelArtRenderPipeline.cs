// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace Rendering {
    public class PixelArtRenderPipeline : RenderPipeline {
        private ScriptableCullingParameters _cullParam = new ScriptableCullingParameters() {
            isOrthographic = true
        };

        // opaque
        private DrawingSettings _opaqueDrawing = new DrawingSettings(new ShaderTagId("SRPDefaultUnlit"),
            new SortingSettings() {
                criteria = SortingCriteria.CommonOpaque
            }) {
            enableDynamicBatching = true,
            enableInstancing = true
        };
        private FilteringSettings _opaqueFiltering = new FilteringSettings(RenderQueueRange.opaque);

        // transparent
        private DrawingSettings _transparentDrawing = new DrawingSettings(new ShaderTagId("SRPDefaultUnlit"),
            new SortingSettings() {
                criteria = SortingCriteria.CommonTransparent
            }) {
            enableDynamicBatching = true,
            enableInstancing = true
        };
        private FilteringSettings _transparentFiltering = new FilteringSettings(RenderQueueRange.transparent);

        // post-process
        private readonly int _colorNameId = Shader.PropertyToID("_CameraColorTexture");
        private readonly int _depthNameId = Shader.PropertyToID("_CameraDepthTexture");
        private readonly PostProcessRenderContext _ppContext = new PostProcessRenderContext();

        protected override void Render(ScriptableRenderContext context, Camera[] cameras) {
            BeginFrameRendering(context, cameras);
            foreach (var camera in cameras) {
                if (false == camera.TryGetCullingParameters(out _cullParam)) {
                    continue;
                }

                BeginCameraRendering(context, camera);
#if UNITY_EDITOR
                if (CameraType.SceneView == camera.cameraType) {
                    ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
                }
#endif

                var ppLayer = camera.GetComponent<PostProcessLayer>();
                if (null == ppLayer) {
                    NormalRendering(context, camera);
                }
                else {
                    PostProcessingRendering(context, camera, ppLayer);
                }

                EndCameraRendering(context, camera);
            }
            EndFrameRendering(context, cameras);
        }

        private const string _normalCmdName = "NormalRendering";
        private void NormalRendering(ScriptableRenderContext context, Camera camera) {
            var cullResults = context.Cull(ref _cullParam);

            context.SetupCameraProperties(camera);

            var cmdBuf = CommandBufferPool.Get(_normalCmdName);

            cmdBuf.ClearRenderTarget(true, false, camera.backgroundColor);
            context.ExecuteCommandBuffer(cmdBuf);
            cmdBuf.Clear();

            context.DrawRenderers(cullResults, ref _opaqueDrawing, ref _opaqueFiltering);
            context.DrawRenderers(cullResults, ref _transparentDrawing, ref _transparentFiltering);
#if UNITY_EDITOR
            if (CameraType.SceneView == camera.cameraType) {
                context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
                context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
            }
#endif

            context.Submit();
        }

        private const string _ppRenderCmdName = "PostprocessRendering";
        private void PostProcessingRendering(ScriptableRenderContext context, Camera camera, PostProcessLayer ppLayer) {
            var cullResults = context.Cull(ref _cullParam);

            context.SetupCameraProperties(camera);

            var cmdBuf = CommandBufferPool.Get(_ppRenderCmdName);

            cmdBuf.GetTemporaryRT(_colorNameId, camera.pixelWidth, camera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            cmdBuf.GetTemporaryRT(_depthNameId, camera.pixelWidth, camera.pixelHeight, 24, FilterMode.Point, RenderTextureFormat.Depth);
            context.ExecuteCommandBuffer(cmdBuf);
            cmdBuf.Clear();

            cmdBuf.SetRenderTarget(
                _colorNameId, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store,
                _depthNameId, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
            cmdBuf.ClearRenderTarget(true, true, camera.backgroundColor);
            context.ExecuteCommandBuffer(cmdBuf);
            cmdBuf.Clear();

            context.DrawRenderers(cullResults, ref _opaqueDrawing, ref _opaqueFiltering);
            context.DrawRenderers(cullResults, ref _transparentDrawing, ref _transparentFiltering);
#if UNITY_EDITOR
            if (CameraType.SceneView == camera.cameraType) {
                context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
                context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
            }
#endif

            _ppContext.Reset();
            _ppContext.camera = camera;
            _ppContext.source = _colorNameId;
            _ppContext.sourceFormat = RenderTextureFormat.ARGB32;
            _ppContext.destination = BuiltinRenderTextureType.CameraTarget;
            _ppContext.command = cmdBuf;
            _ppContext.flip = true;
            ppLayer.Render(_ppContext);
            context.ExecuteCommandBuffer(cmdBuf);
            cmdBuf.Clear();

            cmdBuf.ReleaseTemporaryRT(_colorNameId);
            cmdBuf.ReleaseTemporaryRT(_depthNameId);
            context.ExecuteCommandBuffer(cmdBuf);
            cmdBuf.Clear();

            context.Submit();
        }
    }
}
