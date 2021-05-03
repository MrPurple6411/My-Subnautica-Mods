namespace Base_Deconstruct_Fix.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(BaseDeconstructable), nameof(BaseDeconstructable.Deconstruct))]
    public static class BaseDeconstructable_Patches
    {
        [HarmonyPrefix]
        public static void Postfix(ref BaseDeconstructable __instance)
        {
            if(__instance.recipe == TechType.BaseCorridor || __instance.recipe == TechType.BaseCorridorGlass)
            {
                switch(__instance.name)
                {
                    case "BaseCorridorIShape(Clone)":
                        __instance.recipe = TechType.BaseCorridorI;
                        break;
                    case "BaseCorridorIShapeGlass(Clone)":
                        __instance.recipe = TechType.BaseCorridorGlassI;
                        break;
                    case "BaseCorridorLShape(Clone)":
                        __instance.recipe = TechType.BaseCorridorL;
                        break;
                    case "BaseCorridorLShapeGlass(Clone)":
                        __instance.recipe = TechType.BaseCorridorGlassL;
                        break;
                    case "BaseCorridorTShape(Clone)":
                        __instance.recipe = TechType.BaseCorridorT;
                        break;
                    case "BaseCorridorXShape(Clone)":
                        __instance.recipe = TechType.BaseCorridorX;
                        break;
                }
            }
        }
    }
}
