// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using Components;

namespace Proxies {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class AudioVolumeProxy : MonoBehaviour, IConvertGameObjectToEntity {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            dstManager.RemoveComponent<LocalToWorld>(entity);
            dstManager.RemoveComponent<Translation>(entity);
            dstManager.RemoveComponent<Rotation>(entity);

            var boxCollider2D = GetComponent<BoxCollider2D>();
            Debug.Assert(null != boxCollider2D);

            var pos = transform.position;
            dstManager.AddComponentData(entity, new AudioVolumeComponent() {
                pos = new float2(pos.x, pos.y),
                offset = boxCollider2D.offset,
                size = boxCollider2D.size
            });

            var audioSource = GetComponent<AudioSource>();
            Debug.Assert(null != audioSource);
            
            dstManager.AddSharedComponentData(entity, new AudioSourceComponent() {
                source = audioSource
            });
        }
    }
}
