namespace Time_Eternal.Configuration;

using Nautilus.Json;
using Nautilus.Options.Attributes;
using System.Collections.Generic;

[Menu("Time Eternal", LoadOn = MenuAttribute.LoadEvents.MenuRegistered | MenuAttribute.LoadEvents.MenuOpened)]
public class SMLConfig: ConfigFile
{
    public List<string> validOptions = new() { "0 = Normal", "1 = Noon", "2 = MidNight" };

    [Choice(Label = "Freeze Lighting", Options = new[] { "Normal", "Noon", "MidNight" })]
    public int freezeTimeChoice;
}