// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using UnityEngine;

using Components;

namespace Systems {
    [UpdateAfter(typeof(EyesightSystem))]
    public class IntelligenceSystem : ComponentSystem {
        protected override void OnUpdate() {
            var deltaTime = Time.deltaTime;

            Entities.WithAll<PlayerAvatarComponent>().ForEach((Entity playerEntity,
                ref IntelligenceComponent intelligenceComp, ref ForceStateComponent forceStateComp,
                ref VelocityComponent velocityComp) => {
                if (Entity.Null == intelligenceComp.inEyesightEntity) {
                    return;
                }

                intelligenceComp.inEyesightEntityThinkingTime -= deltaTime;
                if (0.0f < intelligenceComp.inEyesightEntityThinkingTime) {
                    return;
                }

                switch ((ReactiveType) intelligenceComp.inEyesightEntityReactiveType) {
                    case ReactiveType.Item:
                        EntityManager.RemoveComponent<ReactiveComponent>(intelligenceComp.inEyesightEntity);
                        EntityManager.AddComponentData(intelligenceComp.inEyesightEntity, new ReactingItemComponent());

                        intelligenceComp.inEyesightEntity = Entity.Null;
                        intelligenceComp.inEyesightEntityToDistance = float.MaxValue;
                        forceStateComp.state = (int) ForceState.None;
                        break;
                    case ReactiveType.Wall:
                    case ReactiveType.Something:
                        intelligenceComp.inEyesightEntity = Entity.Null;
                        intelligenceComp.inEyesightEntityToDistance = float.MaxValue;
                        forceStateComp.state = (int) ForceState.None;
                        velocityComp.xValue *= -1.0f;
                        break;
                }
            });
        }
    }
}
