namespace UnknownName.Patches;

using System.Collections;
using HarmonyLib;
using UnityEngine;
using UWE;

[HarmonyPatch]
public class uGUI_SceneIntro_Stop_Patch
{

#if SUBNAUTICA
    [HarmonyPatch(typeof(uGUI_SceneIntro), nameof(uGUI_SceneIntro.Stop)), HarmonyPostfix]
    public static void Postfix(bool isInterrupted)
    {
        if (Main.Config.Hardcore)
        {
            CoroutineHost.StartCoroutine(GiveHardcoreScanner(isInterrupted));   
        }
    }

    private static IEnumerator GiveHardcoreScanner(bool isInterrupted)
    {
        if (!isInterrupted)
        {
            while (Inventory.main?.GetHeld()?.GetTechType() != TechType.FireExtinguisher)
            {
                yield return null;
            }
        }
        yield return GiveHardcoreScanner();
    } 
#elif BELOWZERO
    [HarmonyPatch(typeof(MainGameController), nameof(MainGameController.OnIntroDone)), HarmonyPostfix]
    public static void Postfix()
    {
        if (Main.Config.Hardcore)
        {
            CoroutineHost.StartCoroutine(GiveHardcoreScanner());
        }
    }
#endif

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

        TaskResult<Pickupable> task4 = new TaskResult<Pickupable>();
        yield return pickupable2.PickupAsync(task4, false);
        yield return task4;
        pickupable2 = task4.Get();

        TaskResult<Pickupable> task3 = new TaskResult<Pickupable>();
        yield return pickupable1.PickupAsync(task3, false);
        yield return task3;
        pickupable1 = task3.Get();
#else
        pickupable2.Pickup(false);
        pickupable1.Pickup(false);
#endif
        var scannerTool = pickupable1.GetComponent<ScannerTool>();
        scannerTool?.energyMixin?.batterySlot?.AddItem(pickupable2);
        Inventory.main.container.AddItem(pickupable1);
    }
}