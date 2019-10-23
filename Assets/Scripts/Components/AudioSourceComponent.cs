// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Entities;
using UnityEngine;

namespace Components {
    [Serializable]
    public struct AudioSourceComponent : ISharedComponentData, IEquatable<AudioSourceComponent> {
        public AudioSource source;

        public bool Equals(AudioSourceComponent other) {
            return source == other.source;
        }

        public override int GetHashCode() {
            return (null == source) ? 0 : source.GetHashCode();
        }
    }
}
