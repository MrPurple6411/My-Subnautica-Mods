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
            yield return task1;
            GameObject gameObject1 = GameObject.Instantiate(task1.GetResult());
            Pickupable pickupable1 = gameObject1.GetComponent<Pickupable>();

            CoroutineTask<GameObject> task2 = CraftData.GetPrefabForTechTypeAsync(TechType.Battery);
            yield return task2;
            GameObject gameObject2 = GameObject.Instantiate(task2.GetResult());
            Pickupable pickupable2 = gameObject2.GetComponent<Pickupable>();

#if SUBNAUTICA_EXP

            TaskResult<Pickupable> task3 = new TaskResult<Pickupable>();
            yield return pickupable1.PickupAsync(task3, false);
            yield return task3;
            pickupable1 = task3.Get();

            TaskResult<Pickupable> task4 = new TaskResult<Pickupable>();
            yield return pickupable2.PickupAsync(task4, false);
            yield return task4;
            pickupable2 = task4.Get();
#else
            pickupable1.Pickup(false);
            pickupable2.Pickup(false);
#endif
            ScannerTool scannerTool = pickupable1?.GetComponent<ScannerTool>();
            scannerTool?.energyMixin?.batterySlot?.AddItem(pickupable2);

            Inventory.main.container.AddItem(pickupable1);
            yield break;
        }
    }

}