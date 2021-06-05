#if SN1
namespace ScannableTimeCapsules
{
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using System.Reflection;
    using UWE;

    [QModCore]
    public static class Main
    {
        internal static Assembly assembly = Assembly.GetExecutingAssembly();

        [QModPatch]
        public static void Load()
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