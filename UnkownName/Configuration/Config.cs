namespace UnKnownName.Configuration
{
    using SMLHelper.V2.Json;
    using SMLHelper.V2.Options.Attributes;

    [Menu("Unkown Name Config", LoadOn = MenuAttribute.LoadEvents.MenuRegistered | MenuAttribute.LoadEvents.MenuOpened)]
    public class Config: ConfigFile
    {
        public string UnKnownLabel = "";
        public string UnKnownTitle = "Unanalyzed Item";
        public string UnKnownDescription = "This item has not been analyzed yet. \nTo discover its usefulness please scan it at your earliest convenience.";

        [Toggle("Scan On Pickup", Tooltip = "Automatically Scans object on Pickup when you have a charged scanner in your inventory")]
        public bool ScanOnPickup = false;

        [Toggle("Hardcore")]
        public bool Hardcore = false;
    }

}