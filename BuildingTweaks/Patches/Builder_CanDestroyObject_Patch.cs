namespace BuildingTweaks.Patches
{
    using HarmonyLib;
    using System.Collections.Generic;
    using UnityEngine;

    [HarmonyPatch(typeof(Builder), nameof(Builder.CanDestroyObject))]
    public static class Builder_CanDestroyObject_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(GameObject go, ref bool __result)
        {
            if(__result)
            {
                if(go.GetComponentInParent<Creature>() != null)
                {
                    __result = false;
                    return;
                }

                if(go.GetComponentInParent<BaseGhost>() != null)
                {
                    __result = false;
                    return;
                }

                if(go.GetComponentInParent<Constructable>() != null)
                {
                    __result = false;
                    return;
                }
#if SN1
                if(go.GetComponentInParent<EscapePod>() != null)
                {
                    __result = false;
                    return;
                }

#elif BZ
                if(go.GetComponentInParent<LifepodDrop>() != null)
                {
                    __result = false;
                    return;
                }
#endif

                var target = UWE.Utils.GetEntityRoot(go) ?? go;

                var blacklist = new List<string>() { "override", "aurora", "crashedship", "starship" };

                foreach(var name in blacklist)
                {
                    if(target.name.ToLower().Contains(name))
                    {
                        __result = false;
                        return;
                    }
                }
            }
        }
    }
}
