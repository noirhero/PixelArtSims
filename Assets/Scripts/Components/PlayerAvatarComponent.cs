// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

namespace Components {
    [Serializable]
    public struct PlayerAvatarComponent : IComponentData {
        public float madness;
        public float eyesight;
        public float agility;
        public float intelligence;
        public float search;
        public float mentality;
        public float physical;
        public float luck;
    }
}
