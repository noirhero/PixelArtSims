// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Components;
using Preset;
using Unity.Entities;

namespace Systems {
    public class InventoryGUISystem : ComponentSystem {
        protected override void OnUpdate() {
            Entities.ForEach((GUIPresetComponent guiPrestComp, ref InventoryComponent inventoryComp) => {
                GUIPreset guiPreset = guiPrestComp.preset;

                if (0 == inventoryComp.item01 && guiPreset.items[0].gameObject.activeSelf) {
                    guiPreset.items[0].gameObject.SetActive(false);
                }
                else if (0 != inventoryComp.item01 && false == guiPreset.items[0].gameObject.activeSelf) {
                    guiPreset.items[0].sprite = guiPreset.itemSprites[0];
                    guiPreset.items[0].gameObject.SetActive(true);
                }
            });
        }
    }
}
