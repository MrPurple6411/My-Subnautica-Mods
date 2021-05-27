namespace BuildingTweaks.Patches
{
    // ReSharper disable RedundantAssignment
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Linq;

#if BZ
    using UnityEngine;
#endif

    [HarmonyPatch(typeof(Builder), nameof(Builder.UpdateAllowed))]
    internal class Builder_UpdateAllowed_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            var blacklist = new List<TechType>() {
#if BZ
                TechType.BaseGlassDome, TechType.BaseLargeGlassDome,
                TechType.BasePartition, TechType.BasePartitionDoor,
#endif 
                TechType.BaseHatch, TechType.BaseWindow, TechType.BaseReinforcement,
                TechType.BaseLadder, TechType.BaseFiltrationMachine, TechType.BaseBulkhead,
                TechType.BaseBioReactor, TechType.BaseNuclearReactor, TechType.BaseWaterPark,
                TechType.BaseUpgradeConsole
                };

            if(blacklist.Contains(Builder.constructableTechType))
                return;

            if (blacklist.Any(techType => Builder.constructableTechType.AsString().EndsWith(techType.AsString())))
                return;

            if(Builder_Update_Patches.UpdatePlacement && (Main.Config.AttachToTarget || (Builder.placementTarget != null && Builder.prefab != null && Builder.prefab.GetComponentInChildren<ConstructableBase>() is null)))
            {
#if SN1
                __result = Builder.CheckAsSubModule();
#elif BZ
                __result = Builder.CheckAsSubModule(out _);
#endif
            }

            if(Main.Config.FullOverride)
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
            if(___allowedSurfaceTypes.Contains(SurfaceType.Wall) && !___allowedSurfaceTypes.Contains(SurfaceType.Ceiling))
            {
                ___allowedSurfaceTypes.Add(SurfaceType.Ceiling);
            }
        }
    }

#if BZ
    [HarmonyPatch(typeof(Builder), nameof(Builder.CheckTag))]
    internal class Builder_CheckAsSubModule_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Collider c, ref bool __result)
        {
            if(__result || c?.gameObject is null)
                return;

            var s = c.gameObject.GetComponentInParent<SeaTruckSegment>();
            if(s != null)
                __result = true;
        }
    }
#endif
}
