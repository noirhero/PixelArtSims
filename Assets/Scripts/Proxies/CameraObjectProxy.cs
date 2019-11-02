// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

using Components;

namespace Proxies {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class CameraObjectProxy : MonoBehaviour, IConvertGameObjectToEntity {
        public GameObject cameraObject = null;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            if (null == cameraObject) {
                Debug.LogError("Set camera, now!!!!!");
                dstManager.DestroyEntity(entity);
                return;
            }

            dstManager.RemoveComponent<Translation>(entity);
            dstManager.RemoveComponent<Rotation>(entity);
            dstManager.RemoveComponent<LocalToWorld>(entity);

            dstManager.AddSharedComponentData(entity, new CameraObjectComponent() {
                cameraTransform = cameraObject.transform,
                camera = cameraObject.GetComponent<Camera>()
            });
        }
    }
}
