// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

using Components;

namespace Systems {
    public class EyesightSystem : JobComponentSystem {
        private EntityCommandBufferSystem _cmdSystem = null;
        protected override void OnCreate() {
            _cmdSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        struct EyesightSystemJob : IJobForEachWithEntity<ReactiveComponent, Translation> {
            [ReadOnly] public float xPos;
            [ReadOnly] public float xDir;
            [ReadOnly] public AvatarPropertyComponent propComp;
            [ReadOnly] public IntelligenceComponent intelligenceComp;
            public Entity playerEntity;
            public EntityCommandBuffer.Concurrent cmdBuf;

            public void Execute(Entity entity, int index, [ReadOnly] ref ReactiveComponent reactiveComp, [ReadOnly] ref Translation posComp) {
                if (intelligenceComp.inEyesightEntity == entity) {
                    return;
                }

                var at = posComp.Value.x - xPos;
                var toDistance = math.abs(at);
                if (propComp.eyesight < toDistance || intelligenceComp.inEyesightEntityToDistance <= toDistance) {
                    // Not in eyesight.
                    return;
                }

                if (0.0f < xDir && 0.0f > at || 0.0f > xDir && 0.0f < at) {
                    // Looking direction different.
                    return;
                }

                var thinkingTime = propComp.intelligence;
                switch ((ReactiveType) reactiveComp.type) {
                    case ReactiveType.Item: thinkingTime *= 0.5f;
                        break;
                    case ReactiveType.Wall: thinkingTime *= 0.25f;
                        break;
                    case ReactiveType.Something: thinkingTime *= 3.0f;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                cmdBuf.SetComponent(index, playerEntity, new IntelligenceComponent() {
                    inEyesightEntity = entity,
                    inEyesightEntityReactiveType = reactiveComp.type,
                    inEyesightEntityToDistance = toDistance,
                    inEyesightEntityThinkingTime = thinkingTime
                });
                cmdBuf.SetComponent(index, playerEntity, new ForceStateComponent() {
                    state = (int) ForceState.Thinking
                });
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            Entity playerEntity = Entity.Null;

            var entities = EntityManager.GetAllEntities();
            foreach (var entity in entities) {
                if (EntityManager.HasComponent<PlayerAvatarComponent>(entity)) {
                    playerEntity = entity;
                    break;
                }
            }
            entities.Dispose();

            if (Entity.Null == playerEntity) {
                return new JobHandle();
            }
            
            var job = new EyesightSystemJob() {
                xPos = EntityManager.GetComponentData<Translation>(playerEntity).Value.x,
                xDir = EntityManager.GetComponentData<VelocityComponent>(playerEntity).xValue,
                propComp = EntityManager.GetComponentData<AvatarPropertyComponent>(playerEntity),
                intelligenceComp = EntityManager.GetComponentData<IntelligenceComponent>(playerEntity),
                playerEntity = playerEntity,
                cmdBuf = _cmdSystem.CreateCommandBuffer().ToConcurrent()
            };

            var handle = job.Schedule(this, inputDependencies);
            _cmdSystem.AddJobHandleForProducer(handle);
            return handle;
        }
    }
}