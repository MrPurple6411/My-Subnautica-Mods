namespace BetterACU.Configuration
{
    using SMLHelper.Json;
    using SMLHelper.Options.Attributes;
    using System.Collections.Generic;

    [Menu("Better ACU", LoadOn = MenuAttribute.LoadEvents.MenuOpened | MenuAttribute.LoadEvents.MenuRegistered)]
    public class SMLConfig: ConfigFile
    {
        [Toggle("Breed Into Ocean")]
        public bool OceanBreeding = false;

        [Toggle("Overflow into Bio-Reactors")]
        public bool BioReactorOverflow = true;

        [Toggle("Overflow into Alterra-Gen")]
        public bool AlterraGenOverflow = true;

        [Slider("Alien Containment Limit", 10, 100, DefaultValue = 10, Step = 1)]
        public int WaterParkSize = 10;

        [Slider("Large Room Alien Containment Limit", 20, 200, DefaultValue = 20, Step = 1)]
        public int LargeWaterParkSize = 20;

        [Toggle("Enable Power Generation")]
        public bool EnablePowerGeneration = false;

        [Slider("Overall Power Generation Multiplier", 1, 100, DefaultValue = 1, Step = 1)]
        public int PowerGenSpeed = 1;

        public Dictionary<string, float> CreaturePowerGeneration = new() {
#if SN1
            { "Shocker", 2f },
            { "CrabSquid", 1f },
            { "GhostLeviathan", 10f },
            { "GhostLeviathanJuvenile", 3f },
            { "Warper", 1f }
#elif BZ
            { "Jellyfish", 2f },
            { "SquidShark", 1f }
#endif
        };

        public List<string> OceanBreedWhiteList =
#if SN1
            new() { "Peeper", "Bladderfish", "Boomerang", "Cutefish", "Eyeye", "GarryFish", "GhostRayBlue",
                "GhostRayRed", "HoleFish", "Hoopfish", "Hoverfish", "Jellyray", "LavaBoomerang", "Oculus",
                "RabbitRay", "LavaEyeye", "Reginald", "Spadefish", "Spinefish" };
#elif BZ
            new () { "ArcticPeeper", "ArcticRay", "ArrowRay", "Bladderfish", "Boomerang", "DiscusFish", "FeatherFish", 
                "FeatherFishRed", "Hoopfish", "SeaMonkey", "Spinefish", "SpinnerFish", "TitanHolefish", "Triops", 
                "TrivalveBlue", "TrivalveYellow" };
#endif
        
        public List<string> AlterraGenBlackList =
#if SN1
            new() { "Cutefish" };
#elif BZ
            new() { "TrivalveBlue", "TrivalveYellow" };
#endif

        public List<string> BioReactorBlackList =
#if SN1
            new() { "Cutefish" };
#elif BZ
            new() { "TrivalveBlue", "TrivalveYellow" };
#endif
        public Dictionary<string, float> PowerValues = new();
    }
}
