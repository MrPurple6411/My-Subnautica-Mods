namespace PowerOrder.Patches
{
    using HarmonyLib;
    using QModManager.Utility;
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
                if(__instance is null || (__instance.inboundPowerSources?.Contains(powerInterface) ?? true))
                    return;
                Logger.Log(Logger.Level.Debug, $"{Regex.Replace(__instance.gameObject.name, @"\(.*?\)", "")} AddInboundPower: {Regex.Replace(powerInterface.GetType().Name, @"\(.*?\)", "")}");
                Main.config.doSort = true;
            }
            catch(Exception e)
            {
                Logger.Log(Logger.Level.Error, ex: e);
            }
        }

        [HarmonyPostfix]
        private static void Postfix(PowerRelay __instance)
        {
            try
            {
                if(!Main.config.doSort)
                    return;
                List<IPowerInterface> info = __instance.inboundPowerSources;
                List<IPowerInterface> test = new List<IPowerInterface>(info);
                test.Sort((i, i2) =>
                {
                    UnityEngine.MonoBehaviour p = (UnityEngine.MonoBehaviour)i;
                    UnityEngine.MonoBehaviour p2 = (UnityEngine.MonoBehaviour)i2;
                    int pn = GetOrderNumber(p.gameObject.name);
                    int p2n = GetOrderNumber(p2.gameObject.name);
                    return Math.Sign(pn - p2n);
                });
                __instance.inboundPowerSources = test;
                Main.config.doSort = false;
            }
            catch(Exception e)
            {
                Logger.Log(Logger.Level.Error, ex: e);
            }
        }
        private static int GetOrderNumber(string name)
        {
            for(int x = 0; x < Main.config.Order.Count; x++)
            {
                KeyValuePair<int, string> kvp = Main.config.Order.ElementAt(x);
                if(name.ToLower().Contains(kvp.Value.ToLower()))
                {
                    int order = kvp.Key;
                    if(order > Main.config.Order.Count || order < 1)
                    {
                        Logger.Log(Logger.Level.Error, kvp.Key + " has an invalid order number.  Please fix this and try again.  (Must be within 1-" + Main.config.Order.Count + ")", showOnScreen: true);
                        throw new Exception();
                    }
                    return order;
                }
            }
            name = Regex.Replace(name, @"\(.*?\)", "");
            Logger.Log(Logger.Level.Info, "New power source found: " + name);
            Main.config.Order.Add(Main.config.Order.Count + 1, name);
            Main.config.Save();
            return Main.config.Order.Count + 1;
        }
    }
}
