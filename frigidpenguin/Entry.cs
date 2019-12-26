using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using Harmony;
using System.Reflection;
using System.IO;
using System.Diagnostics;

/**
 * Sorry, too lazy to do separate builds. Mods are activated based on file name.
 */
namespace frigidpenguin
{
    public class Entry
    {
        private static HarmonyInstance harmony;

        private static void Log(String msg, params object[] args)
        {
            UnityEngine.Debug.Log(String.Format("[frigidpenguin] " + msg, args));
        }

        private static void Log(Type type, String msg, params object[] args)
        {
            UnityEngine.Debug.Log(String.Format("[frigidpenguin:" + type.Name + "] " + msg, args));
        }

        private static void Msg(String msg, params object[] args)
        {
            ErrorMessage.AddMessage(String.Format(msg, args));
        }

        public static void Start()
        {
            Patch();
        }

        public static void Patch()
        {
            Log("Yoo wee are inside!!");

            var asm = Assembly.GetExecutingAssembly();
            var name = Path.GetFileName(Assembly.GetExecutingAssembly().Location)
                    .Replace(".dll", "");
            Log("DLL Name: {0}", name);

            harmony = HarmonyInstance.Create("frigidpenguin." + name);
            if (name == "dev")
            {
                harmony.PatchAll(asm);
            }
            else
            {
                var type = asm.GetType(typeof(Entry).FullName + "+" + name);

                Log("Patching for mod: {0}", type);
                type.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Static).Do(t =>
                {
                    var patches = t.GetHarmonyMethods();
                    if (patches.Count() == 0)
                    {
                        return;
                    }
                    Log("Applying patch: {0}", t);
                    var merged = HarmonyMethod.Merge(patches);
                    new PatchProcessor(harmony, t, merged).Patch();
                });
            }
            Log("Harmony patches applied.");
        }

        private void Debug(GameObject go)
        {
            if (!go || go.name == "Debug")
            {
                return;
            }
            if (!go.transform.Find("Debug") && go.name.StartsWith("WorldMeshes"))
            {
                Log("Creating debug sphere for: {0}", go);
                var debug = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                debug.name = "Debug";
                debug.GetComponent<SphereCollider>().enabled = false;
                debug.transform.SetParent(go.transform, false);
                //debug.transform.localScale = Vector3.one * 0.1f;\\u
            }
            for (var i = 0; i < go.transform.childCount; ++i)
            {
                Debug(go.transform.GetChild(i).gameObject);
            }
        }

        private static void Dump(GameObject go, Action<String, object[]> log)
        {
            var builder = new StringBuilder();
            Dump(go, builder, 0);
            log("DUMP-----\n{0}", new object[] { builder.ToString() });
        }

        private static void Dump(GameObject go, StringBuilder builder, int level)
        {
            var indent = new String('\t', level);
            var components = go.GetComponents<Component>();
            builder.Append(indent);
            builder.Append(String.Format("{0}[{2}] ({1} components, {3} children):\n", go, components.Length - 1, go.GetInstanceID(), go.transform.childCount));
            indent += "\t";
            builder.Append(indent);
            builder.Append(String.Format("localPosition: {0}, localScale: {1}\n", go.transform.localPosition, go.transform.localScale));
            builder.Append(indent);
            builder.Append(String.Format("position: {0}\n", go.transform.position));
            foreach (var component in components)
            {
                if (component is Transform)
                {
                    continue;
                }
                builder.Append(indent);
                builder.Append(String.Format("{0}[{1}]\n", component, component.GetInstanceID()));
                if (component is MeshFilter)
                {
                    var mf = component as MeshFilter;
                    builder.Append(String.Format(indent + "\tsharedMesh = {0}\n", mf.sharedMesh));
                }
            }
            for (var i = 0; i < go.transform.childCount; ++i)
            {
                Dump(go.transform.GetChild(i).gameObject, builder, level + 1);
            }
        }

        private static class LightVolumetrizer
        {
            public static readonly SeaMoth template = Resources.Load<GameObject>("WorldEntities/Tools/SeaMoth").GetComponent<SeaMoth>();

            public static void VolumetrizeLights(GameObject lightGO, float intensityFactor, Vector3 offset)
            {
                var volumeTemplate = template.gameObject.transform.Find("lights_parent/light_left/x_FakeVolumletricLight").gameObject;
                var volumeLightTemplate = template.gameObject.transform.Find("lights_parent/light_left").gameObject
                        .GetComponent<VFXVolumetricLight>();

                var light = lightGO.GetComponent<Light>();
                var templateLight = volumeLightTemplate.GetComponent<Light>();

                var lightSpotAngle = light.spotAngle;
                var templateSpotAngle = templateLight.spotAngle;
                var spotAngleScaleFactor = lightSpotAngle / templateSpotAngle;
                Log("ANGLE {0} TEMPLATE ANGLE {1} FACTOR {2}", lightSpotAngle, templateSpotAngle, spotAngleScaleFactor);

                var lightRange = light.range;
                var templateRange = templateLight.range;
                var rangeScaleFactor = lightRange / templateRange;
                Log("RANGE {0}, TEMPLATE RANGE {1} FACTOR {2}", lightRange, templateRange, rangeScaleFactor);

                var volume = GameObject.Instantiate(volumeTemplate, lightGO.transform).gameObject;
                volume.transform.localScale = new Vector3(
                    volume.transform.localScale.x * rangeScaleFactor * spotAngleScaleFactor,
                    volume.transform.localScale.y * rangeScaleFactor * spotAngleScaleFactor,
                    volume.transform.localScale.z * rangeScaleFactor
                );
                volume.transform.localPosition += offset;

                lightGO.SetActive(false);
                var volumeLight = lightGO.gameObject.AddComponent<VFXVolumetricLight>();
                volumeLight.volumGO = volume;
                volumeLight.coneMat = volumeLightTemplate.coneMat;
                volumeLight.sphereMat = volumeLightTemplate.sphereMat;
                volumeLight.startFallof = volumeLightTemplate.startFallof;
                volumeLight.intensity = volumeLightTemplate.intensity * intensityFactor;
                lightGO.SetActive(true);
            }
        }

        private static void SetNumberOfDaysEnergyLasts(ToggleLights toggleLights, float capacity, float days)
        {
            var rate = DayNightCycle.gameSecondMultiplier * capacity / DayNightCycle.secondsInDay / days;
            toggleLights.energyPerSecond = rate;
        }

        private static T ReflGet<T>(System.Object obj, String name)
        {
            return (T)AccessTools.Field(obj.GetType(), name).GetValue(obj);
        }

        private static void ReflSet(System.Object obj, String name, System.Object value)
        {
            AccessTools.Field(obj.GetType(), name).SetValue(obj, value);
        }

        private static IEnumerator<YieldInstruction> EachFrameUntil(Func<bool> action)
        {
            while (!action())
            {
                yield return null;
            }
        }

        private static class CyclopsNearFieldSonar
        {
            private static readonly GameObject template = Resources.Load<GameObject>("WorldEntities/Tools/SeaGlide");

            private static void Log(String msg, params object[] args)
            {
                Entry.Log(MethodBase.GetCurrentMethod().DeclaringType, msg, args);
            }

            [HarmonyPatch(typeof(SubControl))]
            [HarmonyPatch(nameof(SubControl.Start))]
            private static class Patch_SubControl_Start
            {
                private static void Postfix(SubControl __instance)
                {
                    if (!__instance.gameObject.name.StartsWith("Cyclops-MainPrefab"))
                    {
                        return;
                    }

                    __instance.gameObject.AddComponent<Controller>();
                }
            }

            private class Controller : MonoBehaviour
            {
                private VehicleInterface_Terrain script;
                private Material material;
                private Vector3 position = new Vector3(-0.9762846f, 2, -10.6917f);
                private float scale = 6.699605f;
                private float fadeRadius = 1.503953f;
                private Color color = Color.black;
                private Vector3 shipPosition = new Vector3(0, 0, 0);
                private float shipScale = 0.2f;
                private GameObject ship;

                private void Start()
                {
                    var root = gameObject.transform.Find("SonarMap_Small");
                    var holder = new GameObject("NearFieldSonar");
                    holder.transform.SetParent(root, false);
                    holder.transform.localScale = Vector3.one * 0.1f;

                    var prefab = template.GetComponent<VehicleInterface_MapController>().interfacePrefab;
                    var hologram = GameObject.Instantiate(prefab);
                    hologram.transform.SetParent(holder.transform, false);

                    script = hologram.GetComponentInChildren<VehicleInterface_Terrain>();
                    script.active = true;
                    script.EnableMap();

                    StartCoroutine(EachFrameUntil(() =>
                    {
                        material = ReflGet<Material>(script, "materialInstance");
                        return UpdateParameters();
                    }));

                    ship = GameObject.Instantiate(gameObject.transform.Find("HolographicDisplay/CyclopsMini_Mid").gameObject);
                    ship.transform.SetParent(root, false);
                    ship.transform.GetComponentsInChildren<MeshRenderer>()
                        .Where(r => r.gameObject.name.StartsWith("cyclops_room_"))
                        .ForEach(r => r.enabled = false);

                    var oldShipRenderer = root.Find("CyclopsMini").GetComponent<MeshRenderer>();
                    var shipMaterial = oldShipRenderer.material;
                    ship.transform.GetComponentsInChildren<MeshRenderer>()
                        .ForEach(r => r.sharedMaterial = shipMaterial);

                    oldShipRenderer.enabled = false;
                    root.Find("Base").GetComponent<MeshRenderer>().enabled = false;
                }

                private bool UpdateParameters()
                {
                    if (!material)
                    {
                        return false;
                    }
                    script.hologramHolder.transform.localScale = Vector3.one * scale;
                    script.hologramHolder.transform.localPosition = position * (1 / scale);

                    material.SetFloat("_FadeRadius", fadeRadius);

                    /*if (color == Color.black)
                    {
                        color = ReflGet<Color>(script, "mapColor");
                    }
                    ReflSet(script, "mapColor", color);*/

                    ship.transform.localPosition = shipPosition;
                    ship.transform.localScale = Vector3.one * shipScale;
                    return true;
                }

                /*private void OnGUI()
                {
                    GUILayout.BeginVertical(GUILayout.Width(1024));
                    var text1 = String.Format("Ship pos: ({0}, {1}, {2})", position.x, position.y, position.z) + " Scale: " + scale;
                    GUILayout.Label(text1);
                    position.x = GUILayout.HorizontalSlider(position.x, -20, 20);
                    position.y = GUILayout.HorizontalSlider(position.y, -20, 20);
                    position.z = GUILayout.HorizontalSlider(position.z, -20, 20);
                    scale = GUILayout.HorizontalSlider(scale, 0, 10);
                    var text2 = "Radius: " + fadeRadius + " Color: " + color;
                    GUILayout.Label(text2);
                    fadeRadius = GUILayout.HorizontalSlider(fadeRadius, 0, 10);
                    color.r = GUILayout.HorizontalSlider(color.r, 0, 1);
                    color.g = GUILayout.HorizontalSlider(color.g, 0, 1);
                    color.b = GUILayout.HorizontalSlider(color.b, 0, 1);
                    color.a = GUILayout.HorizontalSlider(color.a, 0, 1);
                    var text3 = String.Format("Ship pos: ({0}, {1}, {2})", shipPosition.x, shipPosition.y, shipPosition.z) + " Ship scale: " + shipScale;
                    GUILayout.Label(text3);
                    shipPosition.x = GUILayout.HorizontalSlider(shipPosition.x, -1, 1);
                    shipPosition.y = GUILayout.HorizontalSlider(shipPosition.y, -1, 1);
                    shipPosition.z = GUILayout.HorizontalSlider(shipPosition.z, -1, 1);
                    shipScale = GUILayout.HorizontalSlider(shipScale, 0, 0.3f);
                    GUILayout.EndVertical();
                    UpdateParameters();
                    Log("{0} {1} {2}", text1, text2, text3);
                }*/
            }
        }

        private static class FixCyclopsVolumtricLight
        {
            private static void Log(String msg, params object[] args)
            {
                Entry.Log(MethodBase.GetCurrentMethod().DeclaringType, msg, args);
            }

            [HarmonyPatch(typeof(SubRoot))]
            [HarmonyPatch(nameof(SubRoot.OnPlayerExited))]
            private static class Patch_SubRoot_OnPlayerExited
            {
                private static void Postfix(SubRoot __instance)
                {
                    if (!__instance.gameObject.name.StartsWith("Cyclops-MainPrefab"))
                    {
                        return;
                    }
                    //Msg("OnPlayerExited");
                    __instance.transform.Find("Floodlights")
                            .GetAllComponentsInChildren<VFXVolumetricLight>()
                            .ForEach(x => x.RestoreVolume());
                }
            }

            [HarmonyPatch(typeof(SubRoot))]
            [HarmonyPatch(nameof(SubRoot.OnPlayerEntered))]
            private static class Patch_SubRoot_OnPlayerEntered
            {
                private static void Postfix(SubRoot __instance)
                {
                    if (!__instance.gameObject.name.StartsWith("Cyclops-MainPrefab"))
                    {
                        return;
                    }
                    //Msg("OnPlayerEntered");
                    __instance.transform.Find("Floodlights")
                            .GetAllComponentsInChildren<VFXVolumetricLight>()
                            .ForEach(x => x.DisableVolume());
                }
            }
        }

        private static class SeaglideMapControls
        {
            private static void Log(String msg, params object[] args)
            {
                Entry.Log(MethodBase.GetCurrentMethod().DeclaringType, msg, args);
            }

            [HarmonyPatch(typeof(Seaglide))]
            [HarmonyPatch(nameof(Seaglide.OnAltDown))]
            private static class PlayerToolOnAltDownPatch
            {
                private static void Postfix(Seaglide __instance)
                {
                    __instance.toggleLights.lightState = __instance.toggleLights.lightState == 2 ? 0 : 2;
                }
            }

            [HarmonyPatch(typeof(Seaglide))]
            [HarmonyPatch(nameof(Seaglide.Update))]
            private static class SeaglideUpdatePatch
            {
                // Keep old state so we don't trigger the map.
                // Lights are not actually controlled with the lightState!
                private static void Prefix(Seaglide __instance, ref int __state)
                {
                    __state = __instance.toggleLights.lightState;
                }

                private static void Postfix(Seaglide __instance, int __state)
                {
                    __instance.toggleLights.lightState = __state;
                }
            }

            [HarmonyPatch(typeof(Seaglide))]
            [HarmonyPatch(nameof(Seaglide.Start))]
            private static class SeaglideStartPatch
            {
                private static void Postfix(Seaglide __instance)
                {
                    // Start with map off.
                    __instance.toggleLights.lightState = 2;
                }
            }

            [HarmonyPatch(typeof(ToggleLights))]
            [HarmonyPatch(nameof(ToggleLights.OnPoweredChanged))]
            private static class Patch_ToggleLights_OnPoweredChanged
            {
                private static bool Prefix(ToggleLights __instance, bool powered)
                {
                    if (__instance.gameObject.GetComponent<Seaglide>())
                    {
                        // Don't automatically toggle on lights when power is restored.
                        if (powered)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }

        private static class MapRoomCameraLights
        {
            private static readonly MethodInfo uGUI_CameraDrone_UpdateDistanceText = AccessTools.Method(typeof(uGUI_CameraDrone), nameof(uGUI_CameraDrone.UpdateDistanceText));
            private static MapRoomScreen latestScreen;

            private static void Log(String msg, params object[] args)
            {
                Entry.Log(MethodBase.GetCurrentMethod().DeclaringType, msg, args);
            }

            [HarmonyPatch(typeof(MapRoomCamera))]
            [HarmonyPatch(nameof(MapRoomCamera.ControlCamera))]
            private static class MapRoomCameraControlCameraPatch
            {
                private static void Postfix(MapRoomCamera __instance)
                {
                    __instance.gameObject.GetComponentsInChildren<VFXVolumetricLight>(true)
                        .ForEach(x => x.DisableVolume());

                    var dockingPoint = ReflGet<MapRoomCameraDocking>(__instance, "dockingPoint");
                    __instance.lightsParent.SetActive(dockingPoint == null);

                    //Msg("Control, light " + (dockingPoint == null));
                }
            }

            [HarmonyPatch(typeof(MapRoomCamera))]
            [HarmonyPatch(nameof(MapRoomCamera.FreeCamera))]
            private static class MapRoomCameraFreeCameraPatch
            {
                private static void Postfix(MapRoomCamera __instance)
                {
                    __instance.lightsParent.SetActive(true);

                    __instance.gameObject.GetComponentsInChildren<VFXVolumetricLight>(true)
                        .ForEach(x => x.RestoreVolume());
                    //Msg("Free");

                    var dockingPoint = ReflGet<MapRoomCameraDocking>(__instance, "dockingPoint");
                    __instance.lightsParent.SetActive(dockingPoint == null);
                }
            }

            [HarmonyPatch(typeof(MapRoomCamera))]
            [HarmonyPatch(nameof(MapRoomCamera.SetDocked))]
            private static class MapRoomCameraSetDockedPatch
            {
                private static void Postfix(MapRoomCamera __instance, MapRoomCameraDocking dockingPoint)
                {
                    //Msg("Docked, light " + (dockingPoint == null));
                    __instance.lightsParent.SetActive(dockingPoint == null);
                }
            }

            [HarmonyPatch(typeof(MapRoomCamera))]
            [HarmonyPatch(nameof(MapRoomCamera.OnPickedUp))]
            private static class MapRoomCameraOnPickedUpPatch
            {
                private static void Postfix(MapRoomCamera __instance)
                {
                    //Msg("Picked up, light off");
                    __instance.lightsParent.SetActive(true);
                }
            }

            private static void OnDropped(MapRoomCamera camera)
            {
                //Msg("Dropped, light on");
                camera.lightsParent.SetActive(true);
            }


            /*[HarmonyPatch(typeof(MapRoomCamera))]
            [HarmonyPatch("Update")]
            private static class MapRoomCameraStartPatchX
            {
                private static void Postfix(MapRoomCamera __instance)
                {
                    if (GameInput.GetButtonDown(GameInput.Button.AltTool) && __instance.pickupAble.attached)
                    {
                        var node = __instance.transform;
                        Log("CAM ID: {0}", node.gameObject.GetInstanceID());
                        while (node.parent)
                        {
                            node = node.parent;
                        }
                        Dump(node.gameObject, Log);
                    }
                }
            }*/

            [HarmonyPatch(typeof(MapRoomCamera))]
            [HarmonyPatch(nameof(MapRoomCamera.Start))]
            private static class MapRoomCameraStartPatch
            {
                private static void Postfix(MapRoomCamera __instance)
                {
                    var xx = __instance.gameObject.GetComponent<ToggleLights>();
                    while (xx)
                    {
                        UnityEngine.Object.DestroyImmediate(xx);
                        xx = __instance.gameObject.GetComponent<ToggleLights>();
                    }

                    __instance.pickupAble.droppedEvent.AddHandler(__instance.gameObject, x => OnDropped(__instance));
                    if (!__instance.pickupAble.attached)
                    {
                        OnDropped(__instance);
                    }

                    foreach (var light in __instance.lightsParent.GetComponentsInChildren<Light>(true))
                    {
                        //light.intensity *= 0.5f;
                        //light.range *= 0.5f;
                        light.spotAngle *= 0.5f;
                    }

                    LightVolumetrizer.VolumetrizeLights(__instance.gameObject.transform.Find("lights_parent/light_top").gameObject, 0.85f, Vector3.zero);
                    LightVolumetrizer.VolumetrizeLights(__instance.gameObject.transform.Find("lights_parent/light_bottom").gameObject, 0.85f, Vector3.zero);
                }
            }

            [HarmonyPatch(typeof(MapRoomScreen))]
            [HarmonyPatch(nameof(MapRoomScreen.OnHandClick))]
            private static class Patch_MapRoomScreen_OnHandClick
            {
                private static void Postfix(MapRoomScreen __instance)
                {
                    latestScreen = __instance;
                    UpdateDistance();
                }
            }

            [HarmonyPatch(typeof(uGUI_CameraDrone))]
            [HarmonyPatch(nameof(uGUI_CameraDrone.SetCamera))]
            private static class Patch_uGUI_CameraDrone_SetCamera
            {
                private static void Postfix(uGUI_CameraDrone __instance, MapRoomCamera camera)
                {
                    UpdateDistance(camera);
                }
            }

            private static void UpdateDistance(MapRoomCamera latestCamera = null)
            {
                var gui = uGUI_CameraDrone.main;

                var camera = gui.GetCamera() ?? latestCamera;
                var screen = gui.GetScreen() ?? latestScreen;
                if (camera && screen)
                {
                    int distance = Mathf.FloorToInt(camera.GetScreenDistance(latestScreen));
                    uGUI_CameraDrone_UpdateDistanceText.Invoke(gui, new object[] { distance });
                }
            }

            /*[HarmonyPatch(typeof(FPModel))]
            [HarmonyPatch("OnEquip")]
            private static class Patch_FPModel_OnEquip
            {
                private static void Postfix(FPModel __instance)
                {
                    if (__instance.GetComponent<MapRoomCamera>())
                    {
                        var component = __instance.gameObject.GetComponent<DebugComponent>();
                        if (!component)
                        {
                            component = __instance.gameObject.AddComponent<DebugComponent>();
                        }
                        component.component = __instance;

                        //__instance.viewModel.transform.parent.localRotation = Quaternion.Euler(-20.63241f, -91.06719f, 5.335968f);
                        //__instance.viewModel.transform.parent.localPosition = new Vector3(0.08300396f, -0.08498023f, 0.312253f);
                    }
                }
            }

            private class DebugComponent : MonoBehaviour
            {
                public FPModel component;
                private Vector3 angles = Vector3.zero;
                private Vector3 pos = Vector3.zero;

                protected void OnGUI()
                {
                    GUILayout.BeginVertical(GUILayout.Width(1024));
                    var text1 = String.Format("ROT: ({0}, {1}, {2})", angles.x, angles.y, angles.z);
                    GUILayout.Label(text1);
                    angles.x = GUILayout.HorizontalSlider(angles.x, -180, 180);
                    angles.y = GUILayout.HorizontalSlider(angles.y, -180, 180);
                    angles.z = GUILayout.HorizontalSlider(angles.z, -180, 180);
                    var text2 = String.Format("POS: ({0}, {1}, {2})", pos.x, pos.y, pos.z);
                    GUILayout.Label(text2);
                    pos.x = GUILayout.HorizontalSlider(pos.x, -1, 1);
                    pos.y = GUILayout.HorizontalSlider(pos.y, -1, 1);
                    pos.z = GUILayout.HorizontalSlider(pos.z, -1, 1);
                    GUILayout.EndVertical();
                    Log("{0} {1}", text1, text2);

                    component.viewModel.transform.parent.localRotation = Quaternion.Euler(angles.x, angles.y, angles.z);
                    component.viewModel.transform.parent.localPosition = pos;
                }
            }*/
        }

        private static class PrawnsuitVolumetricLights
        {
            private static void Log(String msg, params object[] args)
            {
                Entry.Log(MethodBase.GetCurrentMethod().DeclaringType, msg, args);
            }

            [HarmonyPatch(typeof(Exosuit))]
            [HarmonyPatch(nameof(Exosuit.Start))]
            private static class Patch_Exosuit_Start
            {
                private static void Postfix(Exosuit __instance)
                {
                    var offset = new Vector3(0, 0.3f, -0.5f);
                    LightVolumetrizer.VolumetrizeLights(__instance.gameObject.transform.Find("lights_parent/light_left").gameObject, 0.85f, offset);
                    LightVolumetrizer.VolumetrizeLights(__instance.gameObject.transform.Find("lights_parent/light_right").gameObject, 0.85f, offset);
                    Log("Lights volumetrized.");
                }
            }

            [HarmonyPatch(typeof(Exosuit))]
            [HarmonyPatch(nameof(Exosuit.OnPilotModeBegin))]
            private static class Patch_Exosuit_OnPilotModeBegin
            {
                private static void Postfix(Exosuit __instance)
                {
                    __instance.GetAllComponentsInChildren<VFXVolumetricLight>()
                                .ForEach(x => x.DisableVolume());
                }
            }

            [HarmonyPatch(typeof(Exosuit))]
            [HarmonyPatch(nameof(Exosuit.OnPilotModeEnd))]
            private static class Patch_Exosuit_OnPilotModeEnd
            {
                private static void Postfix(Exosuit __instance)
                {
                    __instance.GetAllComponentsInChildren<VFXVolumetricLight>()
                                .ForEach(x => x.RestoreVolume());
                }
            }
        }

        private static class ExosuitToggleLights
        {
            private static void Log(String msg, params object[] args)
            {
                Entry.Log(MethodBase.GetCurrentMethod().DeclaringType, msg, args);
            }

            [HarmonyPatch(typeof(Exosuit))]
            [HarmonyPatch(nameof(Exosuit.Start))]
            private static class Patch_Exosuit_Start
            {
                private static void Postfix(Exosuit __instance)
                {
                    //Dump(__instance.gameObject, Log);
                    var toggleLights = __instance.GetComponent<ToggleLights>();
                    if (!toggleLights)
                    {
                        toggleLights = __instance.gameObject.AddComponent<ToggleLights>();
                    }

                    var template = LightVolumetrizer.template.toggleLights;
                    toggleLights.lightsOnSound = template.lightsOnSound;
                    toggleLights.lightsOffSound = template.lightsOffSound;
                    toggleLights.onSound = template.onSound;
                    toggleLights.offSound = template.offSound;
                    toggleLights.lightsParent = __instance.transform.Find("lights_parent").gameObject;
                    toggleLights.energyMixin = __instance.GetComponent<EnergyMixin>();
                    Log("EnergyPerSecond Before: {0}", toggleLights.energyPerSecond);
                    SetNumberOfDaysEnergyLasts(toggleLights, 400, 4);
                    Log("EnergyPerSecond: {0}", toggleLights.energyPerSecond);

                    toggleLights.SetLightsActive(false);
                    Log("ToggleLights component added.");
                }
            }

            [HarmonyPatch(typeof(Exosuit))]
            [HarmonyPatch(nameof(Exosuit.Update))]
            private static class Patch_Exosuit_Update
            {
                private static void Postfix(Exosuit __instance)
                {
                    var component = __instance.GetComponent<ToggleLights>();
                    if (component == null)
                    {
                        return;
                    }
                    component.UpdateLightEnergy();
                    if (__instance.GetPilotingMode() && GameInput.GetButtonDown(GameInput.Button.AltTool))
                    {
                        component.SetLightsActive(!component.lightsActive);
                    }
                }
            }
        }

        private static class SeamothLightUsesEnergy
        {
            private static void Log(String msg, params object[] args)
            {
                Entry.Log(MethodBase.GetCurrentMethod().DeclaringType, msg, args);
            }

            [HarmonyPatch(typeof(SeaMoth))]
            [HarmonyPatch(nameof(SeaMoth.Start))]
            private static class Patch_SeaMoth_Start
            {
                private static void Postfix(SeaMoth __instance)
                {
                    //Dump(__instance.gameObject, Log);

                    var toggleLights = __instance.GetComponentInChildren<ToggleLights>(true);
                    Log("toggleLights: {0}", toggleLights != null);
                    Log("EnergyPerSecond Before: {0}", toggleLights.energyPerSecond);
                    SetNumberOfDaysEnergyLasts(toggleLights, 200, 1);
                    Log("EnergyPerSecond: {0}", toggleLights.energyPerSecond);

                    toggleLights.SetLightsActive(false);
                }
            }

            [HarmonyPatch(typeof(ToggleLights))]
            [HarmonyPatch(nameof(ToggleLights.OnPoweredChanged))]
            private static class Patch_ToggleLights_OnPoweredChanged
            {
                private static bool Prefix(ToggleLights __instance, bool powered)
                {
                    if (__instance.gameObject.GetComponentInParent<SeaMoth>())
                    {
                        // Don't automatically toggle on lights when power is restored.
                        if (powered)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }

        private static class NoCrosshair
        {
            private static readonly MethodInfo HandReticle_SetIconInternal = AccessTools.Method(typeof(HandReticle), nameof(HandReticle.SetIconInternal));
            private static readonly String SCANNER_ROOM = "SCANNER_ROOM";
            private static uGUI_HandReticleIcon icon;

            private static PurgableStack<System.Object> stack = new PurgableStack<System.Object>();
            private static int currentRequestId;
            private static Coroutine delayedHandler;
            private static String lastStackUpdate;

            private static void Log(String msg, params object[] args)
            {
                Entry.Log(MethodBase.GetCurrentMethod().DeclaringType, msg, args);
            }

            private static IEnumerator<YieldInstruction> SetCrosshairDelayed(HandReticle instance, bool visible, float delay, int requestId)
            {
                if (!visible && delay > 0)
                {
                    yield return new WaitForSeconds(delay);
                }
                if (requestId != currentRequestId)
                {
                    Log("Coroutine should have been stopped, yet here we are...");
                    yield break;
                }
                var icons = AccessTools.Field(typeof(HandReticle), nameof(HandReticle._icons)).GetValue(instance)
                        as Dictionary<HandReticle.IconType, uGUI_HandReticleIcon>;
                if (!visible)
                {
                    if (!icon)
                    {
                        icon = icons[HandReticle.IconType.Default];
                    }
                    icons.Remove(HandReticle.IconType.Default);
                }
                else if (icon)
                {
                    icons[HandReticle.IconType.Default] = icon;
                }
                icon.SetActive(visible, 0.1f);
            }

            private static void SetCrosshair(HandReticle instance, bool visible, float delay)
            {
                if (visible)
                {
                    if ((Player.main == null || Player.main.GetPDA().isInUse))
                    {
                        return;
                    }
                    if (HandReticle.main.CurrentIconType != HandReticle.IconType.Default)
                    {
                        // Fixes duplicate cursor in vehicle theme screen.
                        return;
                    }
                }
                if (delayedHandler != null)
                {
                    instance.StopCoroutine(delayedHandler);
                }
                delayedHandler = instance.StartCoroutine(SetCrosshairDelayed(instance, visible, delay, ++currentRequestId));
            }

            [HarmonyPatch(typeof(HandReticle))]
            [HarmonyPatch(nameof(HandReticle.Start))]
            private static class Patch_HandReticle_Start
            {
                private static void Postfix(HandReticle __instance)
                {
                    var blacklist = new List<Type>
                    {
                        typeof(uGUI_ItemIcon), // Icon in PDA/Fabricator/etc, nothing good will come of this.
                        typeof(uGUI_EquipmentSlot),
                    };

                    var types = AppDomain.CurrentDomain.GetAssemblies().ToList()
                        .FindAll(asm =>
                        {
                            // .Net 3.5 for IsDynamic: https://stackoverflow.com/a/1423747
                            if (asm.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                            {
                                return false;
                            }
                            return asm.Location.Contains("\\QMods\\") || asm.Location.Contains("\\Assembly-CSharp.dll");
                        })
                        .SelectMany(asm => asm.GetTypes()).ToList()
                        .FindAll(type => typeof(IPointerEnterHandler).IsAssignableFrom(type) && typeof(IPointerExitHandler).IsAssignableFrom(type))
                        .FindAll(type => !blacklist.Contains(type));

                    Log("Found {0} types to patch:\n{1}", types.Count(), String.Join("\n", types.Select(t => t.FullName).ToArray()));

                    types.ForEach(type =>
                    {
                        var enter = type.GetMethod("OnPointerEnter");
                        var exit = type.GetMethod("OnPointerExit");
                        var disable = type.GetMethod("OnDisable");

                        if (enter != null && exit != null)
                        {
                            harmony.Patch(enter, null, new HarmonyMethod(((Action<MonoBehaviour>)OnPointerEnterPostfix).Method), null);
                            harmony.Patch(exit, null, new HarmonyMethod(((Action<MonoBehaviour>)OnPointerExitPostfix).Method), null);
                        }
                        if (disable != null)
                        {
                            harmony.Patch(exit, null, new HarmonyMethod(((Action<MonoBehaviour>)OnDisablePostfix).Method), null);
                        }
                    });

                    updateStack();
                }

                private static void OnPointerEnterPostfix(MonoBehaviour __instance)
                {
                    updateStack(add: __instance);
                }

                private static void OnPointerExitPostfix(MonoBehaviour __instance)
                {
                    updateStack(remove: __instance);
                }

                private static void OnDisablePostfix(MonoBehaviour __instance)
                {
                    updateStack(remove: __instance);
                }
            }

            private static void updateStack(System.Object add = null, System.Object remove = null)
            {
                stack.Purge();
                if (add != null)
                {
                    if (IngameMenu.main?.gameObject.activeSelf ?? false)
                    {
                        return;
                    }
                    if (add is Component)
                    {
                        if (((Component)add).name == "FireSuppressionButton")
                        {
                            // This button is weird. It has it's own cursor but when hovering the IconType is still default.
                            // Maybe because it grows in size when you get close?
                            return;
                        }
                    }
                    if (stack.Count() == 0 || stack.Peek() != add)
                    {
                        // Don't add duplicates if we receive duplicate enter events.
                        stack.Push(add);
                    }
                }
                if (remove != null)
                {
                    stack.UnwindTo(remove);
                }

                lastStackUpdate = stack.ToString();
                //Msg("Stack: {0}", lastStackUpdate);
                // Don't introduce additional delay, scanner room screen already has a big collision box.
#pragma warning disable CS0253 // Yes, I do mean reference comparison.
                var delay = SCANNER_ROOM == remove ? 0 : 0.25f;
#pragma warning restore CS0253 // Yes, I do mean reference comparison.
                SetCrosshair(HandReticle.main, stack.Count() > 0, delay);
            }

            
            [HarmonyPatch(typeof(HandReticle))]
            [HarmonyPatch(nameof(HandReticle.LateUpdate))]
            private static class Patch_HandReticle_LateUpdate
            {
                private static void Postfix()
                {
                    if (Input.GetKeyDown(KeyCode.F7) && Input.GetKey(KeyCode.LeftAlt))
                    {
                        var list = stack.ToString();
                        Log("USER RESET CROSSHAIR.\nSTACK: {0}\nLAST: {1}\n", list, lastStackUpdate);
                        Msg("If you want to help fix this please PM me your player.log file.");
                        stack.Clear();
                        updateStack();
                    }
                }
            }

            private class PurgableStack<T>
            {
                private LinkedList<T> data = new LinkedList<T>();

                public void Push(T value)
                {
                    data.AddFirst(value);
                }

                public T Peek()
                {
                    return data.First();
                }

                public T Pop()
                {
                    var result = Peek();
                    data.RemoveFirst();
                    return result;
                }

                public bool Purge()
                {
                    var node = data.First;
                    while (node != null)
                    {
                        var remove = IsNull(node.Value);
                        if (!remove && node.Value is Behaviour)
                        {
                            var behaviour = node.Value as Behaviour;
                            remove |= !behaviour.enabled;
                            remove |= !behaviour.gameObject.activeSelf;
                        }
                        if (remove)
                        {
                            data.Remove(node);
                        }
                        node = node.Next;
                    }
                    return Count() == 0;
                }

                public int Count()
                {
                    return data.Count();
                }

                public void Clear()
                {
                    data.Clear();
                }

                public void UnwindTo(T value)
                {
                    var node = data.Find(value);
                    while (node != null)
                    {
                        data.Remove(node);
                        node = node.Previous;
                    }
                }

                public override String ToString()
                {
                    return String.Join(", ", data.Select(o => o.ToString()).ToArray());
                }
            }

            // FFS Unity: https://forum.unity.com/threads/fun-with-null.148090/
            private static bool IsNull(System.Object obj)
            {
                return obj == null || (obj is UnityEngine.Object && ((obj as UnityEngine.Object) == null));
            }

            //[HarmonyPatch(typeof(Player))]
            //[HarmonyPatch("EnterPilotingMode")]
            //private static class Patch_Player_EnterPilotingMode
            //{
            //    private static void Postfix(Player __instance)
            //    {
            //        SetCrosshair(HandReticle.main, true);
            //    }
            //}

            //[HarmonyPatch(typeof(Player))]
            //[HarmonyPatch("ExitPilotingMode")]
            //private static class Patch_Player_ExitPilotingMode
            //{
            //    private static void Postfix(Player __instance)
            //    {
            //        SetCrosshair(HandReticle.main, false);
            //    }
            //}

            [HarmonyPatch(typeof(uGUI_MapRoomScanner))]
            [HarmonyPatch(nameof(uGUI_MapRoomScanner.OnTriggerEnter))]
            private static class Patch_uGUI_MapRoomScanner_OnTriggerEnter
            {
                private static void Postfix(uGUI_MapRoomScanner __instance)
                {
                    updateStack(add: SCANNER_ROOM);

                    //Dump(__instance.gameObject, Log);
                }
            }

            [HarmonyPatch(typeof(uGUI_MapRoomScanner))]
            [HarmonyPatch(nameof(uGUI_MapRoomScanner.OnTriggerExit))]
            private static class Patch_uGUI_MapRoomScanner_OnTriggerExit
            {
                private static void Postfix()
                {
                    stack.Clear();
                    updateStack();
                }
            }

            [HarmonyPatch(typeof(Player))]
            [HarmonyPatch(nameof(Player.Awake))]
            private static class Patch_Player_Awake
            {
                private static void Postfix()
                {
                    // TODO: Further investigate problems with loading games.
                    updateStack();
                }
            }
        }

        private static class BasePowerPriority
        {
            private static void Log(String msg, params object[] args)
            {
                Entry.Log(MethodBase.GetCurrentMethod().DeclaringType, msg, args);
            }

            /*private static void DumpPowerTree(StringBuilder sb, String indent, PowerRelay relay)
            {
                var inboundPowerSources = ReflGet<List<IPowerInterface>>(relay, "inboundPowerSources");
                sb.Append(String.Format("{0}{1} {2} {3}[{4}]\n", indent, relay.GetType(), GetPriority(relay), relay, relay.gameObject.GetInstanceID()));
                if (relay is BasePowerRelay)
                {
                    sb.Append(String.Format("{0}  SubRootId: {1}\n", indent, ((BasePowerRelay)relay).subRoot.gameObject.GetInstanceID()));
                }

                indent += "\t";
                inboundPowerSources.ForEach(s =>
                {
                    if (s is PowerRelay)
                    {
                        DumpPowerTree(sb, indent, (PowerRelay)s);
                    } else
                    {
                        sb.Append(String.Format("{0}Not a PowerRelay: {1} {2} {3}\n", indent, s.GetType(), GetPriority(s), s));
                    }
                });
            }*/

            private static int GetPriority(IPowerInterface powerInterface)
            {
                if (powerInterface is MonoBehaviour)
                {
                    var obj = ((MonoBehaviour)powerInterface).gameObject;
                    if (obj.GetComponent<BaseNuclearReactor>())
                    {
                        return 4;
                    }
                    if (obj.GetComponent<BaseBioReactor>())
                    {
                        return 3;
                    }
                    if (obj.GetComponent<ThermalPlant>())
                    {
                        return 2;
                    }
                    if (obj.GetComponent<SolarPanel>())
                    {
                        return 1;
                    }
                    return 0;
                }
                return Int32.MaxValue;
            }

            [HarmonyPatch(typeof(PowerRelay))]
            [HarmonyPatch(nameof(PowerRelay.AddInboundPower))]
            private static class Patch_PowerRelay_AddInboundPower
            {
                private static void Postfix(PowerRelay __instance, IPowerInterface powerInterface)
                {
                    var inboundPowerSources = ReflGet<List<IPowerInterface>>(__instance, "inboundPowerSources");
                    inboundPowerSources.Sort((a, b) =>
                    {
                        return GetPriority(a).CompareTo(GetPriority(b));
                    });
                }
            }
        }

        private static class BeaconsStartOff
        {
            [HarmonyPatch(typeof(PingManager))]
            [HarmonyPatch(nameof(PingManager.Register))]
            private static class Patch_PingManager_Register
            {
                private static void Postfix(PingInstance instance)
                {
                    Log("{0}", "Patch_PingManager_Register");
                    instance.SetVisible(false);
                }
            }
        }

        private static class NoRotateQuickSlots
        {
            private static void Log(String msg, params object[] args)
            {
                Entry.Log(MethodBase.GetCurrentMethod().DeclaringType, msg, args);
            }

            [HarmonyPatch(typeof(QuickSlots))]
            [HarmonyPatch(nameof(QuickSlots.Assign))]
            private static class QuickSlotsAssignPatch
            {
                private static bool Prefix(QuickSlots __instance, InventoryItem item)
                {
                    var slot = __instance.GetSlotByItem(item);
                    if (slot < 0)
                    {
                        __instance.Bind(0, item);
                    }
                    return false;
                }
            }
        }

        private static class FasterDrilling
        {
            private static readonly float speedUpFactor = 4f;

            private static void Log(String msg, params object[] args)
            {
                Entry.Log(MethodBase.GetCurrentMethod().DeclaringType, msg, args);
            }

            [HarmonyPatch(typeof(Drillable))]
            [HarmonyPatch(nameof(Drillable.Start))]
            private static class Patch_Drillable_Start
            {
                private static void Postfix(Drillable __instance)
                {
                    __instance.health = __instance.health.Select(r => r / speedUpFactor).ToArray();
                }
            }

            [HarmonyPatch(typeof(Drillable))]
            [HarmonyPatch(nameof(Drillable.Restore))]
            private static class Patch_Drillable_Restore
            {
                private static void Postfix(Drillable __instance)
                {
                    __instance.health = __instance.health.Select(r => r / speedUpFactor).ToArray();
                }
            }

        }
    }
}
