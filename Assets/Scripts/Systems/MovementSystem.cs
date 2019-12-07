// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using Components;

namespace Systems {
    public class MovementSystem : JobComponentSystem {
        [BurstCompile]
        struct MovementSystemJob : IJobForEach<Translation, VelocityComponent, AvatarComponent> {
            [ReadOnly] public float deltaTime;

            public void Execute(ref Translation posComp, [ReadOnly] ref VelocityComponent velocityComp, [ReadOnly] ref AvatarComponent avatarComp) {
                posComp.Value.x += velocityComp.velocity.x * avatarComp.agility * deltaTime;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            return (new MovementSystemJob() {
                deltaTime = Time.deltaTime
            }).Schedule(this, inputDependencies);
        }
    }
}
