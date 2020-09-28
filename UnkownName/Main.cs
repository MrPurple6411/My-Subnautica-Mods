using System;
using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using UnityEngine;
using UnKnownName.Configuration;
#if SN1
using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif

namespace UnKnownName
{

    [QModCore]
    public static class Main
    {
        internal static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        [QModPatch]
        public static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }

        internal static RecipeData GetData(TechType techType)
        {
#if SN1
            return CraftDataHandler.GetTechData(techType);
#elif BZ
            return CraftDataHandler.GetRecipeData(techType);
#endif
        }
    }
}