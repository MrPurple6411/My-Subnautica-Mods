namespace PersistentCommands.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    public static class Player_Awake_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if(Main.SMLConfig.NoAggression)
                GameModeUtils.ActivateCheat(GameModeOption.NoAggression);

            if(Main.SMLConfig.NoBlueprints)
                GameModeUtils.ActivateCheat(GameModeOption.NoBlueprints);

            if(Main.SMLConfig.NoCost)
                GameModeUtils.ActivateCheat(GameModeOption.NoCost);

            if(Main.SMLConfig.NoEnergy)
                GameModeUtils.ActivateCheat(GameModeOption.NoEnergy);

            if(Main.SMLConfig.NoOxygen)
                GameModeUtils.ActivateCheat(GameModeOption.NoOxygen);

            if(Main.SMLConfig.NoPressure)
                GameModeUtils.ActivateCheat(GameModeOption.NoPressure);

            if(Main.SMLConfig.NoRadiation)
                GameModeUtils.ActivateCheat(GameModeOption.NoRadiation);
#if BZ
            if (Main.Config.NoCold)
                GameModeUtils.ActivateCheat(GameModeOption.NoCold);
#endif

        }
    }
}
