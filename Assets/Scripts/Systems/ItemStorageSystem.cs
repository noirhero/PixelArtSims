﻿// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

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

        [RequireComponentTag(typeof(ReactingItemComponent))]
        struct ItemStorageSystemJob : IJobForEachWithEntity<Translation, ItemStorageComponent> {
            [ReadOnly] public float deltaTime;
            [ReadOnly] public float playerPosX;
            [ReadOnly] public float agility;
            [ReadOnly] public ForceStateComponent playerForceState;
            [ReadOnly] public InventoryComponent inventory;
            public Entity playerEntity;
            public EntityCommandBuffer.Concurrent cmdBuf;

            public void Execute(Entity entity, int index, [ReadOnly] ref Translation posComp, ref ItemStorageComponent itemStorageComp) {
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
                cmdBuf.SetComponent(index, playerEntity, new ForceStateComponent() {
                    state = (int) ForceState.None
                });

                cmdBuf.DestroyEntity(index, entity);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            var job = new ItemStorageSystemJob() {
                deltaTime = Time.deltaTime,
                cmdBuf = _cmdSystem.CreateCommandBuffer().ToConcurrent()
            };

            var entities = EntityManager.GetAllEntities();
            foreach (var entity in entities.Where(entity =>
                EntityManager.HasComponent(entity, typeof(PlayerAvatarComponent))
            )) {
                job.playerEntity = entity;
                job.playerPosX = EntityManager.GetComponentData<Translation>(entity).Value.x;
                job.agility = EntityManager.GetComponentData<AvatarPropertyComponent>(entity).agility;
                job.playerForceState = EntityManager.GetComponentData<ForceStateComponent>(entity);
                job.inventory = EntityManager.GetComponentData<InventoryComponent>(entity);
                break;
            }
            entities.Dispose();

            var handle = job.Schedule(this, inputDependencies);
            _cmdSystem.AddJobHandleForProducer(handle);

            return handle;
        }
    }
}
