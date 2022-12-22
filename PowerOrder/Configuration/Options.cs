namespace PowerOrder.Configuration
{
    using BepInEx.Logging;
    using SMLHelper.V2.Options;
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
            }
            catch(Exception e)
            {
                Main.logSource.Log(LogLevel.Error, $"Failed to load Config file. Generating fresh file.\n"+ e);
            }

            SMLConfig.Order = SMLConfig.Order.OrderBy(p => p.Key).ThenBy(p => p.Value).ToDictionary(t => t.Key, t => t.Value);
            SMLConfig.Save();

            ChoiceChanged += Options_ChoiceChanged;
        }


        private void Options_ChoiceChanged(object sender, ChoiceChangedEventArgs e)
        {
            if(!e.Id.Contains("PowerOrder_"))
            {
                return;
            }

            var key = int.Parse(e.Id.Replace("PowerOrder_", ""));

            SMLConfig.Order.TryGetValue(key, out var oldValue);

            var otherKey = SMLConfig.Order.First((x) => x.Value == e.Value).Key;
            SMLConfig.Order[otherKey] = oldValue;
            SMLConfig.Order[key] = e.Value;
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

        public override void BuildModOptions()
        {
            var choices = SMLConfig.Order.Values.ToArray();
            foreach(var key in SMLConfig.Order.Keys)
            {
                AddChoiceOption($"PowerOrder_{key}", key.ToString(), choices, key - 1);
            }
        }
    }
}
