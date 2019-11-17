// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

using Components;
using Preset;

namespace Systems {
    public class FootfallSystem : ComponentSystem {
        protected override void OnUpdate() {
            var delta = Time.deltaTime;
            Entities.ForEach((AudioClipComponent audioClipComp, ref SpriteAnimComponent animComp, ref Translation posComp, ref FootfallComponent footfallComp) => {
                if (animComp.hash != StatePreset.GetStateHash(StatePreset.Type.Walk)) {
                    footfallComp.accumTime = 0.0f;
                    return;
                }

                footfallComp.accumTime += delta;
                if (footfallComp.interval <= footfallComp.accumTime) {
                    footfallComp.accumTime = 0.0f;
                    AudioSource.PlayClipAtPoint(audioClipComp.clip, posComp.Value);
                }
            });
        }
    }
}
