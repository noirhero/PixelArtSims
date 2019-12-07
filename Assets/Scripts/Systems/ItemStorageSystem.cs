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

        [RequireComponentTag(typeof(ReactingTargetComponent))]
        struct ItemStorageSystemJob : IJobForEachWithEntity<Translation, ItemStorageComponent> {
            [ReadOnly] public float deltaTime;
            [ReadOnly] public float playerPosX;
            [ReadOnly] public float agility;
            [ReadOnly] public InventoryComponent inventory;
            public Entity avatarEntity;
            public EntityCommandBuffer.Concurrent cmdBuf;

            public void Execute(Entity entity, int index, [ReadOnly] ref Translation posComp, ref ItemStorageComponent itemStorageComp) {
                var atPlayerPosDelta = math.abs(playerPosX - posComp.Value.x);
                if (itemStorageComp.checkRadius < atPlayerPosDelta) {
                    return;
                }

                itemStorageComp.gettingTime -= (agility * deltaTime);
                if (0.0f < itemStorageComp.gettingTime) {
                    cmdBuf.SetComponent(index, avatarEntity, new ForceStateComponent() {
                        state = (int) ForceState.Item
                    });
                    return;
                }

                var newInventoryComp = inventory;
                newInventoryComp.currentGettingItem = itemStorageComp.index;
                cmdBuf.SetComponent(index, avatarEntity, newInventoryComp);
                cmdBuf.SetComponent(index, avatarEntity, new ForceStateComponent() {
                    state = (int) ForceState.None
                });

                cmdBuf.DestroyEntity(index, entity);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            var avatarEntity = Entity.Null;
            var entities = EntityManager.GetAllEntities();
            foreach(var entity in entities) {
                if (EntityManager.HasComponent<AvatarComponent>(entity)) {
                    avatarEntity = entity;
                    break;
                }
            }
            entities.Dispose();

            if (Entity.Null == avatarEntity) {
                return inputDependencies;
            }

            var job = new ItemStorageSystemJob() {
                deltaTime = Time.deltaTime,
                playerPosX = EntityManager.GetComponentData<Translation>(avatarEntity).Value.x,
                agility = EntityManager.GetComponentData<AvatarComponent>(avatarEntity).agility,
                inventory = EntityManager.GetComponentData<InventoryComponent>(avatarEntity),
                avatarEntity = avatarEntity,
                cmdBuf = _cmdSystem.CreateCommandBuffer().ToConcurrent()
            };

            var handle = job.Schedule(this, inputDependencies);
            _cmdSystem.AddJobHandleForProducer(handle);

            return handle;
        }
    }
}
