using UnityEngine;

namespace PowerOrder.Patches
{
    using BepInEx.Logging;
    using HarmonyLib;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    [HarmonyPatch(typeof(PowerRelay), nameof(PowerRelay.AddInboundPower))]
    internal class PowerRelay_AddInboundPower
    {
        [HarmonyPrefix]
        private static void Prefix(PowerRelay __instance, IPowerInterface powerInterface)
        {
            try
            {
                if (__instance == null || __instance.inboundPowerSources == null ||
                    __instance.inboundPowerSources.Contains(powerInterface))
                    return;
                Main.logSource.Log(LogLevel.Debug,
                    $"{Regex.Replace(__instance.gameObject.name, @"\(.*?\)", "")} AddInboundPower: {Regex.Replace(powerInterface.GetType().Name, @"\(.*?\)", "")}");
                Main.config.doSort = true;
            }
            catch (Exception e)
            {
                Main.logSource.Log(LogLevel.Error, e);
            }
        }

        [HarmonyPostfix]
        private static void Postfix(PowerRelay __instance)
        {
            try
            {
                if (!Main.config.doSort)
                    return;
                var info = __instance.inboundPowerSources;
                var test = new List<IPowerInterface>(info);
                test.Sort((i, i2) =>
                {
                    var p = (MonoBehaviour) i;
                    var p2 = (MonoBehaviour) i2;
                    var pn = GetOrderNumber(p.gameObject);
                    var p2n = GetOrderNumber(p2.gameObject);
                    return Math.Sign(pn - p2n);
                });
                __instance.inboundPowerSources = test;
                Main.config.doSort = false;
            }
            catch (Exception e)
            {
                Main.logSource.Log(LogLevel.Error, e);
            }
        }

        private static int GetOrderNumber(GameObject gameObject)
        {
            TechType techType = CraftData.GetTechType(gameObject);
            if (techType == TechType.None)
                TechTypeExtensions.FromString(gameObject.name, out techType, true);

            var name = techType == TechType.None ? Language.main.GetOrFallback(gameObject.name, gameObject.name) : Language.main.Get(techType);
            if (name.Contains("BioReactor") || name == "Alterra Gen")
            {
                name = "Bio-Reactors";
            }
            else if (name.Contains("Nuclear"))
            {
                name = "Nuclear Reactors";
            }
            else if (name.Contains("WaterPark"))
            {
                name = "Alien Containment Unit";
            }
            else if (name.StartsWith("SubPowerCell"))
            {
                name = Language.main.Get(TechType.PowerCell);
            }

            for (var x = 0; x < Main.config.Order.Count; x++)
            {
                var kvp = Main.config.Order.ElementAt(x);
                if (name.ToLower().Contains(kvp.Value.ToLower()))
                {
                    var order = kvp.Key;
                    if (order > Main.config.Order.Count || order < 1)
                    {
                        Main.logSource.Log(LogLevel.Error,
                            kvp.Key +
                            " has an invalid order number.  Please fix this and try again.  (Must be within 1-" +
                            Main.config.Order.Count + ")");
                        throw new Exception();
                    }

                    return order;
                }
            }

            name = Regex.Replace(name, @"\(.*?\)", "");
            Main.logSource.Log(LogLevel.Info, "New power source found: " + name);
            Main.config.Order.Add(Main.config.Order.Count + 1, name);
            Main.config.Save();
            return Main.config.Order.Count + 1;
        }
    }
}