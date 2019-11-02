// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

using Components;
using Preset;

namespace Systems {
    public class ForceStateGUISystem : ComponentSystem {
        private ForceState _oldState = ForceState.None;

        protected override void OnUpdate() {
            var playerPos = new float3();
            Entities.ForEach((ref PlayerAvatarComponent playerComp, ref Translation posComp) =>
                playerPos = posComp.Value);

            var screenPosX = 0.0f;
            Entities.ForEach((CameraObjectComponent cameraComp) => {
                screenPosX = cameraComp.camera.WorldToScreenPoint(playerPos).x;
            });

            Entities.ForEach((GUIPresetComponent guiPresetComp, ref ForceStateComponent forceStateComp) => {
                GUIPreset guiPreset = guiPresetComp.preset;

                if (ForceState.None == _oldState && ForceState.None != (ForceState) forceStateComp.state) {
                    guiPreset.forceStateBalloon.SetActive(true);
                }
                else if (ForceState.None != _oldState && ForceState.None == (ForceState) forceStateComp.state) {
                    guiPreset.forceStateBalloon.SetActive(false);
                }
                _oldState = (ForceState) forceStateComp.state;

                if (guiPreset.forceStateBalloon.activeSelf) {
                    var rectTransform = guiPreset.forceStateBalloon.GetComponent<RectTransform>();
                    Debug.Assert(null != rectTransform);

                    rectTransform.anchoredPosition = new Vector2(screenPosX / guiPreset.canvas.scaleFactor, rectTransform.anchoredPosition.y);
                }
            });
        }
    }
}
