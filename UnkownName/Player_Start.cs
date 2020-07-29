using System.IO;
using HarmonyLib;
#if SUBNAUTICA
using Oculus.Newtonsoft.Json;
using Oculus.Newtonsoft.Json.Linq;
#elif BELOWZERO
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace UnKnownName
{
    [HarmonyPatch(typeof(Player), nameof(Player.Start))]
    public class Player_Start
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            Main.config.Load();
        }
    }

}