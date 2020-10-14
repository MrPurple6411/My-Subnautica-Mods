using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoMask.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.SetScubaMaskActive))]
    public static class Player_SetScubaMaskActive_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(ref bool state)
        {
            state = false;
        }
    }
}
