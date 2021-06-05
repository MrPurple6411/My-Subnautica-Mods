namespace NoCrosshair
{
    using HarmonyLib;
    using QModManager.API.ModLoading;

    [QModCore]
    public static class Main
    {
        [QModPatch]
        public static void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Patches.Patches), $"MrPurple6411_NoCrosshair");
        }
    }
}