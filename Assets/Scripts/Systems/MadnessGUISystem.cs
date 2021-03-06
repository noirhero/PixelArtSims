﻿// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Entities;

using Components;

namespace Systems {
    public class MadnessGUISystem : ComponentSystem {
        protected override void OnUpdate() {
            Entities.ForEach((GUIPresetComponent guiPresetComp, ref AvatarComponent avatarComp) => {
                guiPresetComp.preset.madnessGaugeSlider.value = avatarComp.madness * 0.01f/* 0~100 => 0~1 */;
            });
        }
    }
}
