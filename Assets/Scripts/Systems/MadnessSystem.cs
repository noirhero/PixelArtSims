// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

using Components;

namespace Systems {
    public class MadnessSystem : JobComponentSystem {
        [BurstCompile]
        struct MadnessSystemJob : IJobForEach<AvatarComponent, MadnessComponent> {
            [ReadOnly] public float deltaTime;

            public void Execute(ref AvatarComponent propComp, [ReadOnly] ref MadnessComponent madnessComp) {
                propComp.madness += madnessComp.value * deltaTime - propComp.mentality * deltaTime;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            var job = new MadnessSystemJob() {
                deltaTime = Time.deltaTime
            };

            return job.Schedule(this, inputDependencies);
        }
    }
}
