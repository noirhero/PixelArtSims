// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

using Components;

namespace Systems {
    public class IntelligenceSystem : JobComponentSystem {
        [BurstCompile]
        struct IntelligenceSystemJob : IJobForEachWithEntity<AvatarPropertyComponent, ThinkingComponent, ForceStateComponent> {
            [ReadOnly] public float deltaTime;
            public void Execute(Entity entity, int index, [ReadOnly] ref AvatarPropertyComponent propComp, ref ThinkingComponent thinkingComp, ref ForceStateComponent forceStateComp) {
                if (thinkingComp.findEntity == Entity.Null) {
                    return;
                }

                thinkingComp.thinkingTime -= propComp.intelligence * deltaTime;
                if (0.0f < thinkingComp.thinkingTime) {
                    return;
                }

                // Todo : Intelligence here!!!!!

                thinkingComp.findEntity = Entity.Null;
                thinkingComp.thinkingTime = 0.0f;
                thinkingComp.entityToDistance = float.MaxValue;

                forceStateComp.state = (int)ForceState.None;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            var job = new IntelligenceSystemJob() {
                deltaTime = Time.deltaTime
            };
            return job.Schedule(this, inputDependencies);
        }
    }
}
