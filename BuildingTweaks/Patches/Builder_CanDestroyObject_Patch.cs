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
                }
                else if (go.GetComponentInParent<BaseGhost>() != null)
                {
                    __result = false;
                }
            }
        }
    }
}
