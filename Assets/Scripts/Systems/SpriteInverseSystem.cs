// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

using Components;

namespace Systems {
    public class SpriteInverseSystem : JobComponentSystem {
        [BurstCompile]
        struct SpriteInverseSystemJob : IJobForEach<Rotation, VelocityComponent> {
            public void Execute(ref Rotation rotComp, [ReadOnly] ref VelocityComponent velocityComp) {
                rotComp.Value = (0.0f > velocityComp.xValue) ? quaternion.identity : quaternion.RotateY(math.radians(180.0f));
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            return new SpriteInverseSystemJob().Schedule(this, inputDependencies);
        }
    }
}
