using System;
using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using UnityEngine;

namespace WarpersNoWarping
{
    [QModCore]
    public static class Main
    {
        [QModPatch]
        public static void Load()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}