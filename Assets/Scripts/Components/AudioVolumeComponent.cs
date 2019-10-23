// Copyright 2018-2019 TAP, Inc. All Rights.

using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Components {
    [Serializable]
    public struct AudioVolumeComponent : IComponentData {
        public float2 pos;
        public float2 offset;
        public float2 size;
    }
}
