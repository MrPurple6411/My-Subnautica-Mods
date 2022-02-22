namespace Base_Deconstruct_Fix
{
    using BepInEx;
    using HarmonyLib;

    public class Main:BaseUnityPlugin
    {
        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(Patches.BaseDeconstructable_Patches), PluginInfo.PLUGIN_GUID);
        }
    }
}