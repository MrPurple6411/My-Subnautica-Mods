using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScannerRoomUpgrades.Patches
{
    [HarmonyPatch(typeof(), nameof())]
    public static class Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {

        }
    }
}
