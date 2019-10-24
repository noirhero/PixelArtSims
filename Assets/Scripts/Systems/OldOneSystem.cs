﻿// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using Components;

namespace Systems {
    public class OldOneSystem : JobComponentSystem {
        private EndSimulationEntityCommandBufferSystem _cmdSystem;

        protected override void OnCreate() {
            _cmdSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        struct OldOneSystemJob : IJobForEach<Translation, OldOneComponent> {
            [ReadOnly] public float deltaTime;
            [ReadOnly] public float playerPosX;
            [ReadOnly] public float playerDirX;
            [ReadOnly] public AvatarPropertyComponent avatarComp;
            public Entity playerEntity;
            public EntityCommandBuffer cmdBuf;

            public void Execute([ReadOnly] ref Translation posComp, [ReadOnly] ref OldOneComponent oldOneComp) {
                var at = posComp.Value.x - playerPosX;
                if (avatarComp.eyesight < math.abs(at)) {
                    // Not in eyesight.
                    return;
                }

                if (0.0f < playerDirX && 0.0f > at || 0.0f > playerDirX && 0.0f < at) {
                    // Looking direction different.
                    return;
                }
                
                cmdBuf.SetComponent(playerEntity, new AvatarPropertyComponent() {
                    eyesight = avatarComp.eyesight,
                    madness = math.min(avatarComp.madness + oldOneComp.madness * deltaTime, 100.0f)
                });
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            var job = new OldOneSystemJob() {
                deltaTime = Time.deltaTime,
                cmdBuf = _cmdSystem.CreateCommandBuffer()
            };

            var entities = EntityManager.GetAllEntities();
            foreach (var entity in entities.Where(entity =>
                EntityManager.HasComponent(entity, typeof(PlayerAvatarComponent))
            )) {
                job.playerPosX = EntityManager.GetComponentData<Translation>(entity).Value.x;
                job.playerDirX = EntityManager.GetComponentData<VelocityComponent>(entity).xValue;
                job.avatarComp = EntityManager.GetComponentData<AvatarPropertyComponent>(entity);
                job.playerEntity = entity;
            }
            entities.Dispose();

            var handle = job.ScheduleSingle(this, inputDependencies);
            _cmdSystem.AddJobHandleForProducer(handle);

            return handle;
        }
    }
}
