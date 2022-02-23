using BepInEx;

#if SN1
namespace ScannableTimeCapsules
{
    using HarmonyLib;
    using System.Reflection;
    using UWE;

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
        internal static Assembly assembly = Assembly.GetExecutingAssembly();

        public void Start()
        {
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
            var classid = CraftData.GetClassIdForTechType(TechType.TimeCapsule);
            if(WorldEntityDatabase.TryGetInfo(classid, out var worldEntityInfo))
            {
                worldEntityInfo.cellLevel = LargeWorldEntity.CellLevel.VeryFar;

                WorldEntityDatabase.main.infos[classid] = worldEntityInfo;
            }
        }
    }
}
#endif