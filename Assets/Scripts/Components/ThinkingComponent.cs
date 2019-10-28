// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

namespace Components {
    [Serializable]
    public struct ThinkingComponent : IComponentData {
        public Entity findEntity;
        public float entityToDistance;
        public float thinkingTime;
    }
}
