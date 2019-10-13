// Copyright 

using System;
using Unity.Entities;

namespace Components {
    [Serializable]
    public struct GUIPresetComponent : ISharedComponentData, IEquatable<GUIPresetComponent> {
        public Preset.GUIPreset preset;

        public bool Equals(GUIPresetComponent other) {
            return preset == other.preset;
        }

        public override int GetHashCode() {
            return (null == preset) ? 0 : preset.GetHashCode();
        }
    }
}
