namespace PowerOrder.Configuration
{
    using QModManager.Utility;
    using SMLHelper.V2.Options;
    using System;
    using System.Linq;

    internal class Options: ModOptions
    {
        private readonly Config config = Main.config;

        public Options() : base("PowerOrder")
        {
            try
            {
                config.Load();
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
            if(!e.Id.Contains("PowerOrder_"))
            {
                return;
            }

            var key = int.Parse(e.Id.Replace("PowerOrder_", ""));

            config.Order.TryGetValue(key, out var oldValue);

            var otherKey = config.Order.First((x) => x.Value == e.Value).Key;
            config.Order[otherKey] = oldValue;
            config.Order[key] = e.Value;
            config.Save();

            try
            {
                var currentTab = Main.optionsPanel.currentTab;
                Main.optionsPanel.RemoveTabs();
                Main.optionsPanel.AddTabs();
                Main.optionsPanel.SetVisibleTab(currentTab);
            }
            catch(Exception er)
            {
                Logger.Log(Logger.Level.Error, ex: er, showOnScreen: true);
            }
        }

        public override void BuildModOptions()
        {
            var choices = config.Order.Values.ToArray();
            foreach(var key in config.Order.Keys)
            {
                AddChoiceOption($"PowerOrder_{key}", key.ToString(), choices, key - 1);
            }
        }
    }
}
