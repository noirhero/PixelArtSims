﻿// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

namespace Components {
    public enum ReactiveType {
        None,
        Wall,
        Item,
        Something
    }

    [Serializable]
    public struct ReactiveComponent : IComponentData {
        public int type;
    }
}