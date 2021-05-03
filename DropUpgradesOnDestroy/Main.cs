namespace DropUpgradesOnDestroy
{
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    [QModCore]
    public class Main
    {
        [QModPatch]
        public static void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }

        internal static void SpawnModuleNearby(List<InventoryItem> equipment, Vector3 position)
        {
            foreach(InventoryItem item in equipment)
            {
                item.item.Drop(new Vector3(position.x + UnityEngine.Random.Range(-3, 3), position.y + UnityEngine.Random.Range(5, 8), position.z + UnityEngine.Random.Range(-3, 3)));
            }
        }
    }
}
