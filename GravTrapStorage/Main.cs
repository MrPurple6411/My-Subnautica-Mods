
namespace GravTrapStorage
{
    using System.Collections;
    using System.Reflection;
    using UnityEngine;
    using UWE;
    using Logger = QModManager.Utility.Logger;
    using HarmonyLib;
    using QModManager.API.ModLoading;
    using Configuration;
    using SMLHelper.V2.Handlers;

    [QModCore]
    public static class Main
    {
        public static Config ConfigFile { get; private set; }

        [QModPatch]
        public static void Load()
        {
            ConfigFile = OptionsPanelHandler.RegisterModOptions<Config>();
            
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), $"MrPurple6411_GravTrapStorage");
            CoroutineHost.StartCoroutine(ModifyGravspherePrefab());
        }

        public static IEnumerator ModifyGravspherePrefab()
        {
            CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(TechType.Gravsphere, false);
            yield return request;

            var prefab = request.GetResult();
            var coi = prefab.transform.GetChild(0)?.gameObject.EnsureComponent<ChildObjectIdentifier>();
            
            if (coi)
            {
                coi.classId = "GravTrapStorage";
                var storageContainer = coi.gameObject.EnsureComponent<StorageContainer>();
                storageContainer.prefabRoot = prefab;
                storageContainer.storageRoot = coi;

                storageContainer.width = ConfigFile.Width;
                storageContainer.height = ConfigFile.Height;
                storageContainer.storageLabel = "Grav trap";
            }
            else
            {
                Logger.Log(Logger.Level.Error, $"Failed to add COI. Unable to attach storage!");
            }
        }
    }
}