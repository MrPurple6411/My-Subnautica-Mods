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
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }

        internal static void SpawnModuleNearby(List<InventoryItem> equipment, Vector3 position)
        {
            foreach(var item in equipment)
            {
                item.item.Drop(new Vector3(position.x + Random.Range(-3, 3), position.y + Random.Range(5, 8), position.z + Random.Range(-3, 3)));
            }
        }
    }
}
