// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

namespace Components {
    [Serializable]
    public struct SpriteAnimComponent : IComponentData {
        public int hash;
        public int frame;
        public float accumTime;
    }
}
