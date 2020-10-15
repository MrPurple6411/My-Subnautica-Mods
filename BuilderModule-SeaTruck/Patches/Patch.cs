using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuilderModule_SeaTruck.Patches
{
    [HarmonyPatch(typeof(Main), nameof(Main.Load))]
    public static class Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {

        }
    }
}
