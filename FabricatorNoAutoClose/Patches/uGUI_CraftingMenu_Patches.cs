namespace FabricatorNoAutoClose.Patches;

using HarmonyLib;

[HarmonyPatch]
public static class uGUI_CraftingMenu_Patches
{
    private static bool _isOpen = false;

    [HarmonyPatch(typeof(uGUI_CraftingMenu), nameof(uGUI_CraftingMenu.Open)), HarmonyPostfix]
    public static void Open_Postfix(uGUI_CraftingMenu __instance)
    {
        _isOpen = true;
    }

    [HarmonyPatch(typeof(uGUI_CraftingMenu), nameof(uGUI_CraftingMenu.Close)), HarmonyPostfix]
    public static void Close_Postfix(uGUI_CraftingMenu __instance)
    {
        _isOpen = false;
    }


    [HarmonyPatch(typeof(Player), nameof(Player.AddUsedTool)), HarmonyPrefix]
    public static bool AddUsedTool_Prefix(ref bool __result)
    {
        if (_isOpen)
        {
            __result = false;
            return false;
        }
        return true;
    }


    [HarmonyPatch(typeof(Player), nameof(Player.IsToolUsed)), HarmonyPrefix]
    public static bool IsToolUsed_Prefix(ref bool __result)
    {
        if (_isOpen)
        {
            __result = true;
            return false;
        }
        return true;
    }
}