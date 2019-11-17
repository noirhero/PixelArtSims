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
        [Header("Preset")] public SpritePreset spritePreset = null;
        public GUIPreset guiPreset = null;
        public AudioClip footfall = null;

        [Header("Property")] public float eyesight = 1.0f;
        public float agility = 1.0f;
        public float intelligence = 1.0f;
        public float search = 1.0f;
        public float mentality = 1.0f;
        public float physical = 1.0f;
        public float luck = 1.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            if (null == spritePreset || null == guiPreset) {
                Debug.LogError("Set preset, now!!!!!");
                dstManager.DestroyEntity(entity);
                return;
            }

            dstManager.AddComponentData(entity, new SpriteAnimComponent() {
                hash = spritePreset.datas.Keys.First()
            });
            dstManager.AddComponentData(entity, new VelocityComponent() {
                xValue = 1.0f
            });

            dstManager.AddComponentData(entity, new PlayerAvatarComponent());
            dstManager.AddComponentData(entity, new InventoryComponent());
            dstManager.AddComponentData(entity, new AvatarPropertyComponent() {
                eyesight = eyesight,
                agility = agility,
                intelligence = intelligence
            });
            dstManager.AddComponentData(entity, new ForceStateComponent());
            dstManager.AddComponentData(entity, new IntelligenceComponent() {
                inEyesightEntityToDistance = float.MaxValue
            });

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
