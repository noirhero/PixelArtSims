// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System.Linq;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

using Components;

namespace Systems {
    public class ItemStorageSystem : JobComponentSystem {
        private EndSimulationEntityCommandBufferSystem _cmdSystem;

        protected override void OnCreate() {
            _cmdSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        struct ItemStorageSystemJob : IJobForEachWithEntity<Translation, ItemStorageComponent> {
            [ReadOnly] public float deltaTime;
            [ReadOnly] public float playerPosX;
            [ReadOnly] public ForceStateComponent playerForceState;
            [ReadOnly] public Entity playerAvatar;
            public EntityCommandBuffer.Concurrent cmdBuf;

            public void Execute(Entity entity, int index, [ReadOnly] ref Translation posComp,
                ref ItemStorageComponent itemStorageComp) {
                if (0 == itemStorageComp.index) {
                    return;
                }

                if (playerForceState.useEntity == entity) {
                    var lifeTime = playerForceState.time - deltaTime;
                    if (0.0f >= lifeTime) {
                        itemStorageComp.index = 0;

                        cmdBuf.SetComponent(index, playerAvatar, new ForceStateComponent() {
                            useEntity = Entity.Null,
                            state = (int) ForceState.None
                        });
                    }
                    else {
                        cmdBuf.SetComponent(index, playerAvatar, new ForceStateComponent() {
                            useEntity = entity,
                            time = lifeTime,
                            state = (int) ForceState.Item
                        });
                    }
                }
                else if (playerForceState.useEntity == Entity.Null) {
                    var atPlayerPosDelta = math.abs(playerPosX - posComp.Value.x);
                    if (itemStorageComp.checkRadius < atPlayerPosDelta) {
                        return;
                    }

                    cmdBuf.SetComponent(index, playerAvatar, new ForceStateComponent() {
                        useEntity = entity,
                        time = itemStorageComp.forceStateTime,
                        state = (int) ForceState.Item
                    });
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            var job = new ItemStorageSystemJob() {
                deltaTime = Time.deltaTime,
                cmdBuf = _cmdSystem.CreateCommandBuffer().ToConcurrent()
            };

            var entities = EntityManager.GetAllEntities();
            foreach (var entity in entities.Where(entity =>
                EntityManager.HasComponent(entity, typeof(PlayerAvatarComponent)))) {
                job.playerAvatar = entity;
                job.playerPosX = EntityManager.GetComponentData<Translation>(entity).Value.x;
                job.playerForceState = EntityManager.GetComponentData<ForceStateComponent>(entity);
                break;
            }
            entities.Dispose();

            var handle = job.Schedule(this, inputDependencies);
            _cmdSystem.AddJobHandleForProducer(handle);

            return handle;
        }
    }
}
