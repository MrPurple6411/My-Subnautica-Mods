using HarmonyLib;

namespace NoCrosshair.Patches
{
    [HarmonyPatch(typeof(uGUI_MapRoomScanner), nameof(uGUI_MapRoomScanner.OnTriggerEnter))]
    public static class MapRoomScanner_OnTriggerEnter_Patch
    {
        public static void Postfix()
        {
            if (NoCrosshair.check)
            {
                NoCrosshair.mapCheck = NoCrosshair.check;
                NoCrosshair.check = false;
            }
        }
    }
}