﻿namespace ConfigurableChunkDrops.Configuration;

using Nautilus.Json;
using System.Collections.Generic;

public class SMLConfig: ConfigFile
{
    // Add a parameterless constructor for Nautilus
    public SMLConfig() : base("config", null)
    {
    }
    
    public SMLConfig(string fileName = "config", string subfolder = null) : base(fileName, subfolder)
    {
    }

    public Dictionary<string, Dictionary<string, float>> Breakables = new();
}
