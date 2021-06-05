namespace UnKnownName.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(Builder), nameof(Builder.UpdateAllowed))]
    public class Builder_UpdateAllowed
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            if(Main.Config.Hardcore && __result && Builder.prefab != null)
            {
                var techType = CraftData.GetTechType(Builder.prefab);
                __result = CrafterLogic.IsCraftRecipeUnlocked(techType);
            }
        }
    }

}