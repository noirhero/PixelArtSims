// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

namespace Components {
    [Serializable]
    public struct EyesightComponent : IComponentData {
        public Entity target;
        public float thinkingCompletionTime;
        public float thinkingTime;
    }
}
