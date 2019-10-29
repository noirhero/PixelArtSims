// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using Components;

namespace Systems {
    public class EyesightSystem : JobComponentSystem {
        private EndSimulationEntityCommandBufferSystem _cmdSystem;

        protected override void OnCreate() {
            _cmdSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        struct EyesightSystemJob : IJobForEachWithEntity<Translation> {
            [ReadOnly] public ForceState avatarState;
            [ReadOnly] public float2 xPosAndDir;
            [ReadOnly] public AvatarPropertyComponent propComp;
            [ReadOnly] public ThinkingComponent thinkingComp;
            public Entity avatarEntity;
            public EntityCommandBuffer.Concurrent cmdBuf;

            public void Execute(Entity entity, int index, [ReadOnly] ref Translation posComp) {
                if (entity == avatarEntity || ForceState.None != avatarState || thinkingComp.findEntity == entity) {
                    return;
                }

                var atEntity = posComp.Value.x - xPosAndDir.x;
                if (0.0f < xPosAndDir.y && 0.0f > atEntity || 0.0f > xPosAndDir.y && 0.0f < atEntity) {
                    // Looking direction different.
                    return;
                }

                var distance = math.abs(atEntity);
                if (thinkingComp.entityToDistance <= distance || propComp.eyesight < distance) {
                    // Too long distance.
                    return;
                }

                cmdBuf.SetComponent(index, avatarEntity, new ThinkingComponent() {
                    findEntity = entity,
                    entityToDistance = distance,
                    thinkingTime = propComp.intelligence
                });
                cmdBuf.SetComponent(index, avatarEntity, new ForceStateComponent() {
                    state = (int)ForceState.Thinking,
                });
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            var job = new EyesightSystemJob() {
                cmdBuf = _cmdSystem.CreateCommandBuffer().ToConcurrent()
            };

            var entities = EntityManager.GetAllEntities();
            foreach(var entity in entities.Where(entity => 
                EntityManager.HasComponent(entity, typeof(PlayerAvatarComponent)))
            ) {
                job.avatarState = (ForceState)EntityManager.GetComponentData<ForceStateComponent>(entity).state;
                job.xPosAndDir.x = EntityManager.GetComponentData<Translation>(entity).Value.x;
                job.xPosAndDir.y = EntityManager.GetComponentData<VelocityComponent>(entity).xValue;
                job.propComp = EntityManager.GetComponentData<AvatarPropertyComponent>(entity);
                job.thinkingComp = EntityManager.GetComponentData<ThinkingComponent>(entity);
                job.avatarEntity = entity;
            }
            entities.Dispose();

            var handle = job.Schedule(this, inputDependencies);
            _cmdSystem.AddJobHandleForProducer(handle);

            return handle;
        }
    }
}
