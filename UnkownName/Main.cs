using System;
using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using UnityEngine;
using UnKnownName.Configuration;
#if SUBNAUTICA
using Data = SMLHelper.V2.Crafting.TechData;
#elif BELOWZERO
using Data = SMLHelper.V2.Crafting.RecipeData;
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

        internal static Data GetData(TechType techType)
        {
#if SUBNAUTICA
            return CraftDataHandler.GetTechData(techType);
#elif BELOWZERO
            return CraftDataHandler.GetRecipeData(techType);
#endif
        }
    }
}