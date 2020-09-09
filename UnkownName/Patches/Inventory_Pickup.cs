using HarmonyLib;
using System.Collections;
using UnityEngine;
using UWE;

namespace UnKnownName.Patches
{
#if SUBNAUTICA_EXP
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.PickupAsync))]
#else
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.Pickup))]
#endif
    public class Inventory_Pickup
    {
        public static bool newgame = true;

        [HarmonyPostfix]
        public static void Postfix(Pickupable pickupable)
        {
            if (newgame && Main.config.Hardcore && !Utils.GetContinueMode() && pickupable.GetTechType() != TechType.FireExtinguisher)
            {
                CoroutineHost.StartCoroutine(GiveHardcoreScanner());
                newgame = false;
                SMLHelper.V2.Handlers.IngameMenuHandler.RegisterOnQuitEvent(() => newgame = true);
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
#if SN1
                    ResourceTracker.UpdateFragments();
#endif
                }
            }

            if (!Main.config.Hardcore && entryData == null)
            {
                KnownTech.Add(techType, true);
            }
        }

        private static IEnumerator GiveHardcoreScanner()
        {
            CoroutineTask<GameObject> task1 = CraftData.GetPrefabForTechTypeAsync(TechType.Scanner);
            CoroutineTask<GameObject> task2 = CraftData.GetPrefabForTechTypeAsync(TechType.Battery);

            yield return task1;
            yield return task2;

            Pickupable pickupable1 = task1.GetResult().GetComponent<Pickupable>();
            Pickupable pickupable2 = task2.GetResult().GetComponent<Pickupable>();

#if SUBNAUTICA_EXP
            TaskResult<Pickupable> task3 = new TaskResult<Pickupable>();
            TaskResult<Pickupable> task4 = new TaskResult<Pickupable>();

            yield return pickupable1.PickupAsync(task3, false);
            yield return pickupable2.PickupAsync(task4, false);
#else
            pickupable1.Pickup(false);
            pickupable2.Pickup(false);
#endif
            ScannerTool scannerTool = pickupable1.GetComponent<ScannerTool>();
            scannerTool.energyMixin.batterySlot.AddItem(pickupable2);

            Inventory.main.container.AddItem(pickupable1);

            yield break;
        }
    }

}