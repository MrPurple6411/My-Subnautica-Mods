using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace BuildingTweaks.Patches
{
    [HarmonyPatch(typeof(Builder), nameof(Builder.UpdateAllowed))]
    internal class Builder_UpdateAllowed_Patch
    {


        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            if (Main.config.AttachToTarget)
            {
#if SN1
                __result = Builder.CheckAsSubModule();
#elif BZ
                __result = Builder.CheckAsSubModule(out _);
#endif
            }

            if (Main.config.FullOverride)
            {
                __result = true;
            }
        }

        [HarmonyPrefix]
        public static void Prefix(ref bool ___allowedOnConstructables, ref bool ___allowedInBase, ref bool ___allowedInSub, ref bool ___allowedOutside, ref List<SurfaceType> ___allowedSurfaceTypes)
        {
            ___allowedOnConstructables = true;
            ___allowedInBase = true;
            ___allowedInSub = true;
            ___allowedOutside = true;
            if (___allowedSurfaceTypes.Contains(SurfaceType.Wall) && !___allowedSurfaceTypes.Contains(SurfaceType.Ceiling))
            {
                ___allowedSurfaceTypes.Add(SurfaceType.Ceiling);
            }
        }
    }
}
