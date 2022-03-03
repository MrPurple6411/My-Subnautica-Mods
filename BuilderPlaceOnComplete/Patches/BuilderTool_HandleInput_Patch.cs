namespace BuilderPlaceOnComplete.Patches
{
    using HarmonyLib;
    using UnityEngine;
#if !SUBNAUTICA_STABLE
    using UWE;
#endif
    [HarmonyPatch(typeof(BuilderTool), nameof(BuilderTool.HandleInput))]
    public class BuilderTool_HandleInput_Patch
    {
        [HarmonyPrefix]
        public static void Postfix()
        {
            if (uGUI_BuilderMenu.IsOpen() || !Input.GetMouseButton(2) || Builder.isPlacing) return;
            if (!Targeting.GetTarget(Player.main.gameObject, 200f, out var result, out _)) return;
            if (!Targeting.GetRoot(result, out var techType, out var gameObject)
#if SN1
                || !CraftData.IsBuildableTech(techType)
#elif BZ
               || !TechData.GetBuildable(techType)
#endif
            ) return;
#if SUBNAUTICA_STABLE
            if(Builder.Begin(gameObject))
                ErrorMessage.AddMessage($"Placing new {techType}");
            else
                Builder.End();
#else
                CoroutineHost.StartCoroutine(Builder.BeginAsync(techType));
                ErrorMessage.AddMessage($"Placing new {techType}");
#endif
        }
    }
}