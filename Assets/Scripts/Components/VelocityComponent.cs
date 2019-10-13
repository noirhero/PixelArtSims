// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Components {
    [Serializable]
    public struct VelocityComponent : IComponentData {
        public float2 velocity;
        public float xValue;
    }
}
