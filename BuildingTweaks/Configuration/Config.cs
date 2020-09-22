using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BuildingTweaks.Configuration
{
    [Menu("BuildingTweaks")]
    public class Config: ConfigFile
    {

        internal bool AttachToTarget = false;
        internal bool FullOverride = false;

        [Keybind("Attach to Target Toggle Key")]
        public KeyCode AttachToTargetToggle = KeyCode.T;

        [Keybind("Full Override Toggle Key")]
        public KeyCode FullOverrideToggle = KeyCode.G;

    }
}
