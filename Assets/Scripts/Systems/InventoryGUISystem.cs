// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

using Components;
using Preset;

namespace Systems {
    public class InventoryGUISystem : ComponentSystem {
        private void ItemGUIProcess(Image itemImage, int itemIdx, List<Sprite> itemSprites) {
            var itemObject = itemImage.gameObject;
            
            if (0 == itemIdx && itemObject.activeSelf) {
                itemObject.SetActive(false);
            }
            else if (0 != itemIdx && false == itemObject.activeSelf) {
                itemImage.sprite = itemSprites[itemIdx - 1];
                itemObject.SetActive(true);
            }
        }

        protected override void OnUpdate() {
            Entities.ForEach((GUIPresetComponent guiPresetComp, ref InventoryComponent inventoryComp) => {
                GUIPreset guiPreset = guiPresetComp.preset;

                ItemGUIProcess(guiPreset.items[0], inventoryComp.item01, guiPreset.itemSprites);
                ItemGUIProcess(guiPreset.items[1], inventoryComp.item02, guiPreset.itemSprites);
                ItemGUIProcess(guiPreset.items[2], inventoryComp.item03, guiPreset.itemSprites);
            });
        }
    }
}
