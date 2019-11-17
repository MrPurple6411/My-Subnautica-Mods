using System.Reflection;
using Harmony;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace ResourceOverload
{
    public class Entry
    {
        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("MrPurple6411.ResourceOverload");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Config.Load();
            OptionsPanelHandler.RegisterModOptions(new Options());
        }
    }
    public static class Config
    {
        public static float SliderValue;
        public static bool ToggleValue;

        public static void Load()
        {
            SliderValue = PlayerPrefs.GetFloat("ResourceMultiplier", 10f);
            ToggleValue = PlayerPrefsExtra.GetBool("Enabled", true);
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("Resource Overload")
        {
            SliderChanged += Options_SliderChanged;
            ToggleChanged += Options_ToggleChanged;
        }

        public void Options_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if (e.Id != "Multiplier") return;
            Config.SliderValue = e.Value;
            PlayerPrefs.SetFloat("ResourceMultiplier", e.Value);
        }
        public void Options_ToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            if (e.Id != "onOff") return;
            Config.ToggleValue = e.Value;
            PlayerPrefsExtra.SetBool("Enabled", e.Value);
        }

        public override void BuildModOptions()
        {
            AddSliderOption("Multiplier", "Resource Multiplier", 0, 20, Config.SliderValue);
            AddToggleOption("onOff", "Enabled", Config.ToggleValue);
        }
    }
}
