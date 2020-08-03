using HarmonyLib;
using UnityEngine;

namespace UnKnownName
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.Pickup))]
    public class Inventory_Pickup
    {
        public static bool newgame = true;

        [HarmonyPostfix]
        public static void Postfix(Pickupable pickupable)
        {
            if (newgame && Main.config.Hardcore && !Utils.GetContinueMode() && pickupable.GetTechType() != TechType.FireExtinguisher)
            {
                Pickupable pickupable1 = CraftData.InstantiateFromPrefab(TechType.Scanner).GetComponent<Pickupable>();
                ScannerTool scannerTool = pickupable1.GetComponent<ScannerTool>();
                Pickupable pickupable2 = CraftData.InstantiateFromPrefab(TechType.Battery).GetComponent<Pickupable>();
                pickupable1.Pickup(false);
                pickupable2.Pickup(false);
                scannerTool.energyMixin.batterySlot.AddItem(pickupable2);
                Inventory.main.container.AddItem(pickupable1);
                newgame = false;
            }

            TechType techType = pickupable.GetTechType();
            PDAScanner.EntryData entryData = PDAScanner.GetEntryData(techType);
            GameObject gameObject = pickupable.gameObject;
            if (Main.config.ScanOnPickup && Inventory.main.container.Contains(TechType.Scanner) && entryData != null)
            {
                if (!PDAScanner.GetPartialEntryByKey(techType, out PDAScanner.Entry entry))
                {
                    entry = PDAScanner.Add(techType, 1);
                }
                if (entry != null)
                {
                    PDAScanner.partial.Remove(entry);
                    PDAScanner.complete.Add(entry.techType);
                    PDAScanner.NotifyRemove(entry);
                    PDAScanner.Unlock(entryData, true, true, true);
                    KnownTech.Add(techType, false);
                    if (gameObject != null)
                    {
                        gameObject.SendMessage("OnScanned", null, SendMessageOptions.DontRequireReceiver);
                    }
#if SUBNAUTICA
                    ResourceTracker.UpdateFragments();
#endif
                }
            }

            if (!Main.config.Hardcore && entryData == null)
            {
                KnownTech.Add(techType, true);
            }
        }
    }

}