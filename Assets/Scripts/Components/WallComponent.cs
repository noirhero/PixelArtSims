// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Components {
    [Serializable]
    public struct WallComponent : IComponentData {
        public float2 pos;
        public float2 size;
        public float2 offset;
    }
}
