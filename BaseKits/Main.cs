namespace BaseKits
{
    using Prefabs;
    using QModManager.API.ModLoading;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UWE;

    [QModCore]
    public static class Main
    {
        private static readonly List<TechType> RoomsToClone = new()
        {
            TechType.BaseFoundation, TechType.BaseMapRoom, TechType.BaseMoonpool,
            TechType.BaseObservatory, TechType.BaseRoom
#if BZ
            ,TechType.BaseLargeRoom, TechType.BaseControlRoom
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
#if BZ
            ,TechType.BaseGlassDome,TechType.BaseLargeGlassDome, TechType.BasePartitionDoor   
#endif
        };

        [QModPostPatch]
        public static void Load()
        {
            CoroutineHost.StartCoroutine(RegisterKits());
        }

        private static IEnumerator RegisterKits()
        {
            if(Language.main is null)
                yield return new WaitWhile(() => Language.main is null);

            var ClonedRoomKits = new List<TechType>();
            ProcessTypes(RoomsToClone, ref ClonedRoomKits);

            var ClonedCorridorKits = new List<TechType>();
            ProcessTypes(CorridorsToClone, ref ClonedCorridorKits);

            var ClonedModuleKits = new List<TechType>();
            ProcessTypes(ModulesToClone, ref ClonedModuleKits);

            var ClonedUtilityKits = new List<TechType>();
            ProcessTypes(UtilitiesToClone, ref ClonedUtilityKits);


            new KitFabricator(ClonedRoomKits, ClonedCorridorKits, ClonedModuleKits, ClonedUtilityKits).Patch();
        }

        private static void ProcessTypes(List<TechType> typesToClone, ref List<TechType> clonedKits)
        {
            foreach(var techType in typesToClone)
            {
                var cbk = new CloneBaseKit(techType);
                cbk.Patch();

                clonedKits.Add(cbk.TechType);

                var cbp = new CloneBasePiece(techType, cbk.TechType);
                cbp.Patch();
            }
        }
    }
}