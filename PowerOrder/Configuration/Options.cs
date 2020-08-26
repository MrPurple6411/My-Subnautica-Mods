using HarmonyLib;
using Oculus.Newtonsoft.Json;
using QModManager.Utility;
using SMLHelper.V2.Options;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerOrder.Configuration
{
    internal class Options : ModOptions
    {
        private readonly Config config = Main.config;

        public Options() : base("PowerOrder")
        {
            try
            {
                //config.Load();
            }
            catch(Exception e)
            {
                Logger.Log(Logger.Level.Error, $"Failed to load Config file. Generating fresh file.", e);
            }
            
            config.Order = config.Order.OrderBy(p => p.Key).ThenBy(p => p.Value).ToDictionary(t => t.Key, t => t.Value);
            config.Save();

            ChoiceChanged += Options_ChoiceChanged;
        }


        private void Options_ChoiceChanged(object sender, ChoiceChangedEventArgs e)
        {
            if (!e.Id.Contains("PowerOrder_"))
            {
                return;
            }

            int key = int.Parse(e.Id.Replace("PowerOrder_", ""));

            config.Order.TryGetValue(key, out string oldValue);

            int otherKey = config.Order.First((x) => x.Value == e.Value).Key;
            config.Order[otherKey] = oldValue;
            config.Order[key] = e.Value;
            config.Save();

            try
            {
                int currentTab = Traverse.Create(Main.optionsPanel).Field("currentTab").GetValue<int>();
                Main.optionsPanel.RemoveTabs();
                AccessTools.Method(typeof(uGUI_OptionsPanel), "AddTabs").Invoke(Main.optionsPanel, null);
                AccessTools.Method(typeof(uGUI_TabbedControlsPanel), "SetVisibleTab").Invoke(Main.optionsPanel, new object[] { currentTab });
            }
            catch(Exception er)
            {
                Logger.Log(Logger.Level.Error, ex: er, showOnScreen: true);
            }
        }

        public override void BuildModOptions()
        {
            string[] choices = config.Order.Values.ToArray();
            foreach(int key in config.Order.Keys)
            {
                AddChoiceOption($"PowerOrder_{key}", key.ToString(), choices, key-1);
            }
        }
    }
}
