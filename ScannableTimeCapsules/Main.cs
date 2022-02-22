using BepInEx;

#if SN1
namespace ScannableTimeCapsules
{
    using HarmonyLib;
    using System.Reflection;
    using UWE;

    public class Main:BaseUnityPlugin
    {
        internal static Assembly assembly = Assembly.GetExecutingAssembly();

        public void  Awake()
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