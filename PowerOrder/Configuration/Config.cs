namespace PowerOrder.Configuration;

using Nautilus.Json;
using System.Collections.Generic;

public class SMLConfig: ConfigFile
{
    internal static Dictionary<int, string> DefaultOrder = new()
    {
        { 1, "Power transmitter" }, 
        { 2, "Habitat Control Panel" },
        { 3, "Alien Containment Unit" },  
        { 4, "Alterra Solar Cluster" }, 
        { 5, "JetStreamT242" }, 
        { 6, "Wind Turbine" }, 
        { 7, "Deep Engine MK1" }, 
        { 8, "Solar panel" }, 
        { 9, "Thermal plant" }, 
        { 10, "RegenPowerCell" }, 
        { 11, "Bio-Reactors" }, 
        { 12, "Nuclear Reactors" }, 
        { 13, "Power cell" }
    };
    internal bool doSort = false;

    public Dictionary<int, string> Order = DefaultOrder;
}
