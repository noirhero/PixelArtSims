// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;

using Components;

namespace Proxies {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class CameraObjectProxy : MonoBehaviour, IConvertGameObjectToEntity {
        public GameObject cameraObject = null;


        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            if (null == cameraObject) {
                Debug.Log("Set camera, now!!!!!");
                dstManager.DestroyEntity(entity);
                return;
            }

            dstManager.AddSharedComponentData(entity, new CameraObjectComponent() {
                cameraObject = cameraObject
            });
        }
    }
}
