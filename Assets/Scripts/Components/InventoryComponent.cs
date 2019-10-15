// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

namespace Components {
    [Serializable]
    public struct InventoryComponent : IComponentData {
        public int item01;
        public int item02;
        public int item03;
        public int currentGettingItem;
    }
}
