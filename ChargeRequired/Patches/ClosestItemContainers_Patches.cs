// ReSharper disable RedundantAssignment

using System.Linq;

namespace ChargeRequired.Patches
{
    using System;
    using System.Collections.Generic;

    internal class ClosestItemContainers_Patches
    {
        public static bool ClosestItemContainers_GetPickupCount_Prefix(TechType techType, ref int __result)
        {
            var num = 0;
            foreach(var itemsContainer in Main.containers.GetValue(null) as ItemsContainer[] ?? new ItemsContainer[0])
            {
                var items = new List<InventoryItem>();
                itemsContainer.GetItems(techType, items);
                num += items.Count(item => Main.BatteryCheck(item.item));
            }
            __result = num;
            return false;
        }

        public static bool ClosestItemContainers_DestroyItem_Prefix(TechType techType, ref bool __result, int count = 1)
        {
            var num = 0;
            foreach(var itemsContainer in Main.containers.GetValue(null) as ItemsContainer[] ?? new ItemsContainer[0])
            {
                var items = new List<InventoryItem>();
                itemsContainer.GetItems(techType, items);
                foreach(var item in items)
                {
                    if(Main.BatteryCheck(item.item))
                    {
                        if(itemsContainer.RemoveItem(item.item))
                        {
                            UnityEngine.Object.Destroy(item.item.gameObject);
                            num++;
                        }
                    }

                    if(num == count)
                    {
                        break;
                    }
                }

                if(num == count)
                {
                    break;
                }
            }
            if(num < count)
            {
                Console.WriteLine($"[EasyCraft] Unable to remove {count} {techType}");
                __result = false;
                return false;
            }

            __result = true;
            Console.WriteLine($"[EasyCraft] removed {count} {techType}");
            return false;
        }

    }
}