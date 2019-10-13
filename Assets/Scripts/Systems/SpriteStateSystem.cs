// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

using Components;
using Preset;

namespace Systems {
    public class SpriteStateSystem : JobComponentSystem {
        struct SpriteStateSystemJob : IJobForEach<SpriteAnimComponent, VelocityComponent, ForceStateComponent> {
            public void Execute(ref SpriteAnimComponent animComp, [ReadOnly] ref VelocityComponent velocityComp, [ReadOnly] ref ForceStateComponent forceStateComp) {
                if (ForceState.None != (ForceState)forceStateComp.state) {
                    animComp.hash = StatePreset.GetStateHash(StatePreset.Type.SomethingDoIt);
                    return;
                }

                var stateType = (math.FLT_MIN_NORMAL > math.abs(velocityComp.velocity.x))
                    ? StatePreset.Type.Idle
                    : StatePreset.Type.Walk;
                animComp.hash = StatePreset.GetStateHash(stateType);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            return new SpriteStateSystemJob().Schedule(this, inputDependencies);
        }
    }
}
