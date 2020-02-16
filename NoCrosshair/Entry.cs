using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Harmony;
using System.Reflection;
using System.IO;
using QModManager.API.ModLoading;

/**
 * Sorry, too lazy to do separate builds. Mods are activated based on file name.
 */
namespace frigidpenguin
{

    [QModCore]
    public class Entry
    {
        private static void Log(String msg, params object[] args)
        {
            UnityEngine.Debug.Log(String.Format("[frigidpenguin] " + msg, args));
        }

        private static void Log(Type type, String msg, params object[] args)
        {
            UnityEngine.Debug.Log(String.Format("[frigidpenguin:" + type.Name + "]" + msg, args));
        }

        private static void Msg(String msg, params object[] args)
        {
            ErrorMessage.AddMessage(String.Format(msg, args));
        }

        [QModPatch]
        public static void Start()
        {
            Log("Yoo wee are inside!!");

            var asm = Assembly.GetExecutingAssembly();
            var name = Path.GetFileName(Assembly.GetExecutingAssembly().Location)
                    .Replace(".dll", "");
            Log("DLL Name: {0}", name);

            var harmony = HarmonyInstance.Create("frigidpenguin." + name);
            if (name == "dev")
            {
                harmony.PatchAll(asm);
            } else
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
            [HarmonyPatch("Start")]
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
            [HarmonyPatch("OnPlayerExited")]
            private static class Patch_SubRoot_OnPlayerExited
            {
                private static void Postfix(SubRoot __instance)
                {
                    if (!__instance.gameObject.name.StartsWith("Cyclops-MainPrefab"))
                    {
                        return;
                    }
                    Msg("OnPlayerExited");
                    __instance.transform.Find("Floodlights")
                            .GetAllComponentsInChildren<VFXVolumetricLight>()
                            .ForEach(x => x.RestoreVolume());
                }
            }

            [HarmonyPatch(typeof(SubRoot))]
            [HarmonyPatch("OnPlayerEntered")]
            private static class Patch_SubRoot_OnPlayerEntered
            {
                private static void Postfix(SubRoot __instance)
                {
                    if (!__instance.gameObject.name.StartsWith("Cyclops-MainPrefab"))
                    {
                        return;
                    }
                    Msg("OnPlayerEntered");
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
            [HarmonyPatch("OnAltDown")]
            private static class PlayerToolOnAltDownPatch
            {
                private static void Postfix(Seaglide __instance)
                {
                    __instance.toggleLights.lightState = __instance.toggleLights.lightState == 2 ? 0 : 2;
                }
            }

            [HarmonyPatch(typeof(Seaglide))]
            [HarmonyPatch("Update")]
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
            [HarmonyPatch("Start")]
            private static class SeaglideStartPatch
            {
                private static void Postfix(Seaglide __instance)
                {
                    // Start with map off.
                    __instance.toggleLights.lightState = 2;
                }
            }

            [HarmonyPatch(typeof(ToggleLights))]
            [HarmonyPatch("OnPoweredChanged")]
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
            private static readonly ToggleLights templateToggleLights = LightVolumetrizer.template.toggleLights;
            private static readonly MethodInfo MapRoomCamera_IsControlled = AccessTools.Method(typeof(MapRoomCamera), "IsControlled");
            private static readonly MethodInfo ToggleLights_OnPoweredChanged = AccessTools.Method(typeof(ToggleLights), "OnPoweredChanged");
            private static readonly EventInfo EnergyMixin_onPoweredChanged = typeof(EnergyMixin).GetEvent("onPoweredChanged");

            private static void Log(String msg, params object[] args)
            {
                Entry.Log(MethodBase.GetCurrentMethod().DeclaringType, msg, args);
            }

            [HarmonyPatch(typeof(MapRoomCamera))]
            [HarmonyPatch("Update")]
            private static class MapRoomCameraUpdatePatch
            {
                private static void Postfix(MapRoomCamera __instance)
                {
                    var toggleLights = __instance.GetComponent<ToggleLights>();
                    if (toggleLights == null)
                    {
                        return;
                    }
                    toggleLights.UpdateLightEnergy();
                    var active = (bool)MapRoomCamera_IsControlled.Invoke(__instance, null);
                    if (active && GameInput.GetButtonDown(GameInput.Button.RightHand))
                    {
                        toggleLights.SetLightsActive(!toggleLights.lightsActive);
                    }
                }
            }

            [HarmonyPatch(typeof(MapRoomCamera))]
            [HarmonyPatch("ControlCamera")]
            private static class MapRoomCameraControlCameraPatch
            {
                private static void Prefix(MapRoomCamera __instance, ref bool __state)
                {
                    __state = __instance.lightsParent.activeSelf;
                }

                private static void Postfix(MapRoomCamera __instance, bool __state)
                {
                    __instance.lightsParent.SetActive(__state);

                    var toggleLights = __instance.GetComponent<ToggleLights>();
                    if (toggleLights == null)
                    {
                        return;
                    }
                    toggleLights.lightsOnSound = templateToggleLights.lightsOnSound;
                    toggleLights.lightsOffSound = templateToggleLights.lightsOffSound;

                    __instance.GetAllComponentsInChildren<VFXVolumetricLight>()
                        .ForEach(x => x.DisableVolume());
                }
            }

            [HarmonyPatch(typeof(MapRoomCamera))]
            [HarmonyPatch("FreeCamera")]
            private static class MapRoomCameraFreeCameraPatch
            {
                private static void Prefix(MapRoomCamera __instance, ref bool __state)
                {
                    __state = __instance.lightsParent.activeSelf;
                }

                private static void Postfix(MapRoomCamera __instance, bool __state)
                {
                    __instance.lightsParent.SetActive(__state);

                    var toggleLights = __instance.GetComponent<ToggleLights>();
                    if (toggleLights == null)
                    {
                        return;
                    }
                    toggleLights.lightsOnSound = null;
                    toggleLights.lightsOffSound = null;

                    __instance.GetAllComponentsInChildren<VFXVolumetricLight>()
                        .ForEach(x => x.RestoreVolume());
                }
            }

            [HarmonyPatch(typeof(MapRoomCamera))]
            [HarmonyPatch("SetDocked")]
            private static class MapRoomCameraSetDockedPatch
            {
                private static void Postfix(MapRoomCamera __instance, MapRoomCameraDocking dockingPoint)
                {
                    if (dockingPoint)
                    {
                        var toggleLights = __instance.GetComponent<ToggleLights>();
                        toggleLights.SetLightsActive(false);
                    }                    
                }
            }


            [HarmonyPatch(typeof(MapRoomCamera))]
            [HarmonyPatch("OnPickedUp")]
            private static class MapRoomCameraOnPickedUpPatch
            {
                private static void Postfix(MapRoomCamera __instance)
                {
                    var toggleLights = __instance.GetComponent<ToggleLights>();
                    toggleLights.SetLightsActive(false);

                    toggleLights.onSound = null;
                    toggleLights.offSound = null;
                }
            }

            [HarmonyPatch(typeof(ToggleLights))]
            [HarmonyPatch("OnPoweredChanged")]
            private static class ToggleLightsOnPoweredChangedPatch
            {
                private static bool Prefix(ToggleLights __instance, bool powered)
                {
                    if (__instance.gameObject.GetComponent<MapRoomCamera>())
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

            [HarmonyPatch(typeof(MapRoomCamera))]
            [HarmonyPatch("Start")]
            private static class MapRoomCameraStartPatch
            {
                private static void OnDropped(MapRoomCamera camera)
                {
                    var toggleLights = camera.GetComponent<ToggleLights>();
                    toggleLights.onSound = templateToggleLights.onSound;
                    toggleLights.offSound = templateToggleLights.offSound;
                }

                private static void Postfix(MapRoomCamera __instance)
                {
                    __instance.pickupAble.droppedEvent.AddHandler(__instance.gameObject, x => OnDropped(__instance));

                    // Setup ToggleLights:
                    var toggleLights = __instance.GetComponent<ToggleLights>();
                    if (!__instance.pickupAble.attached)
                    {
                        OnDropped(__instance);
                    }

                    toggleLights.lightsParent = __instance.transform.Find("lights_parent").gameObject;
                    toggleLights.energyMixin = __instance.GetComponent<EnergyMixin>();
                    Log("EnergyPerSecond Before: {0}", toggleLights.energyPerSecond);
                    SetNumberOfDaysEnergyLasts(toggleLights, 100, 2);
                    Log("EnergyPerSecond: {0}", toggleLights.energyPerSecond);

                    Log("ToggleLights component added.");

                    foreach (var light in toggleLights.lightsParent.GetComponentsInChildren<Light>())
                    {
                        light.intensity *= 0.5f;
                        light.range *= 0.5f;
                        light.spotAngle *= 0.5f;
                    }

                    LightVolumetrizer.VolumetrizeLights(__instance.gameObject.transform.Find("lights_parent/light_top").gameObject, 0.85f, Vector3.zero);
                    LightVolumetrizer.VolumetrizeLights(__instance.gameObject.transform.Find("lights_parent/light_bottom").gameObject, 0.85f, Vector3.zero);
                }
            }
        }

        private static class PrawnsuitVolumetricLights
        {
            private static void Log(String msg, params object[] args)
            {
                Entry.Log(MethodBase.GetCurrentMethod().DeclaringType, msg, args);
            }

            [HarmonyPatch(typeof(Exosuit))]
            [HarmonyPatch("Start")]
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
            [HarmonyPatch("OnPilotModeBegin")]
            private static class Patch_Exosuit_OnPilotModeBegin
            {
                private static void Postfix(Exosuit __instance)
                {
                    __instance.GetAllComponentsInChildren<VFXVolumetricLight>()
                                .ForEach(x => x.DisableVolume());
                }
            }

            [HarmonyPatch(typeof(Exosuit))]
            [HarmonyPatch("OnPilotModeEnd")]
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
            [HarmonyPatch("Start")]
            private static class Patch_Exosuit_Start
            {
                private static void Postfix(Exosuit __instance)
                {
                    var toggleLights = __instance.GetComponent<ToggleLights>();
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
            [HarmonyPatch("Update")]
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
            [HarmonyPatch("Start")]
            private static class Patch_SeaMoth_Start
            {
                private static void Postfix(SeaMoth __instance)
                {
                    var toggleLights = __instance.GetComponentInChildren<ToggleLights>();
                    Log("toggleLights: {0}", toggleLights != null);
                    Log("EnergyPerSecond Before: {0}", toggleLights.energyPerSecond);
                    SetNumberOfDaysEnergyLasts(toggleLights, 200, 1);
                    Log("EnergyPerSecond: {0}", toggleLights.energyPerSecond);

                    toggleLights.SetLightsActive(false);
                }
            }

            [HarmonyPatch(typeof(ToggleLights))]
            [HarmonyPatch("OnPoweredChanged")]
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
            private static readonly MethodInfo HandReticle_SetIconInternal = AccessTools.Method(typeof(HandReticle), "SetIconInternal");
            private static uGUI_HandReticleIcon icon;            

            private static void Log(String msg, params object[] args)
            {
                Entry.Log(MethodBase.GetCurrentMethod().DeclaringType, msg, args);
            }

            private static void SetCrosshair(HandReticle instance, bool visible) {
                var icons = AccessTools.Field(typeof(HandReticle), "_icons").GetValue(instance)
                        as Dictionary<HandReticle.IconType, uGUI_HandReticleIcon>;
                if (!visible)
                {
                    if (!icon)
                    {
                        icon = icons[HandReticle.IconType.Default];
                    }
                    icons.Remove(HandReticle.IconType.Default);
                } else if (icon)
                {
                    icons[HandReticle.IconType.Default] = icon;
                }
                icon.SetActive(visible, 0.1f);
            }

            [HarmonyPatch(typeof(HandReticle))]
            [HarmonyPatch("Awake")]
            private static class Patch_HandReticle_Awake
            {
                private static void Postfix(HandReticle __instance)
                {
                    SetCrosshair(__instance, false);
                }
            }

            [HarmonyPatch(typeof(Player))]
            [HarmonyPatch("EnterPilotingMode")]
            private static class Patch_Player_EnterPilotingMode
            {
                private static void Postfix(Player __instance)
                {
                    SetCrosshair(HandReticle.main, true);
                }
            }

            [HarmonyPatch(typeof(Player))]
            [HarmonyPatch("ExitPilotingMode")]
            private static class Patch_Player_ExitPilotingMode
            {
                private static void Postfix(Player __instance)
                {
                    SetCrosshair(HandReticle.main, false);
                }
            }

            [HarmonyPatch(typeof(uGUI_MapRoomScanner))]
            [HarmonyPatch("OnTriggerEnter")]
            private static class Patch_uGUI_MapRoomScanner_OnTriggerEnter
            {
                private static void Postfix()
                {
                    SetCrosshair(HandReticle.main, true);
                }
            }

            [HarmonyPatch(typeof(uGUI_MapRoomScanner))]
            [HarmonyPatch("OnTriggerExit")]
            private static class Patch_uGUI_MapRoomScanner_OnTriggerExit
            {
                private static void Postfix()
                {
                    SetCrosshair(HandReticle.main, false);
                }
            }           
        }
    }
}
