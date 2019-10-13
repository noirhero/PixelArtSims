// Copyright 2018-2019 TAP, Inc. All Rights Reserved

using System;
using Unity.Entities;
using UnityEngine;

namespace Components {
    [Serializable]
    public struct CameraObjectComponent : ISharedComponentData, IEquatable<CameraObjectComponent> {
        public GameObject cameraObject;

        public bool Equals(CameraObjectComponent other) {
            return cameraObject == other.cameraObject;
        }

        public override int GetHashCode() {
            return (null == cameraObject) ? 0 : cameraObject.GetHashCode();
        }
    }
}
 