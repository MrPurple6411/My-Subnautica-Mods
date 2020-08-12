using SMLHelper.V2.Json;

namespace UnKnownName.Configuration
{
    public class Config : ConfigFile
    {
        public string UnKnownLabel = "";
        public string UnKnownTitle = "Unanalyzed Item";
        public string UnKnownDescription = "This item has not been analyzed yet. \nTo discover its usefulness please scan it at your earliest convenience.";
        public bool ScanOnPickup = false;
        public bool Hardcore = false;
    }

}