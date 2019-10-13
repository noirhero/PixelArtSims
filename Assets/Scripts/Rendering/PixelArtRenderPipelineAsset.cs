// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using UnityEngine;
using UnityEngine.Rendering;

namespace Rendering {
    [CreateAssetMenu(menuName ="Rendering/CustomPipeline")]
    public class PixelArtRenderPipelineAsset : RenderPipelineAsset {
        protected override RenderPipeline CreatePipeline() {
            return new PixelArtRenderPipeline();
        }
    }
}