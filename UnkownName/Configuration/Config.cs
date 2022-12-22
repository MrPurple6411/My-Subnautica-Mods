namespace UnknownName.Configuration
{
    using SMLHelper.V2.Json;
    using SMLHelper.V2.Options.Attributes;

    [Menu("Unknown Name Config", LoadOn = MenuAttribute.LoadEvents.MenuRegistered | MenuAttribute.LoadEvents.MenuOpened)]
    public class SMLConfig: ConfigFile
    {
        public string UnKnownLabel = "";
        public string UnKnownTitle = "Unknown Item";
        public string UnKnownDescription = "This item has not been analyzed yet. \nTo discover its usefulness please scan it at your earliest convenience.";

        [Toggle("Scan On Pickup", Tooltip = "Automatically Scans object on Pickup when you have a charged scanner in your inventory")]
        public bool ScanOnPickup = false;

        [Toggle("Hardcore")]
        public bool Hardcore = false;
    }

}