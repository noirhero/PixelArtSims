// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

using Components;

namespace Systems {
    public class CameraMovementSystem : ComponentSystem {
        protected override void OnUpdate() {
            var deltaTime = Time.deltaTime;
            Entities.ForEach((ref PlayerAvatarComponent playerComp, ref Translation posComp) => {
                var posX = posComp.Value.x;
                Entities.ForEach((CameraObjectComponent cameraComp) => {
                    var position = cameraComp.cameraTransform.position;
                    var at = (posX - position.x) * deltaTime;

                    cameraComp.cameraTransform.position = new Vector3(position.x + at, position.y, position.z);
                });
            });
        }
    }
}
