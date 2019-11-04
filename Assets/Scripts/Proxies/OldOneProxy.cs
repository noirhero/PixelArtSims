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

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            if (null == spritePreset) {
                Debug.LogError("Set preset, now!!!!!");
                dstManager.DestroyEntity(entity);
                return;
            }

            dstManager.AddComponentData(entity, new OldOneComponent() {
                madness = 20.0f
            });
            dstManager.AddComponentData(entity, new SpriteAnimComponent() {
                hash = spritePreset.datas.Keys.First()
            });
            dstManager.AddComponentData(entity, new ReactiveComponent() {
                type = (int) ReactiveType.Something
            });
            dstManager.AddSharedComponentData(entity, new SpritePresetComponent() {
                preset = spritePreset
            });
        }
    }
}
