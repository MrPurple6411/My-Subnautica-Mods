using Harmony;
using QModManager.API.ModLoading;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BuilderPlaceOnComplete
{
	[QModCore]
	public class Main
	{

		[QModPatch]
		public void Load()
		{
			HarmonyInstance.Create("MrPurple6411.BuilderPlaceOnComplete").PatchAll(Assembly.GetExecutingAssembly());
		}
	}

	[HarmonyPatch(typeof(Builder), nameof(Builder.TryPlace))]
	public class Builder_TryPlace
	{
		[HarmonyPrefix]
		public static void Prefix()
		{
			Constructable_Construct.prefab = Builder.prefab;
		}

		[HarmonyPostfix]
		public static void Postfix(bool __result)
		{
			if (!__result)
			{
				Constructable_Construct.prefab = null;
			}
		}
	}

	[HarmonyPatch(typeof(Constructable), nameof(Constructable.Construct))]
	public class Constructable_Construct
	{
		public static GameObject prefab;

		[HarmonyPostfix]
		public static void Postfix(Constructable __instance)
		{
			if(__instance.constructedAmount >= 1f)
			{
				Builder.Begin(prefab);
			}
		}
	}
}
