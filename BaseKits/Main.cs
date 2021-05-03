namespace BaseKits
{
    using BaseKits.Prefabs;
    using QModManager.API.ModLoading;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UWE;

    [QModCore]
    public static class Main
    {
        private static readonly List<TechType> RoomsToClone = new List<TechType>()
        {
            TechType.BaseFoundation, TechType.BaseMapRoom, TechType.BaseMoonpool,
            TechType.BaseObservatory, TechType.BaseRoom
        };

        private static readonly List<TechType> CorridorsToClone = new List<TechType>()
        {
            TechType.BaseCorridorGlassI, TechType.BaseCorridorGlassL,
            TechType.BaseCorridorI, TechType.BaseCorridorL, TechType.BaseCorridorT, TechType.BaseCorridorX

        };

        private static readonly List<TechType> ModulesToClone = new List<TechType>()
        {
            TechType.BaseBioReactor, TechType.BaseFiltrationMachine,
            TechType.BaseNuclearReactor, TechType.BaseUpgradeConsole, TechType.BaseWaterPark
        };

        private static readonly List<TechType> UtilitiesToClone = new List<TechType>()
        {
            TechType.BaseConnector, TechType.BaseBulkhead, TechType.BaseHatch,
            TechType.BaseLadder, TechType.BaseReinforcement, TechType.BaseWindow
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

            List<TechType> ClonedRoomKits = new List<TechType>();
            ProcessTypes(RoomsToClone, ref ClonedRoomKits);

            List<TechType> ClonedCorridorKits = new List<TechType>();
            ProcessTypes(CorridorsToClone, ref ClonedCorridorKits);

            List<TechType> ClonedModuleKits = new List<TechType>();
            ProcessTypes(ModulesToClone, ref ClonedModuleKits);

            List<TechType> ClonedUtilityKits = new List<TechType>();
            ProcessTypes(UtilitiesToClone, ref ClonedUtilityKits);


            new KitFabricator(ClonedRoomKits, ClonedCorridorKits, ClonedModuleKits, ClonedUtilityKits).Patch();
        }

        private static void ProcessTypes(List<TechType> typesToClone, ref List<TechType> clonedKits)
        {
            foreach(TechType techType in typesToClone)
            {
                CloneBaseKit cbk = new CloneBaseKit(techType);
                cbk.Patch();

                clonedKits.Add(cbk.TechType);

                CloneBasePiece cbp = new CloneBasePiece(techType, cbk.TechType);
                cbp.Patch();
            }
        }
    }
}