namespace ImprovedPowerNetwork.Configuration
{
    using SMLHelper.V2.Json;
    using SMLHelper.V2.Options.Attributes;

    [Menu("Improved Power Network")]
    public class Config: ConfigFile
    {
        [Toggle("Line of Sight Required (Blue Beam)", Tooltip = "Makes the main power beam require Line of Sight so it (mostly) wont go through the ground.")]
        public bool LOSBlue = false;

        [Slider("Blue Beam Range", 100, 1000, DefaultValue = 400, Step = 10, Tooltip = "Max range that the main power line of a transmitter can reach.")]
        public int BlueBeamRange = 400;

        [Slider("Green Beam Range", 1, 100, DefaultValue = 15, Step = 1, Tooltip = "Max range that the machine connection line of a transmitter can reach.")]
        public int GreenBeamRange = 15;
    }
}
