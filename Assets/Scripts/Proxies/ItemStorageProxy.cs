// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using Components;
using SuperTiled2Unity;
using Unity.Entities;
using UnityEngine;

namespace Proxies {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class ItemStorageProxy : MonoBehaviour, IConvertGameObjectToEntity {
        public float useTimeSec = 0.0f;
        public List<int> randItemIndices = new List<int>();

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            if (randItemIndices.IsEmpty()) {
                Debug.Log("Fill item indices, now!!!!!");
                dstManager.DestroyEntity(entity);
                return;
            }

            dstManager.AddComponentData(entity, new ItemStorageComponent() {
                index = randItemIndices[new System.Random().Next(0, randItemIndices.Count)],
                forceStateTime = useTimeSec,
                checkRadius = GetComponent<SphereCollider>().radius 
            });
        }
    }
}
