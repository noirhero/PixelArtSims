// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using Components;

namespace Proxies {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class WallProxy : MonoBehaviour, IConvertGameObjectToEntity {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            var boxCollision = GetComponent<BoxCollider2D>();
            if (null == boxCollision) {
                Debug.LogError("Add component 'BoxCollider2D', now!!!!!");
                dstManager.DestroyEntity(entity);
                return;
            }

            dstManager.RemoveComponent<LocalToWorld>(entity);
            dstManager.RemoveComponent<Rotation>(entity);

            var pos = transform.position;
            dstManager.AddComponentData(entity, new WallComponent() {
                pos = new float2(pos.x, pos.y),
                size = boxCollision.size,
                offset = boxCollision.offset
            });
            dstManager.AddComponentData(entity, new ReactiveComponent() {
                type = (int) ReactiveType.Wall
            });
        }
    }
}
