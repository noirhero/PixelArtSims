// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

namespace Components {
    [Serializable]
    public struct ItemStorageComponent : IComponentData {
        public int index;
        public float gettingTime;
        public float checkRadius;
    }
}
