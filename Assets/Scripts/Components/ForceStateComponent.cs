﻿// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;

namespace Components {
    enum ForceState {
        None,
        Item,
        Thinking,
        Count
    }

    [Serializable]
    public struct ForceStateComponent : IComponentData {
        public int state;
    }
}
