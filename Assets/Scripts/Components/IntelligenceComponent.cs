// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

namespace Components {
    [Serializable]
    public struct IntelligenceComponent : IComponentData {
        public Entity inEyesightEntity;
        public int inEyesightEntityReactiveType;
        public float inEyesightEntityToDistance;
        public float inEyesightEntityThinkingTime;
    }
}
