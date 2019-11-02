// Copyright 2018-2019 TAP, Inc. All Rights Reserved

using System;
using Unity.Entities;
using UnityEngine;

namespace Components {
    [Serializable]
    public struct CameraObjectComponent : ISharedComponentData, IEquatable<CameraObjectComponent> {
        public Transform cameraTransform;
        public Camera camera;

        public bool Equals(CameraObjectComponent other) {
            return camera == other.camera;
        }

        public override int GetHashCode() {
            return (null == camera) ? 0 : camera.GetHashCode();
        }
    }
}
