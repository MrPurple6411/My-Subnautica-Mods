namespace BetterACU.Configuration
{
    using SMLHelper.V2.Json;
    using SMLHelper.V2.Options.Attributes;
    using System.Collections.Generic;

    [Menu("Better ACU", LoadOn = MenuAttribute.LoadEvents.MenuOpened | MenuAttribute.LoadEvents.MenuRegistered)]
    public class Config: ConfigFile
    {
        [Toggle("Breed Into Ocean")]
        public bool OceanBreeding = false;

        [Toggle("Overflow into Bio-Reactors")]
        public bool BioReactorOverflow = true;

        [Toggle("Overflow into Alterra-Gen")]
        public bool AlterraGenOverflow = true;

        [Slider("Alien Containment Limit", 10, 100, DefaultValue = 10, Step = 1)]
        public int WaterParkSize = 10;

#if BZ
        [Slider("Large Room Alien Containment Limit", 20, 200, DefaultValue = 20, Step = 1)]
        public int LargeWaterParkSize = 20;
#endif

        [Toggle("Enable Power Generation")]
        public bool EnablePowerGeneration = false;

        [Slider("Overall Power Generation Multiplier", 1, 100, DefaultValue = 1, Step = 1)]
        public int PowerGenSpeed = 1;

        public Dictionary<TechType, float> CreaturePowerGeneration = new() {
#if SN1
            { TechType.Shocker, 2f },
            { TechType.CrabSquid, 1f },
            { TechType.GhostLeviathan, 10f },
            { TechType.GhostLeviathanJuvenile, 3f },
            { TechType.Warper, 1f }
#elif BZ
            { TechType.Jellyfish, 2f },
            { TechType.SquidShark, 1f }
#endif
        };

        public List<TechType> OceanBreedWhiteList =
#if SN1
            new() { TechType.Peeper, TechType.Bladderfish, TechType.Boomerang, TechType.Cutefish, TechType.Eyeye, TechType.GarryFish, TechType.GhostRayBlue, TechType.GhostRayRed, TechType.HoleFish, TechType.Hoopfish, TechType.Hoverfish, TechType.Jellyray, TechType.LavaBoomerang, TechType.Oculus, TechType.RabbitRay, TechType.LavaEyeye, TechType.Reginald, TechType.Spadefish, TechType.Spinefish };
#elif BZ
            new List<TechType>() { TechType.ArcticPeeper, TechType.ArcticRay, TechType.ArrowRay, TechType.Bladderfish, TechType.Boomerang, TechType.DiscusFish, TechType.FeatherFish, TechType.FeatherFishRed, TechType.Hoopfish, TechType.SeaMonkey, TechType.Spinefish, TechType.SpinnerFish, TechType.TitanHolefish, TechType.Triops, TechType.TrivalveBlue, TechType.TrivalveYellow };
#endif
        
        public List<TechType> AlterraGenBlackList =
#if SN1
            new() { TechType.Cutefish };
#elif BZ
            new List<TechType>() { TechType.TrivalveBlue, TechType.TrivalveYellow };
#endif

        public List<TechType> BioReactorBlackList =
#if SN1
            new() { TechType.Cutefish };
#elif BZ
            new List<TechType>() { TechType.TrivalveBlue, TechType.TrivalveYellow };
#endif
        public Dictionary<string, float> PowerValues = new();
    }
}
