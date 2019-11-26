// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System.Linq;
using Unity.Entities;
using UnityEngine;

using Preset;
using Components;

namespace Proxies {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class OldOneProxy : MonoBehaviour, IConvertGameObjectToEntity {
        public SpritePreset spritePreset = null;
        public float search = 0.0f;
        public float madness = 0.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            if (null == spritePreset) {
                Debug.LogError("Set preset, now!!!!!");
                dstManager.DestroyEntity(entity);
                return;
            }

            dstManager.AddSharedComponentData(entity, new SpritePresetComponent() {
                preset = spritePreset
            });

            dstManager.AddComponentData(entity, new SpriteAnimComponent() {
                hash = spritePreset.datas.Keys.First()
            });
            dstManager.AddComponentData(entity, new ReactiveComponent() {
                type = (int) ReactiveType.Something,
                search = search,
                madness = madness
            });
        }
    }
}
