namespace BetterACU.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(CreatureEgg), nameof(CreatureEgg.Hatch))]
    internal class CreatureEgg_Hatch_Prefix
    {
        [HarmonyPostfix]
        public static void Postfix(CreatureEgg __instance)
        {
            UnityEngine.Object.Destroy(__instance.gameObject);
        }
    }
}
