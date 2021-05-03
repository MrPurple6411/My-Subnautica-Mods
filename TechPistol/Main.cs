namespace TechPistol
{
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;
    using System.IO;
    using System.Reflection;
    using TechPistol.Configuration;
    using TechPistol.Module;
    using UnityEngine;

    [QModCore]
    public static class Main
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string modPath = Path.GetDirectoryName(assembly.Location);
        internal static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(modPath, "Assets/TechPistol"));
        internal static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        internal static PistolFragmentPrefab PistolFragment { get; } = new PistolFragmentPrefab();
        internal static PistolPrefab Pistol { get; } = new PistolPrefab();

        [QModPatch]
        public static void Load()
        {
            PistolFragment.Patch();
            Pistol.Patch();

            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}