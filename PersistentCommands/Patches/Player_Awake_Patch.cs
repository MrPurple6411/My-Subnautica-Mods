namespace PersistentCommands.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    public static class Player_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if(Main.SmcConfig.NoAggression)
                GameModeUtils.ActivateCheat(GameModeOption.NoAggression);

            if(Main.SmcConfig.NoBlueprints)
                GameModeUtils.ActivateCheat(GameModeOption.NoBlueprints);

            if(Main.SmcConfig.NoCost)
                GameModeUtils.ActivateCheat(GameModeOption.NoCost);

            if(Main.SmcConfig.NoEnergy)
                GameModeUtils.ActivateCheat(GameModeOption.NoEnergy);

            if(Main.SmcConfig.NoOxygen)
                GameModeUtils.ActivateCheat(GameModeOption.NoOxygen);

            if(Main.SmcConfig.NoPressure)
                GameModeUtils.ActivateCheat(GameModeOption.NoPressure);

            if(Main.SmcConfig.NoRadiation)
                GameModeUtils.ActivateCheat(GameModeOption.NoRadiation);
#if BZ
            if (Main.Config.NoCold)
                GameModeUtils.ActivateCheat(GameModeOption.NoCold);
#endif

        }
    }
}
