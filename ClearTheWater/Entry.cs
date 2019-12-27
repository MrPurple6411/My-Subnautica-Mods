using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ClearTheWater
{
    public class Entry
    {
        public static void Patch()
        {
            HarmonyInstance.Create("MrPurple6411.ClearTheWater").PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(WaterscapeVolume.Settings))]
    [HarmonyPatch(nameof(WaterscapeVolume.Settings.GetExtinctionAndScatteringCoefficients))]
    internal class ClearTheWater_Patch
    {
        [HarmonyPrefix]
        public static bool prefix(WaterscapeVolume.Settings __instance, ref Vector4 __result)
        {
            Vector3 vector = __instance.absorption + __instance.scattering * Vector3.one;
            __result = new Vector4(vector.x, vector.y, vector.z, __instance.scattering) * (__instance.murkiness / 400f);
            return false;
        }
    }

    [HarmonyPatch(typeof(WaterscapeVolume))]
    [HarmonyPatch(nameof(WaterscapeVolume.Awake))]
    internal class WaterscapeVolume_Patch
    {
        [HarmonyPostfix]
        public static void postfix(WaterscapeVolume __instance)
        {
            __instance.aboveWaterDensityScale = 1f;
        }
    }
}
