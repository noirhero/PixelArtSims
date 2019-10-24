// Copyright 2018-2019 TAP, Inc. All Rights Reserved. 

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using RotaryHeart.Lib.SerializableDictionary;

namespace Preset {
    [Serializable]
    public struct SpritePresetData {
        public Texture texture;
        public float length;
        public float frameRate;
        public float3 scale;
        public float3 posOffset;
        public List<Vector4> rects;
    }

    [Serializable]
    public class SpritePresetDataDictionary : SerializableDictionaryBase<int, SpritePresetData> {}

    [Serializable]
    public class SpritePreset : MonoBehaviour {
        [Header("Geometry")]
        public Mesh mesh = null;
        public Material material = null;

        [Header("Animation clips")]
        public List<AnimationClip> clips = new List<AnimationClip>();

        [Header("Do not touch!")]
        public SpritePresetDataDictionary datas = new SpritePresetDataDictionary();
    }
}
