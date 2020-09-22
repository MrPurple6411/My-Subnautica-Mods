using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartWithCheats.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    public static class Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {

        }
    }
}
