#if SN1
namespace BuilderPlaceOnComplete.Patches
{
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(BuilderTool), nameof(BuilderTool.HandleInput))]
    public class BuilderTool_HandleInput_Patch
    {
        [HarmonyPrefix]
        public static void Postfix()
        {
            if(!uGUI_BuilderMenu.IsOpen() && Input.GetMouseButton(2) && !Builder.isPlacing)
            {
                if(Targeting.GetTarget(Player.main.gameObject, 200f, out GameObject result, out _))
                {
                    if(Targeting.GetRoot(result, out TechType techType, out GameObject gameObject) && CraftData.IsBuildableTech(techType))
                    {
                        if(Builder.Begin(gameObject))
                            ErrorMessage.AddMessage($"Placing new {techType}");
                        else
                            Builder.End();
                    }
                }
            }
        }
    }
}
#endif