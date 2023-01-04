namespace BuildingTweaks.Configuration
{
    using SMLHelper.Json;
    using SMLHelper.Options.Attributes;
    using UnityEngine;

    [Menu("BuildingTweaks")]
    public class SMLConfig: ConfigFile
    {
        public bool AttachToTarget = false;
        public bool FullOverride = false;

        [Keybind("Attach to Target Toggle Key")]
        public KeyCode AttachToTargetToggle = KeyCode.T;

        [Keybind("Full Override Toggle Key")]
        public KeyCode FullOverrideToggle = KeyCode.G;

    }
}
