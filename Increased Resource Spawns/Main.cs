using System;
using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using UnityEngine;
using UWE;

namespace Increased_Resource_Spawns
{
    [QModCore]
    public class Main
    {
        [QModPatch]
        public static void Load()
        {
            Config.Load();
            Assembly assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
    public static class Config
    {
        public static int ResourceMultiplier;

        public static void Load()
        {
            ResourceMultiplier = PlayerPrefs.GetInt("ResourceMultiplier", 1);
            OptionsPanelHandler.RegisterModOptions(new Options());
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("Increased Resource Spawn Settings")
        {
            SliderChanged += ResourceMultiplier_SliderChanged;
        }

        public override void BuildModOptions()
        {
            AddSliderOption("ResourceMultiplier", "Resource Multiplier", 1, 100, Config.ResourceMultiplier);
        }

        public void ResourceMultiplier_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if (e.Id != "ResourceMultiplier")
            {
                return;
            }

            Config.ResourceMultiplier = (int)e.Value;
            PlayerPrefs.SetInt("ResourceMultiplier", (int)e.Value);
        }
    }

    [HarmonyPatch(typeof(CellManager))]
    [HarmonyPatch(nameof(CellManager.GetPrefabForSlot), new Type[] { typeof(IEntitySlot) })]
    internal class IncreaseResourceSpawn
    {
        public static void Postfix(CellManager __instance, IEntitySlot slot, ref EntitySlot.Filler __result)
        {
            int num = 0;
            if (__instance.spawner != null && !slot.IsCreatureSlot() && Config.ResourceMultiplier > 1)
            {
                while (string.IsNullOrEmpty(__result.classId) && (float)num < Config.ResourceMultiplier)
                {
                    num++;
                    __result = __instance.spawner.GetPrefabForSlot(slot, true);
                }
            }
        }
    }
}