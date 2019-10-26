// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Systems {
    public class IntelligenceSystem : JobComponentSystem {
        private EntityCommandBufferSystem _cmdBufSystem = null;

        protected override void OnCreate() {
            _cmdBufSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        [BurstCompile]
        struct IntelligenceSystemJob : IJobForEach<Translation> {
            public EntityCommandBuffer cmdBuf;
            public void Execute([ReadOnly] ref Translation posComp) {
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            var job = new IntelligenceSystemJob() {
                cmdBuf = _cmdBufSystem.CreateCommandBuffer()
            };

            var handle = job.ScheduleSingle(this, inputDependencies);
            _cmdBufSystem.AddJobHandleForProducer(handle);

            return handle;
        }
    }
}
