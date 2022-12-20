#if SN1
namespace ScannableTimeCapsules
{
    using HarmonyLib;
    using System.Reflection;
    using UWE;
    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        internal static Assembly assembly = Assembly.GetExecutingAssembly();

        #region[Declarations]

        public const string
            MODNAME = "ScannableTimeCapsules",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        #endregion

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
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