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
            [ReadOnly] public AvatarComponent propComp;
            public Entity avatarEntity;
            public EntityCommandBuffer.Concurrent cmdBuf;

            public void Execute(Entity entity, int index, [ReadOnly] ref ReactiveComponent reactiveComp, [ReadOnly] ref Translation posComp) {
                if (avatarEntity == entity) {
                    return;
                }

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

                var thinkingTime = 1.0f;
                switch((ReactiveType) reactiveComp.type) {
                    case ReactiveType.Item : thinkingTime = 1.5f; break;
                    case ReactiveType.Wall : thinkingTime = 0.5f; break;
                    case ReactiveType.Something :
                        thinkingTime = 3.0f;
                        cmdBuf.AddComponent(index, avatarEntity, new MadnessComponent() {
                            value = reactiveComp.madness
                        });
                        break;
                };

                cmdBuf.SetComponent(index, avatarEntity, new ForceStateComponent() {
                    state = (int) ForceState.Thinking
                });
                cmdBuf.AddComponent(index, avatarEntity, new EyesightComponent() {
                    target = entity,
                    thinkingTime = thinkingTime
                });
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            Entity avatarEntity = Entity.Null;

            var entities = EntityManager.GetAllEntities();
            foreach (var entity in entities) {
                if (EntityManager.HasComponent<AvatarComponent>(entity)) {
                    avatarEntity = entity;
                    break;
                }
            }
            entities.Dispose();

            if (Entity.Null == avatarEntity || EntityManager.HasComponent<EyesightComponent>(avatarEntity)) {
                return inputDependencies;
            }
            
            var job = new SearchSystemJob() {
                xPos = EntityManager.GetComponentData<Translation>(avatarEntity).Value.x,
                xDir = EntityManager.GetComponentData<VelocityComponent>(avatarEntity).xValue,
                propComp = EntityManager.GetComponentData<AvatarComponent>(avatarEntity),
                avatarEntity = avatarEntity,
                cmdBuf = _cmdSystem.CreateCommandBuffer().ToConcurrent()
            };

            var handle = job.Schedule(this, inputDependencies);
            _cmdSystem.AddJobHandleForProducer(handle);
            return handle;
        }
    }
}
