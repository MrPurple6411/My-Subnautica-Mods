namespace PowerOrder.Configuration
{
    using BepInEx.Logging;
    using SMLHelper.Options;
    using System;
    using System.Linq;

    internal class Options: ModOptions
    {
        private readonly SMLConfig SMLConfig = Main.SMLConfig;
        internal static uGUI_OptionsPanel optionsPanel;

        public Options() : base("PowerOrder")
        {
            try
            {
                SMLConfig.Load();
                var choices = SMLConfig.Order.Values.ToArray();
                OnChanged += Options_ChoiceChanged;
                foreach(var key in SMLConfig.Order.Keys)
                {
                    AddItem(ModChoiceOption<string>.Create($"PowerOrder_{key}", key.ToString(), choices, key - 1));
                }
            }
            catch(Exception e)
            {
                Main.logSource.Log(LogLevel.Error, $"Failed to load Config file. Generating fresh file.\n"+ e);
            }

            SMLConfig.Order = SMLConfig.Order.OrderBy(p => p.Key).ThenBy(p => p.Value).ToDictionary(t => t.Key, t => t.Value);
            SMLConfig.Save();

        }


        private void Options_ChoiceChanged(object sender, OptionEventArgs e)
        {
            if(!e.Id.Contains("PowerOrder_") || e is not ChoiceChangedEventArgs<string> ea)
            {
                return;
            }

            var key = int.Parse(e.Id.Replace("PowerOrder_", ""));

            SMLConfig.Order.TryGetValue(key, out var oldValue);

            var otherKey = SMLConfig.Order.First((x) => x.Value == ea.Value).Key;
            SMLConfig.Order[otherKey] = oldValue;
            SMLConfig.Order[key] = ea.Value;
            SMLConfig.Save();

            try
            {
                var currentTab = optionsPanel.currentTab;
                optionsPanel.RemoveTabs();
                optionsPanel.AddTabs();
                optionsPanel.SetVisibleTab(currentTab);
            }
            catch(Exception er)
            {
                Main.logSource.Log(LogLevel.Error, er);
            }
        }

    }
}
