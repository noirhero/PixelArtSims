// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

using Components;

namespace Systems {
    [UpdateAfter(typeof(InputMovementSystem))]
    public class WallSystem : ComponentSystem {
        protected override void OnUpdate() {
            var playerAvatarEntity = Entity.Null;
            Entities.WithAll<PlayerAvatarComponent>().ForEach((Entity entity) => playerAvatarEntity = entity);
            if (Entity.Null == playerAvatarEntity) {
                return;
            }

            Entities.ForEach((ref WallComponent wallComp) => {
                var playerPos = EntityManager.GetComponentData<Translation>(playerAvatarEntity).Value;
                var boxPosX = wallComp.pos.x + wallComp.offset.x;
                var posAtLength = math.abs(boxPosX - playerPos.x);
                var halfBoxSizeX = wallComp.size.x * 0.5f;
                if (halfBoxSizeX <= posAtLength) {
                    return;
                }

                float at = (wallComp.pos.x < playerPos.x) ? 1.0f : -1.0f;
                playerPos.x = boxPosX + at * halfBoxSizeX;

                EntityManager.SetComponentData(playerAvatarEntity, new Translation() {
                    Value = playerPos
                });
            });
        }
    }
}
