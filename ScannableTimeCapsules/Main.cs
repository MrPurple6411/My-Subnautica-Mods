#if SUBNAUTICA
namespace ScannableTimeCapsules;

using HarmonyLib;
using System.Reflection;
using UWE;
using BepInEx;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInProcess("Subnautica.exe")]
public class Main : BaseUnityPlugin
{
	private void Awake()
	{
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
		var classid = CraftData.GetClassIdForTechType(TechType.TimeCapsule);
		if (WorldEntityDatabase.TryGetInfo(classid, out var worldEntityInfo))
		{
			worldEntityInfo.cellLevel = LargeWorldEntity.CellLevel.VeryFar;

			WorldEntityDatabase.main.infos[classid] = worldEntityInfo;
		}
	}
}
#endif