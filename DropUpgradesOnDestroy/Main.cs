namespace DropUpgradesOnDestroy
{
    using HarmonyLib;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "DropUpgradesOnDestroy",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";
        #endregion

        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }

        internal static void SpawnModuleNearby(List<InventoryItem> equipment, Vector3 position)
        {
            foreach(var item in equipment)
            {
                item.item.Drop(new Vector3(position.x + Random.Range(-3, 3), position.y + Random.Range(5, 8), position.z + Random.Range(-3, 3)));
            }
        }
    }
}
