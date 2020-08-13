using HarmonyLib;
using Oculus.Newtonsoft.Json;
using QModManager.API.ModLoading;
using QuantumPowerTransmitters.Prefabs;
using SMLHelper.V2.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace QuantumPowerTransmitters
{
    [QModCore]
    public static class Main
    {
        internal static TransmitterDishPrefab transmitterDish = new TransmitterDishPrefab();
        internal static List<BasePowerRelay> baseRelays = new List<BasePowerRelay>();

        [QModPatch]
        public static void Load()
        {
            //Assembly assembly = Assembly.GetExecutingAssembly();
            //new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);

            transmitterDish.Patch();
        }
    }
}