namespace GravTrapStorage
{
    using System.Collections;
    using System.Reflection;
    using UnityEngine;
    using HarmonyLib;
    using Configuration;
    using SMLHelper.Handlers;

    using BepInEx;
    using BepInEx.Logging;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        #region[Declarations]
        public const string
            MODNAME = "GravTrapStorage",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";
        public static SMLConfig SMLConfig { get; private set; }
        internal static ManualLogSource logSource;
        #endregion

        private void Awake()
        {
            logSource = Logger;
            SMLConfig = OptionsPanelHandler.RegisterModOptions<SMLConfig>();
            
            var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
            harmony.Patch(
                AccessTools.Method(
                    typeof(PlatformUtils), nameof(PlatformUtils.PlatformInitAsync)
                ),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(Main), nameof(Main.Postfix)))
            );
            Logger.Log(LogLevel.Info, $" Loaded.");
        }

        public static IEnumerator Postfix(IEnumerator result)
        {
            yield return result;
            logSource.Log(LogLevel.Info, $" Starting Coroutine.");
            yield return ModifyGravspherePrefab();
        }
        
        public static IEnumerator ModifyGravspherePrefab()
        {
            logSource.Log(LogLevel.Info, $" Attempting to Attaching Storage");
            CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(TechType.Gravsphere, false);
            yield return request;

            var prefab = request.GetResult();
            logSource.Log(LogLevel.Info, $" Ensuring COI");
            var coi = prefab.transform.GetChild(0)?.gameObject.EnsureComponent<ChildObjectIdentifier>();
            
            if (coi)
            {
                logSource.Log(LogLevel.Info, $"Attaching Storage");
                coi.classId = "GravTrapStorage";
                var storageContainer = coi.gameObject.EnsureComponent<StorageContainer>();
                storageContainer.prefabRoot = prefab;
                storageContainer.storageRoot = coi;

                storageContainer.width = SMLConfig.Width;
                storageContainer.height = SMLConfig.Height;
                storageContainer.storageLabel = "Grav trap";
            }
            else
            {
                logSource.Log(LogLevel.Error, $"Failed to add COI. Unable to attach storage!");
            }
        }
    }
}