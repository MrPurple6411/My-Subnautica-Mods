using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingTweaks.Patches
{
    [HarmonyPatch(typeof(Base), "BuildPillars")]
    public static class Base_BuildPillars_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }
}
