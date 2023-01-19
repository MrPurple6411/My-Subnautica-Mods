namespace BaseKits
{
    using Prefabs;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UWE;
    using BepInEx;
    using BepInEx.Logging;
    using SMLHelper.Handlers;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]

        public const string
            MODNAME = "BaseKits",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        private static readonly List<TechType> RoomsToClone = new()
        {
            TechType.BaseFoundation, TechType.BaseMapRoom, TechType.BaseMoonpool,
            TechType.BaseObservatory, TechType.BaseRoom
            ,TechType.BaseLargeRoom,TechType.BaseGlassDome,TechType.BaseLargeGlassDome, TechType.BasePartitionDoor,
            TechType.BasePartition
#if BZ
            , TechType.BaseControlRoom
#endif
        };

        private static readonly List<TechType> CorridorsToClone = new()
        {
            TechType.BaseCorridorGlassI, TechType.BaseCorridorGlassL,
            TechType.BaseCorridorI, TechType.BaseCorridorL, TechType.BaseCorridorT, TechType.BaseCorridorX

        };

        private static readonly List<TechType> ModulesToClone = new()
        {
            TechType.BaseBioReactor, TechType.BaseFiltrationMachine,
            TechType.BaseNuclearReactor, TechType.BaseUpgradeConsole, TechType.BaseWaterPark
        };

        private static readonly List<TechType> UtilitiesToClone = new()
        {
            TechType.BaseConnector, TechType.BaseBulkhead, TechType.BaseHatch,
            TechType.BaseLadder, TechType.BaseReinforcement, TechType.BaseWindow
            
        };

        internal const string LangFormat = "{0}Menu_{1}";
        internal const string SpriteFormat = "{0}_{1}";
        internal const string KitFab = "PurpleKitFabricator";
        internal const string RoomsMenu = "RoomsMenu";
        internal const string CorridorMenu = "CorridorMenu";
        internal const string ModuleMenu = "ModuleMenu";
        internal const string UtilityMenu = "UtilityMenu";

        internal static ManualLogSource logSource;
        #endregion

        public void Awake()
        {
            logSource = Logger;
            CoroutineHost.StartCoroutine(RegisterKits());
        }

        public CraftTree.Type PurpleKitFabricator { get; private set; }

        private IEnumerator RegisterKits()
        {
            if(Language.main is null)
                yield return new WaitWhile(() => Language.main is null);

            PurpleKitFabricator = new KitFabricator(KitFab).TreeTypeID;

            ProcessTypes(RoomsToClone, RoomsMenu);
            ProcessTypes(CorridorsToClone, CorridorMenu);
            ProcessTypes(ModulesToClone, ModuleMenu);
            ProcessTypes(UtilitiesToClone, UtilityMenu);
        }

        private void ProcessTypes(List<TechType> typesToClone, string FabricatorMenu)
        {
            foreach(var techType in typesToClone)
            {
                new CloneBasePiece(techType, new CloneBaseKit(techType, FabricatorMenu, PurpleKitFabricator).PrefabInfo.TechType);
            }
        }
    }
}