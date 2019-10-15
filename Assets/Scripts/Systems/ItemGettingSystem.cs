// Copyright 2018-2019 TAP, Inc. All Rights Reserved.

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Systems {
    public class ItemGettingSystem : JobComponentSystem {
        [BurstCompile]
        struct ItemGettingSystemJob : IJobForEach<InventoryComponent> {
            public void Execute(ref InventoryComponent inventoryComp) {
                if (0 == inventoryComp.currentGettingItem) {
                    return;
                }

                if (0 == inventoryComp.item01) {
                    inventoryComp.item01 = inventoryComp.currentGettingItem;
                }
                if (0 == inventoryComp.item02) {
                    inventoryComp.item02 = inventoryComp.currentGettingItem;
                }
                if (0 == inventoryComp.item03) {
                    inventoryComp.item03 = inventoryComp.currentGettingItem;
                }
                
                // Todo: Choose drop item.

                inventoryComp.currentGettingItem = 0;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies) {
            var job = new ItemGettingSystemJob();
            return job.Schedule(this, inputDependencies);
        }
    }
}
