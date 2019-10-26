// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using Components;

namespace Systems {
    public class AudioVolumeSystem : ComponentSystem {
        protected override void OnUpdate() {
            var playerPos = new float2();
            Entities.ForEach((ref PlayerAvatarComponent playerAvatarComp, ref Translation posComp) => {
                playerPos.x = posComp.Value.x;
                playerPos.y = posComp.Value.y;
            });

            var delta = Time.deltaTime;
            Entities.ForEach((AudioSourceComponent audioSrcComp, ref AudioVolumeComponent audioVolumeComp) => {
                var center = audioVolumeComp.pos.x + audioVolumeComp.offset.x;
                var length = math.abs(center - playerPos.x);
                var volumeDelta = ((audioVolumeComp.size.x * 0.5f < length) ? -0.2f : 0.2f) * delta;

                var source = audioSrcComp.source;
                var volume = source.volume;
                volume = math.clamp(volume + volumeDelta, audioVolumeComp.minVolume, audioVolumeComp.maxVolume);
                source.volume = volume;

                if (math.FLT_MIN_NORMAL > volume && source.isPlaying) {
                    source.Pause();
                }
                else if (math.FLT_MIN_NORMAL < volume && false == source.isPlaying) {
                    source.Play();
                }
            });
        }
    }
}
