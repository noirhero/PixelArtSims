// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using UnityEngine;

namespace Components {
    [Serializable]
    public struct AudioClipComponent : ISharedComponentData, IEquatable<AudioClipComponent> {
        public AudioClip clip;

        public bool Equals(AudioClipComponent other) {
            return clip == other.clip;
        }

        public override int GetHashCode() {
            return (null == clip) ? 0 : clip.GetHashCode();
        }
    }
}
