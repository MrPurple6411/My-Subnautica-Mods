namespace UnKnownName.Patches
{
    using HarmonyLib;
    using System.Collections;
    using UnityEngine;
    using UWE;

#if SUBNAUTICA_EXP
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.PickupAsync))]
#else
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.Pickup))]
#endif
    public class Inventory_Pickup
    {
        private static bool newGame = true;

        [HarmonyPostfix]
        public static void Postfix(Pickupable pickupable)
        {
            if(newGame && Main.Config.Hardcore && !global::Utils.GetContinueMode() && !Player.main.IsInside())
            {
                CoroutineHost.StartCoroutine(GiveHardcoreScanner());
                newGame = false;
                SMLHelper.V2.Handlers.IngameMenuHandler.RegisterOnQuitEvent(() => newGame = true);
            }

            var techType = pickupable.GetTechType();
            var entryData = PDAScanner.GetEntryData(techType);
            var gameObject = pickupable.gameObject;
            if(Main.Config.ScanOnPickup && Inventory.main.container.Contains(TechType.Scanner) && entryData != null)
            {
                if(!PDAScanner.GetPartialEntryByKey(techType, out var entry))
                {
                    entry = PDAScanner.Add(techType, 1);
                }
                if(entry != null)
                {
                    PDAScanner.partial.Remove(entry);
                    PDAScanner.complete.Add(entry.techType);
                    PDAScanner.NotifyRemove(entry);
                    PDAScanner.Unlock(entryData, true, true);
                    KnownTech.Add(techType, false);
                    if(gameObject != null)
                    {
                        gameObject.SendMessage("OnScanned", null, SendMessageOptions.DontRequireReceiver);
                    }
#if SN1
                    ResourceTracker.UpdateFragments();
#endif
                }
            }

            if(!Main.Config.Hardcore && entryData == null)
            {
                KnownTech.Add(techType);
            }
        }

        private static IEnumerator GiveHardcoreScanner()
        {
            var task1 = CraftData.GetPrefabForTechTypeAsync(TechType.Scanner);
            yield return task1;
            var scannerPrefab = task1.GetResult();
            var gameObject1 = Object.Instantiate(scannerPrefab);
            var pickupable1 = gameObject1.GetComponent<Pickupable>();
            scannerPrefab.SetActive(false);

            var task2 = CraftData.GetPrefabForTechTypeAsync(TechType.Battery);
            yield return task2;
            var batteryPrefab = task2.GetResult();
            var gameObject2 = Object.Instantiate(batteryPrefab);
            var pickupable2 = gameObject2.GetComponent<Pickupable>();
            batteryPrefab.SetActive(false);

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
            var scannerTool = pickupable1.GetComponent<ScannerTool>();
            scannerTool?.energyMixin?.batterySlot?.AddItem(pickupable2);

            Inventory.main.container.AddItem(pickupable1);
        }
    }

}