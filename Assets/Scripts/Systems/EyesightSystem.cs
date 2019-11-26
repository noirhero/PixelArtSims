// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

using Components;

namespace Systems {
    public class EyesightSystem : JobComponentSystem {
        private EntityCommandBufferSystem _cmdSystem = null;
        protected override void OnCreate() {
            _cmdSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        struct EyesightSystemJob : IJobForEachWithEntity<EyesightComponent, PlayerAvatarComponent> {
            [ReadOnly] public float deltaTime;
            public EntityCommandBuffer.Concurrent cmdBuf;

            public void Execute(Entity entity, int index, ref EyesightComponent eyesightComp, [ReadOnly] ref PlayerAvatarComponent propComp) {
                eyesightComp.thinkingTime += deltaTime * propComp.intelligence;
                if (eyesightComp.thinkingCompletionTime > eyesightComp.thinkingTime) {
                    return;
                }

                cmdBuf.AddComponent<ReactingTargetComponent>(index, eyesightComp.target);

                cmdBuf.SetComponent(index, entity, new ForceStateComponent() {
                    state = (int) ForceState.None
                });
                cmdBuf.RemoveComponent<EyesightComponent>(index, entity);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            var job = new EyesightSystemJob() {
                deltaTime = Time.deltaTime,
                cmdBuf = _cmdSystem.CreateCommandBuffer().ToConcurrent()
            };

            var handle = job.Schedule(this, inputDependencies);
            _cmdSystem.AddJobHandleForProducer(handle);
            return handle;
        }
    }
}
