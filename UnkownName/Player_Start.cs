using HarmonyLib;

namespace UnKnownName
{
    [HarmonyPatch(typeof(Player), nameof(Player.Start))]
    public class Player_Start
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            Main.config.Load();
        }
    }

}