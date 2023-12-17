namespace AllowEatingWhenFull.Patches;

using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(Survival), nameof(Survival.Eat))]
public class Patches
{
	[HarmonyPostfix]
	public static void Postfix(ref Survival __instance, GameObject useObj)
	{
		Eatable component = useObj.GetComponent<Eatable>();
		if (component == null)
			return;

		if (__instance.food <= 99f)
			return;

		if (component.GetFoodValue() != 0f)
		{
			__instance.food = Mathf.Clamp(__instance.food + component.GetFoodValue(), 0f, 200f);
		}
	}
}