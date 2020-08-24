using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ChargeRequired.Patches
{
    internal class ClosestItemContainers_Patches
    {
        public static bool ClosestItemContainers_GetPickupCount_Prefix(TechType techType, ref int __result)
        {
            int num = 0;
            foreach (ItemsContainer itemsContainer in Main.containers.GetValue(null) as ItemsContainer[])
            {
                var items = new List<InventoryItem>();
                itemsContainer.GetItems(techType, items);
                foreach (InventoryItem item in items)
                {
                    if (Main.BatteryCheck(item.item))
                    {
                        num++;
                    }
                }
            }
            __result = num;
            return false;
        }

        public static bool ClosestItemContainers_DestroyItem_Prefix(TechType techType, ref bool __result, int count = 1)
        {
            int num = 0;
            foreach (ItemsContainer itemsContainer in Main.containers.GetValue(null) as ItemsContainer[])
            {
                var items = new List<InventoryItem>();
                itemsContainer.GetItems(techType, items);
                foreach (InventoryItem item in items)
                {
                    if (Main.BatteryCheck(item.item))
                    {
                        if (itemsContainer.RemoveItem(item.item))
                        {
                            UnityEngine.Object.Destroy(item.item.gameObject);
                            num++;
                        }
                    }

                    if (num == count)
                    {
                        break;
                    }
                }

                if (num == count)
                {
                    break;
                }
            }
            if (num < count)
            {
                Console.WriteLine(string.Format("[EasyCraft] Unable to remove {0} {1}", count, techType));
                __result = false;
                return false;
            }

            __result = true;
            Console.WriteLine(string.Format("[EasyCraft] removed {0} {1}", count, techType));
            return false;
        }

    }
}