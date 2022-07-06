
using QModInstaller.BepInEx.Plugins;

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

        [QModPrePatch]
        public static void Load()
        {
            ConfigFile = OptionsPanelHandler.RegisterModOptions<Config>();
            
            var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), $"MrPurple6411_GravTrapStorage");
#if SUBNAUTICA_STABLE
            CoroutineHost.StartCoroutine(ModifyGravspherePrefab());
#else
            harmony.Patch(
                AccessTools.Method(
                    typeof(PlatformUtils), nameof(PlatformUtils.PlatformInitAsync)
                ),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(Main), nameof(Main.Postfix)))
            );
#endif
            Logger.Log(Logger.Level.Info, $" Loaded.");
        }

        private static IEnumerator Postfix(IEnumerator result)
        {
            while (result.MoveNext())
            {
                yield return result;
            }
            Logger.Log(Logger.Level.Debug, $" Starting Coroutine.");
            CoroutineHost.StartCoroutine(ModifyGravspherePrefab());
        }
        
        public static IEnumerator ModifyGravspherePrefab()
        {
            Logger.Log(Logger.Level.Info, $" Attempting to Attach Storage");
            CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(TechType.Gravsphere, false);
            yield return request;

            var prefab = request.GetResult();
            Logger.Log(Logger.Level.Debug, $" Ensuring COI");
            var coi = prefab.transform.GetChild(0)?.gameObject.EnsureComponent<ChildObjectIdentifier>();
            
            if (coi)
            {
                Logger.Log(Logger.Level.Debug, $"Attaching Storage");
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
