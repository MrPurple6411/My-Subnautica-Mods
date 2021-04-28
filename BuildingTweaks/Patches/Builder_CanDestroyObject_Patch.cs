using HarmonyLib;
using UnityEngine;

namespace BuildingTweaks.Patches
{
    [HarmonyPatch(typeof(Builder),nameof(Builder.CanDestroyObject))]
    public static class Builder_CanDestroyObject_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(GameObject go, ref bool __result)
        {
            if (__result)
            {
                if (go.GetComponentInParent<Creature>() != null)
                {
                    __result = false;
                    return;
                }
                
                if (go.GetComponentInParent<BaseGhost>() != null)
                {
                    __result = false;
                    return;
                }

                GameObject target = UWE.Utils.GetEntityRoot(go) ?? go;

                if(target.name.ToLower().Contains("override") || target.name.ToLower().Contains("aurora") || target.name.ToLower().Contains("crashedship") || target.name.ToLower().Contains("starship"))
                {
                    __result = false;
                    return;
                }


            }
        }
    }
}
