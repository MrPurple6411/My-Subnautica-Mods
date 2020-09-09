using HarmonyLib;
using RandomCreatureSize.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomCreatureSize.Patches
{
	[HarmonyPatch(typeof(Creature), "Start")]
	internal static class Creature_Start_Patch
	{

		public static void Prefix(Creature __instance)
		{
			if(Main.CreatureConfig is null)
			{
				Main.CreatureConfig = new CreatureConfig();
				Main.CreatureConfig.Load();
			}

			if ((!__instance.gameObject.GetComponent<WaterParkCreature>()?.IsInsideWaterPark() ?? true) && !Main.CreatureConfig.CreatureSizes.ContainsKey(__instance.GetComponent<PrefabIdentifier>().Id))
			{
				UnityEngine.Random.InitState(DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond);
				float scale = UnityEngine.Random.Range(Main.Config.minsize, Main.Config.maxsize);
				__instance.GetComponent<Creature>().SetScale(scale);
				Main.CreatureConfig.CreatureSizes.Add(__instance.GetComponent<PrefabIdentifier>().Id, scale);
				Main.CreatureConfig.Save();
			}
		}
	}

}
