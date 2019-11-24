// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System.Linq;
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
        
        struct OldOneSystemJob : IJobForEachWithEntity<Translation, OldOneComponent> {
            [ReadOnly] public float deltaTime;
            [ReadOnly] public float playerPosX;
            [ReadOnly] public float playerDirX;
            [ReadOnly] public PlayerAvatarComponent avatarComp;
            public Entity playerEntity;
            public EntityCommandBuffer.Concurrent cmdBuf;

            public void Execute(Entity entity, int index, [ReadOnly] ref Translation posComp, [ReadOnly] ref OldOneComponent oldOneComp) {
                var at = posComp.Value.x - playerPosX;
                if (avatarComp.eyesight < math.abs(at)) {
                    // Not in eyesight.
                    return;
                }

                if (0.0f < playerDirX && 0.0f > at || 0.0f > playerDirX && 0.0f < at) {
                    // Looking direction different.
                    return;
                }

                var newAvatarComp = avatarComp;
                newAvatarComp.madness = math.min(avatarComp.madness + oldOneComp.madness * deltaTime, 100.0f);
                cmdBuf.SetComponent(index, playerEntity, newAvatarComp);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            var playerEntity = Entity.Null;
            var entities = EntityManager.GetAllEntities();
            for (var i = 0; i < entities.Length; ++i) {
                var entity = entities[i];
                if (EntityManager.HasComponent<PlayerAvatarComponent>(entity)) {
                    playerEntity = entity;
                    break;
                }
            }
            entities.Dispose();

            if (Entity.Null == playerEntity) {
                return inputDependencies;
            }

            var job = new OldOneSystemJob() {
                deltaTime = Time.deltaTime,
                playerPosX = EntityManager.GetComponentData<Translation>(playerEntity).Value.x,
                playerDirX = EntityManager.GetComponentData<VelocityComponent>(playerEntity).xValue,
                avatarComp = EntityManager.GetComponentData<PlayerAvatarComponent>(playerEntity),
                playerEntity = playerEntity,
                cmdBuf = _cmdSystem.CreateCommandBuffer().ToConcurrent()
            };

            var handle = job.Schedule(this, inputDependencies);
            _cmdSystem.AddJobHandleForProducer(handle);

            return handle;
        }
    }
}
