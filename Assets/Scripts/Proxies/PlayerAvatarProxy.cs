// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System.Linq;
using Unity.Entities;
using UnityEngine;

using Components;
using Preset;

namespace Proxies {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class PlayerAvatarProxy : MonoBehaviour, IConvertGameObjectToEntity {
        public SpritePreset spritePreset = null;
        public GUIPreset guiPreset = null;
        public AudioClip footfall = null;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            if (null == spritePreset || null == guiPreset) {
                Debug.LogError("Set preset, now!!!!!");
                dstManager.DestroyEntity(entity);
                return;
            }

            dstManager.AddComponentData(entity, new PlayerAvatarComponent());
            dstManager.AddComponentData(entity, new InventoryComponent());
            dstManager.AddComponentData(entity, new SpriteAnimComponent() {
                hash = spritePreset.datas.Keys.First()
            });
            dstManager.AddComponentData(entity, new VelocityComponent() {
                xValue = 1.0f
            });
            dstManager.AddComponentData(entity, new ForceStateComponent() {
                useEntity = Entity.Null,
                state = (int)ForceState.None
            });

            // shared
            dstManager.AddSharedComponentData(entity, new SpritePresetComponent() {
                preset = spritePreset
            });
            dstManager.AddSharedComponentData(entity, new GUIPresetComponent() {
                preset = guiPreset
            });

            if (null != footfall) {
                dstManager.AddSharedComponentData(entity, new AudioClipComponent() {
                    clip = footfall
                });
                dstManager.AddComponentData(entity, new FootfallComponent() {
                    interval = 0.5f,
                    accumTime = 0.0f
                });
            }
        }
    }
}
