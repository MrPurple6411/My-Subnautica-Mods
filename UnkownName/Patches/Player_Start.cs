namespace UnKnownName.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(Player), nameof(Player.Start))]
    public class Player_Start
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            Main.Config.Load();
        }
    }

}