using BepInEx;

namespace DropUpgradesOnDestroy
{
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    public class Main:BaseUnityPlugin
    {
        public void  Awake()
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
