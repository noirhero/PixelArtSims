// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

using Components;

namespace Systems {
    public class InputMovementSystem : ComponentSystem {
        protected override void OnUpdate() {
            var delta = Time.deltaTime;
            Entities.ForEach((ref ForceStateComponent forceStateComp, ref VelocityComponent velocityComp) => {
                if (ForceState.None != (ForceState) forceStateComp.state) {
                    velocityComp.velocity.x = 0.0f;
                    return;
                }

                var currentKeyboard = Keyboard.current;
                if (null == currentKeyboard) {
                    return;
                }

                velocityComp.velocity = float2.zero;
                if (currentKeyboard.dKey.isPressed) {
                    velocityComp.velocity.x += 1.0f;
                    velocityComp.xValue = 1.0f;
                }

                if (currentKeyboard.aKey.isPressed) {
                    velocityComp.velocity.x -= 1.0f;
                    velocityComp.xValue = -1.0f;
                }
            });
        }
    }
}
