// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

namespace Components {
    enum ForceState {
        None,
        Item,
        Count
    }

    [Serializable]
    public struct ForceStateComponent : IComponentData {
        public Entity setterEntity;
        public float time;
        public int state;
    }
}
