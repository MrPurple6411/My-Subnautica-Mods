using HarmonyLib;
using System.Collections.Generic;

namespace MoreSeamothDepth.Patches
{
    [HarmonyPatch(typeof(SeaMoth), "OnUpgradeModuleChange")]
    public class Seamoth_OnUpgradeModuleChange_Patch
    { 
        static void Postfix(SeaMoth __instance, TechType techType)
        {
            // Dictionary of TechTypes and their depth additions.
            var dictionary = new Dictionary<TechType, float>
            {
                {
                    TechType.SeamothReinforcementModule,
                    800f
                },
                {
                    TechType.VehicleHullModule1,
                    100f
                },
                {
                    TechType.VehicleHullModule2,
                    300f
                },
                {
                    TechType.VehicleHullModule3,
                    700f
                },
                {
                    Main.moduleMK4.TechType,
                    1100f
                },
                {
                    Main.moduleMK5.TechType,
                    1500f
                }
            };

            // Depth upgrade to add.
            var depthUpgrade = 0f;

            // Loop through available depth module upgrades
            foreach (var entry in dictionary)
            {
                TechType depthTechType = entry.Key;
                float depthAddition = entry.Value;

                int count = __instance.modules.GetCount(depthTechType);

                // If you have at least 1 such depth module
                if(count > 0)
                {
                    if(depthAddition > depthUpgrade)
                    {
                        depthUpgrade = depthAddition;
                    }
                }
            }

            // Add the upgrade.
            __instance.crushDamage.SetExtraCrushDepth(depthUpgrade);
        }
    }
}
