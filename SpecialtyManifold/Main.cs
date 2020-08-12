using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using NitrogenMod.Items;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using SpecialtyManifold.Configuration;
using UnityEngine;

namespace SpecialtyManifold
{

    [QModCore]
    public class Main
    {
        internal static Config config = new Config();

        [QModPatch]
        public static void Load()
        {
            try
            {
                config.Load();
                OptionsPanelHandler.RegisterModOptions(new Options());

                var assembly = Assembly.GetExecutingAssembly();
                new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);

                KnownTechHandler.SetAnalysisTechEntry(TechType.Workbench, new List<TechType>() { O2TanksCore.PhotosynthesisSmallID, O2TanksCore.PhotosynthesisTankID, O2TanksCore.ChemosynthesisTankID });
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}