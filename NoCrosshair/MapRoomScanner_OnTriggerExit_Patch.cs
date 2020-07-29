using HarmonyLib;

namespace NoCrosshair
{
    [HarmonyPatch(typeof(uGUI_MapRoomScanner), "OnTriggerExit")]
    public static class MapRoomScanner_OnTriggerExit_Patch
    {
        public static void Postfix()
        {
            if (NoCrosshair.mapCheck)
            {
                NoCrosshair.check = NoCrosshair.mapCheck;
                NoCrosshair.mapCheck = false;
            }
        }
    }
}