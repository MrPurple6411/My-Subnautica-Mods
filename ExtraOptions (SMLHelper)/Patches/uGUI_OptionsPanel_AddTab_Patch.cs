using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using Logger = QModManager.Utility.Logger;
#if SUBNAUTICA_STABLE
using Oculus.Newtonsoft.Json;
#else
using Newtonsoft.Json;
#endif

namespace ExtraOptions.Patches
{
    [HarmonyPatch(typeof(uGUI_OptionsPanel), nameof(uGUI_OptionsPanel.AddTab))]
    class uGUI_OptionsPanel_AddTab_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(uGUI_OptionsPanel __instance, int __result, string label)
        {
            if(label == "Mods")
            {
                try
                {
                    WaterBiomeManager.BiomeSettings biome = Main.GetBiome();
                    if (biome != null)
                    {
                        var t = __instance;
                        var idx = __result;
                        var name = biome.name ?? "???";
                        t.AddHeading(idx, $"Biome Config ({name})");
                        Dictionary<string, WaterscapeVolume.Settings> themes;

                        try
                        {
                            if (File.Exists(Main.themesPath))
                                themes = JsonConvert.DeserializeObject<Dictionary<string, WaterscapeVolume.Settings>>(File.ReadAllText(Main.themesPath), Main.themeJSS);
                            else
                                themes = new Dictionary<string, WaterscapeVolume.Settings>();
                        }
                        catch (Exception)
                        {
                            themes = new Dictionary<string, WaterscapeVolume.Settings>();
                        }

                        if (themes.ContainsKey(name))
                            biome.settings = themes[name];
                        else
                            themes[name] = biome.settings;

                        Vector("absorption", 0, 200);
                        Range("scattering", 0, 2);
                        Color("scatteringColor");
                        Range("murkiness", 0.01568f, 1.0f);
                        Color("emissive");
                        Range("emissiveScale", 0, 1);
                        Range("startDistance", 0, 100);
                        Range("sunlightScale", 0, 1);
                        Range("ambientScale", 0, 1);

                        void Apply()
                        {
                            Logger.Log(Logger.Level.Debug, "Reloading");
                            Main.ApplyOptions();
                            Logger.Log(Logger.Level.Debug, "Reloaded");
                            var json = JsonConvert.SerializeObject(themes
                                                                  , Formatting.Indented
                                                                  , Main.themeJSS);
                            Logger.Log(Logger.Level.Debug, "serialized");
                            File.WriteAllText(Main.themesPath, json);
                            Logger.Log(Logger.Level.Debug, "Written");
                        }

                        void AddVectorRawOption<T>(string fieldName, float min, float max, Func<T, Vector3> from, Func<Vector3, T> to)
                        {
                            float scale = 255f / (max - min);
                            float nmin = min * scale;
                            float nmax = max * scale;
                            var fld = Traverse.Create(biome.settings).Field(fieldName);
                            Func<Vector3> get = () => from(fld.GetValue<T>()) * scale;
                            Action<Vector3> set = (iv) => fld.SetValue(to(iv / scale));
#if SN1
                            t.AddSliderOption(idx, $"{fieldName}.x/r", get().x, nmin, nmax, get().x, new UnityAction<float>(v => { set(new Vector3(v, get().y, get().z)); Apply(); }));
                            t.AddSliderOption(idx, $"{fieldName}.y/g", get().y, nmin, nmax, get().y, new UnityAction<float>(v => { set(new Vector3(get().x, v, get().z)); Apply(); }));
                            t.AddSliderOption(idx, $"{fieldName}.z/b", get().z, nmin, nmax, get().z, new UnityAction<float>(v => { set(new Vector3(get().x, get().y, v)); Apply(); }));
#elif BELOWZERO_STABLE
                            t.AddSliderOption(idx, $"{fieldName}.x/r", get().x, nmin, nmax, get().x, Mathf.Clamp(nmin / nmax, 0.01f, 100f), new UnityAction<float>(v => { set(new Vector3(v, get().y, get().z)); Apply(); }));
                            t.AddSliderOption(idx, $"{fieldName}.y/g", get().y, nmin, nmax, get().y, Mathf.Clamp(nmin / nmax, 0.01f, 100f), new UnityAction<float>(v => { set(new Vector3(get().x, v, get().z)); Apply(); }));
                            t.AddSliderOption(idx, $"{fieldName}.z/b", get().z, nmin, nmax, get().z, Mathf.Clamp(nmin / nmax, 0.01f, 100f), new UnityAction<float>(v => { set(new Vector3(get().x, get().y, v)); Apply(); }));
#elif BELOWZERO_EXP
                            t.AddSliderOption(idx, $"{fieldName}.x/r", get().x, nmin, nmax, get().x, Mathf.Clamp(nmin / nmax, 0.01f, 100f), new UnityAction<float>(v => { set(new Vector3(v, get().y, get().z)); Apply(); }), SliderLabelMode.Default, "0.0");
                            t.AddSliderOption(idx, $"{fieldName}.y/g", get().y, nmin, nmax, get().y, Mathf.Clamp(nmin / nmax, 0.01f, 100f), new UnityAction<float>(v => { set(new Vector3(get().x, v, get().z)); Apply(); }), SliderLabelMode.Default, "0.0");
                            t.AddSliderOption(idx, $"{fieldName}.z/b", get().z, nmin, nmax, get().z, Mathf.Clamp(nmin / nmax, 0.01f, 100f), new UnityAction<float>(v => { set(new Vector3(get().x, get().y, v)); Apply(); }), SliderLabelMode.Default, "0.0");
#endif
                        }

                        void Vector(string fieldName, float min, float max)
                        {
                            AddVectorRawOption(fieldName, min, max, x => x, y => y);
                        }

                        void Color(string fieldName)
                        {
                            AddVectorRawOption<Color>(fieldName, 0, 1, xx => new Vector3(xx.r, xx.g, xx.b), yy => new Color(yy.x, yy.y, yy.z));
                        }

                        void Range(string fieldName, float min, float max)
                        {
                            float scale = 255f / (max - min);
                            float nmin = min * scale;
                            float nmax = max * scale;
                            var fld = Traverse.Create(biome.settings).Field(fieldName);
                            Func<float> get = () => fld.GetValue<float>() * scale;
                            Action<float> set = (iv) => fld.SetValue(iv / scale);
#if SN1
                            t.AddSliderOption(idx, $"{fieldName}", get(), nmin, nmax, get(), new UnityAction<float>(v => { set(v); Apply(); }));
#elif BELOWZERO_STABLE
                            t.AddSliderOption(idx, $"{fieldName}", get(), nmin, nmax, get(), Mathf.Clamp(nmin / nmax, 0.01f, 100f), new UnityAction<float>(v => { set(v); Apply(); }));
#elif BELOWZERO_EXP
                            t.AddSliderOption(idx, $"{fieldName}", get(), nmin, nmax, get(), Mathf.Clamp(nmin / nmax, 0.01f, 100f), new UnityAction<float>(v => { set(v); Apply(); }), SliderLabelMode.Default, "0.0");
#endif
                        }

                    }
                }
                catch (Exception e)
                {
                    Logger.Log(Logger.Level.Error, "Failed to add Biome Options Panel", e);
                }
            }
        }

    }
}
