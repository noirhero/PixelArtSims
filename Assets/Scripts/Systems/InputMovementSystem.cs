// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System.ComponentModel;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

using Components;

namespace Systems {
    public class InputMovementSystem : JobComponentSystem {
        struct InputMovementSystemJob : IJobForEach<ForceStateComponent, Translation, VelocityComponent> {
            public float deltaTime;

            public void Execute(ref ForceStateComponent forceStateComp, ref Translation posComp, ref VelocityComponent velocityComp) {
                if (ForceState.None != (ForceState)forceStateComp.state) {
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

                if (math.FLT_MIN_NORMAL >= math.abs(velocityComp.velocity.x)) {
                    return;
                }

                posComp.Value.x += velocityComp.velocity.x * deltaTime;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            return new InputMovementSystemJob() {
                deltaTime = Time.deltaTime
            }.Schedule(this, inputDependencies);
        }
    }
}
