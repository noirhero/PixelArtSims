// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System.Linq;
using Unity.Entities;
using UnityEngine;

using Components;

namespace Proxies {
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class PlayerAvatarProxy : MonoBehaviour, IConvertGameObjectToEntity {
        public Preset.SpritePreset preset = null;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            if (null == preset) {
                Debug.Log("Set preset, now!!!!!");
                dstManager.DestroyEntity(entity);
                return;
            }

            dstManager.AddComponentData(entity, new PlayerAvatarComponent());
            dstManager.AddSharedComponentData(entity, new SpritePresetComponent() {
                preset = preset
            });
            dstManager.AddComponentData(entity, new SpriteAnimComponent() {
                hash = preset.datas.Keys.First()
            });
            dstManager.AddComponentData(entity, new VelocityComponent() {
                xValue = 1.0f
            });
            dstManager.AddComponentData(entity, new ForceStateComponent() {
                useEntity = Entity.Null,
                state = (int)ForceState.None
            });
        }
    }
}
