using GravTrapStorage.MonoBehaviours;

namespace GravTrapStorage.Patches;
using HarmonyLib;

[HarmonyPatch]
public class uGUI_ScannerIcon_Patch
{
    [HarmonyPatch(typeof(uGUI_ScannerIcon),nameof(uGUI_ScannerIcon.Awake))]
    [HarmonyPostfix]
    public static void PostFix(uGUI_ScannerIcon __instance)
    {
        __instance.gameObject.EnsureComponent<uGUI_GravtrapIcon>();
    }
    
}