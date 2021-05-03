namespace PersistentCommands.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    public static class Player_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if(Main.Config.NoAggression)
                GameModeUtils.ActivateCheat(GameModeOption.NoAggression);

            if(Main.Config.NoBlueprints)
                GameModeUtils.ActivateCheat(GameModeOption.NoBlueprints);

            if(Main.Config.NoCost)
                GameModeUtils.ActivateCheat(GameModeOption.NoCost);

            if(Main.Config.NoEnergy)
                GameModeUtils.ActivateCheat(GameModeOption.NoEnergy);

            if(Main.Config.NoOxygen)
                GameModeUtils.ActivateCheat(GameModeOption.NoOxygen);

            if(Main.Config.NoPressure)
                GameModeUtils.ActivateCheat(GameModeOption.NoPressure);

            if(Main.Config.NoRadiation)
                GameModeUtils.ActivateCheat(GameModeOption.NoRadiation);
#if BZ
            if (Main.Config.NoCold)
                GameModeUtils.ActivateCheat(GameModeOption.NoCold);
#endif

        }
    }
}
