using HarmonyLib;

namespace UnKnownName
{
    [HarmonyPatch(typeof(Builder), nameof(Builder.UpdateAllowed))]
    public class Builder_UpdateAllowed
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            if (Main.config.Hardcore && __result && Builder.prefab != null)
            {
                TechType techType = CraftData.GetTechType(Builder.prefab);
                __result = CrafterLogic.IsCraftRecipeUnlocked(techType);
            }
        }
    }

}