using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnKnownName;

namespace UnknownName.Patches
{
    [HarmonyPatch(typeof(KnownTech), nameof(KnownTech.GetTechUnlockState), new Type[] { typeof(TechType), typeof(int), typeof(int) }, new ArgumentType[]{ ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out })]
    public static class KnownTech_GetTechUnlockState
    {
        [HarmonyPostfix]
        public static void Postfix(TechType techType, ref TechUnlockState __result)
        {
            if(Main.config.Hardcore && (__result != TechUnlockState.Available || !CrafterLogic.IsCraftRecipeUnlocked(techType)))
            {
                __result = TechUnlockState.Hidden;
            }
        }
    }
}
