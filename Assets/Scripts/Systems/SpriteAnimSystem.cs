// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;

using Components;

namespace Systems {
    public class SpriteAnimSystem : ComponentSystem {
        protected override void OnUpdate() {
            var dt = Time.deltaTime;

            Entities.ForEach((SpritePresetComponent presetComp, ref SpriteAnimComponent animComp) => {
                if (false == presetComp.preset.datas.TryGetValue(animComp.hash, out var presetData)) {
                    return;
                }

                animComp.accumTime += dt;
                if (presetData.length <= animComp.accumTime) {
                    animComp.accumTime %= presetData.length;
                }
                animComp.frame = (int)(animComp.accumTime / presetData.frameRate);
            });
        }
    }
}
