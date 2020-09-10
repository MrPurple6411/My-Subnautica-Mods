using HarmonyLib;
using QModManager.API.ModLoading;
using TechPistol.Configuration;
using SMLHelper.V2.Handlers;
using System.Reflection;
using UnityEngine;
using System;
using System.IO;
using TechPistol.Module;

namespace TechPistol
{
    [QModCore]
    public static class Main
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
		private static string modPath = Path.GetDirectoryName(assembly.Location);
		internal static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(modPath, "Assets/Assets"));
		internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
		internal static PistolPrefab pistol { get; } = new PistolPrefab();

		[QModPatch]
        public static void Load()
        {
			pistol.Patch();
		}
	}
}