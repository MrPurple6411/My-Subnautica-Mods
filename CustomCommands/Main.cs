using BepInEx;

namespace CustomCommands
{
    using MonoBehaviours;
#if SN1
    using System.Collections.Generic;
    using SMCLib.Utility;
    using System;
#endif

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
#if SN1
        private static readonly Dictionary<TechType, WaterParkCreatureParameters> CreatureParameters = new()
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

        public void Start()
        {
#if SN1
            foreach(var pair in CreatureParameters)
            {
                try
                {
                    WaterParkCreature.waterParkCreatureParameters[pair.Key] = pair.Value;
                }
                catch(Exception e)
                {
                    Logger.LogDebug($"Failed to add {pair.Key} into the Aquarium Database.\n{e}");
                }
            }
#endif
            Placeholder.Awake();
        }
    }
}