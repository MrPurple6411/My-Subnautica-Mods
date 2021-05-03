namespace ConfigurableChunkDrops.Patches
{
    using HarmonyLib;
    using System.IO;
    using UWE;

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    public static class Defaults_Patch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            if(!File.Exists(Path.Combine(Main.modPath, "DefaultValues.json")))
                CoroutineHost.StartCoroutine(Main.GenerateDefaults());
        }
    }
}