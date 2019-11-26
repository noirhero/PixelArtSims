// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

using Components;

namespace Systems {
    public class SearchSystem : JobComponentSystem {
        private EntityCommandBufferSystem _cmdSystem = null;
        protected override void OnCreate() {
            _cmdSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        [ExcludeComponent(typeof(ReactingTargetComponent))]
        struct SearchSystemJob : IJobForEachWithEntity<ReactiveComponent, Translation> {
            [ReadOnly] public float xPos;
            [ReadOnly] public float xDir;
            [ReadOnly] public PlayerAvatarComponent propComp;
            public Entity playerEntity;
            public EntityCommandBuffer.Concurrent cmdBuf;

            public void Execute(Entity entity, int index, [ReadOnly] ref ReactiveComponent reactiveComp, [ReadOnly] ref Translation posComp) {
                if (reactiveComp.search > propComp.search) {
                    // Not in search.
                    return;
                }

                var at = posComp.Value.x - xPos;
                var toDistance = math.abs(at);
                if (propComp.eyesight < toDistance) {
                    // Not in eyesight.
                    return;
                }

                if (0.0f < xDir && 0.0f > at || 0.0f > xDir && 0.0f < at) {
                    // Looking direction different.
                    return;
                }

                var thinkingCompletionTime = 1.0f;
                switch((ReactiveType) reactiveComp.type) {
                    case ReactiveType.Item : thinkingCompletionTime = 1.5f; break;
                    case ReactiveType.Wall : thinkingCompletionTime = 0.5f; break;
                    case ReactiveType.Something :
                        thinkingCompletionTime = 3.0f;
                        cmdBuf.AddComponent(index, playerEntity, new MadnessComponent() {
                            value = reactiveComp.madness
                        });
                        break;
                };

                cmdBuf.SetComponent(index, playerEntity, new ForceStateComponent() {
                    state = (int) ForceState.Thinking
                });
                cmdBuf.AddComponent(index, playerEntity, new EyesightComponent() {
                    target = entity,
                    thinkingCompletionTime = thinkingCompletionTime
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

            if (Entity.Null == playerEntity || EntityManager.HasComponent<EyesightComponent>(playerEntity)) {
                return inputDependencies;
            }
            
            var job = new SearchSystemJob() {
                xPos = EntityManager.GetComponentData<Translation>(playerEntity).Value.x,
                xDir = EntityManager.GetComponentData<VelocityComponent>(playerEntity).xValue,
                propComp = EntityManager.GetComponentData<PlayerAvatarComponent>(playerEntity),
                playerEntity = playerEntity,
                cmdBuf = _cmdSystem.CreateCommandBuffer().ToConcurrent()
            };

            var handle = job.Schedule(this, inputDependencies);
            _cmdSystem.AddJobHandleForProducer(handle);
            return handle;
        }
    }
}
