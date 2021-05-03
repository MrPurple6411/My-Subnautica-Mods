namespace CustomCommands
{
    using CustomCommands.MonoBehaviours;
    using QModManager.API.ModLoading;
    using System.Collections.Generic;
#if SN1
    using QModManager.Utility;
    using System;
#endif

    [QModCore]
    public static class Main
    {
#if SN1
        internal static Dictionary<TechType, WaterParkCreatureParameters> CreatureParameters = new Dictionary<TechType, WaterParkCreatureParameters>()
        {
            { TechType.ReaperLeviathan, new WaterParkCreatureParameters(0.01f, 0.05f, 1f, 3f, false) },
            { TechType.SeaDragon, new WaterParkCreatureParameters(0.01f, 0.05f, 1f, 3f, false) },
            { TechType.GhostLeviathan, new WaterParkCreatureParameters(0.01f, 0.05f, 1f, 3f, false) },
            { TechType.SeaEmperorJuvenile, new WaterParkCreatureParameters(0.01f, 0.05f, 1f, 3f, false) },
            { TechType.SeaEmperorBaby, new WaterParkCreatureParameters(0.1f, 0.3f, 1f, 3f, false) },
            { TechType.GhostLeviathanJuvenile, new WaterParkCreatureParameters(0.01f, 0.05f, 1f, 3f, false) },
            { TechType.Warper, new WaterParkCreatureParameters(0.05f, 0.2f, 1f, 3f, false) },
        };
#endif

        [QModPatch]
        public static void Load()
        {
#if SN1
            foreach(KeyValuePair<TechType, WaterParkCreatureParameters> pair in CreatureParameters)
            {
                try
                {
                    WaterParkCreature.waterParkCreatureParameters[pair.Key] = pair.Value;
                }
                catch(Exception e)
                {
                    Logger.Log(Logger.Level.Debug, $"Failed to add {pair.Key} into the Aquarium Database.", e);
                }
            }
#endif
            Placeholder.Awake();
        }
    }
}