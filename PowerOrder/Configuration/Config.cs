namespace PowerOrder.Configuration
{
    using SMLHelper.V2.Json;
    using System.Collections.Generic;

    public class Config: ConfigFile
    {
        internal static Dictionary<int, string> DefaultOrder = new Dictionary<int, string>() { { 1, "PowerTransmitter" }, { 2, "SolarPanel" }, { 3, "ThermalPlant" }, { 4, "BaseBioReactor" }, { 5, "BaseNuclearReactor" } };
        internal bool doSort = false;

        public Dictionary<int, string> Order = DefaultOrder;
    }
}
