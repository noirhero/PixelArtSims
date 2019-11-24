// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

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
            [ReadOnly] public Entity target;
            [ReadOnly] public float playerPosX;
            [ReadOnly] public float agility;
            [ReadOnly] public InventoryComponent inventory;
            public Entity playerEntity;
            public EntityCommandBuffer.Concurrent cmdBuf;

            public void Execute(Entity entity, int index, [ReadOnly] ref Translation posComp, ref ItemStorageComponent itemStorageComp) {
                if (target != entity) {
                    return;
                }

                var atPlayerPosDelta = math.abs(playerPosX - posComp.Value.x);
                if (itemStorageComp.checkRadius < atPlayerPosDelta) {
                    return;
                }

                itemStorageComp.gettingTime -= (agility * deltaTime);
                if (0.0f < itemStorageComp.gettingTime) {
                    cmdBuf.SetComponent(index, playerEntity, new ForceStateComponent() {
                        state = (int) ForceState.Item
                    });
                    return;
                }

                var newInventoryComp = inventory;
                newInventoryComp.currentGettingItem = itemStorageComp.index;
                cmdBuf.SetComponent(index, playerEntity, newInventoryComp);
                cmdBuf.RemoveComponent<TargetComponent>(index, playerEntity);
                cmdBuf.SetComponent(index, playerEntity, new ForceStateComponent() {
                    state = (int) ForceState.None
                });

                cmdBuf.DestroyEntity(index, entity);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            var playerEntity = Entity.Null;
            var entities = EntityManager.GetAllEntities();
            for (var i = 0; i < entities.Length; ++i) {
                var eachEntity = entities[i];
                if (EntityManager.HasComponent<PlayerAvatarComponent>(eachEntity)) {
                    playerEntity = eachEntity;
                    break;
                }
            }
            entities.Dispose();

            if (Entity.Null == playerEntity || false == EntityManager.HasComponent<TargetComponent>(playerEntity)) {
                return inputDependencies;
            }

            var job = new ItemStorageSystemJob() {
                deltaTime = Time.deltaTime,
                target = EntityManager.GetComponentData<TargetComponent>(playerEntity).target,
                playerPosX = EntityManager.GetComponentData<Translation>(playerEntity).Value.x,
                agility = EntityManager.GetComponentData<PlayerAvatarComponent>(playerEntity).agility,
                inventory = EntityManager.GetComponentData<InventoryComponent>(playerEntity),
                playerEntity = playerEntity,
                cmdBuf = _cmdSystem.CreateCommandBuffer().ToConcurrent()
            };

            var handle = job.Schedule(this, inputDependencies);
            _cmdSystem.AddJobHandleForProducer(handle);

            return handle;
        }
    }
}
