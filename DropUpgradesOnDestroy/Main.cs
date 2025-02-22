namespace DropUpgradesOnDestroy;

using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;using BepInEx;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
#if SUBNAUTICA
[BepInProcess("Subnautica.exe")]
#elif BELOWZERO
[BepInProcess("SubnauticaZero.exe")]
#endif
public class Main: BaseUnityPlugin
{
    private void Awake()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
    }

    internal static void SpawnModuleNearby(List<InventoryItem> equipment, Vector3 position)
    {
        foreach(var item in equipment)
        {
            item.item.Drop(new Vector3(position.x + Random.Range(-3, 3), position.y + Random.Range(5, 8), position.z + Random.Range(-3, 3)));
        }
    }
}
