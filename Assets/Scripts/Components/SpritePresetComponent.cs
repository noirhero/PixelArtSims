// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using UnityEngine.Serialization;

namespace Components {
    [Serializable]
    public struct SpritePresetComponent : ISharedComponentData, IEquatable<SpritePresetComponent> {
        public Preset.SpritePreset preset;

        public bool Equals(SpritePresetComponent other) {
            return preset == other.preset;
        }

        public override int GetHashCode() {
            return (null == preset) ? 0 : preset.GetHashCode();
        }
    }
}
