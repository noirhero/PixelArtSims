// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

namespace Components {
    [Serializable]
    public struct MadnessComponent : IComponentData {
        public float value;
    }
}
