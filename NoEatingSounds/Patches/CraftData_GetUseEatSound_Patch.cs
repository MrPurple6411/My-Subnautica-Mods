namespace NoEatingSounds.Patches;

using HarmonyLib;

#if SUBNAUTICA
[HarmonyPatch(typeof(CraftData), nameof(CraftData.GetUseEatSound))]
internal class Data_GetUseEatSound_Patch
{
	[HarmonyPostfix]
	internal static void Postfix(ref string __result)
	{
		__result = "";
	}
}
#elif BELOWZERO
[HarmonyPatch(typeof(TechData), nameof(TechData.GetSoundType))]
internal class Data_GetSoundType_Patch
{
	[HarmonyPostfix]
	internal static void Postfix(ref TechData.SoundType __result)
	{
		if (__result == TechData.SoundType.Food)
			__result = TechData.SoundType.Default;
		if (__result == TechData.SoundType.FilteredWater || __result == TechData.SoundType.DisinfectedWater || __result == TechData.SoundType.BigWaterBottle)
			__result = TechData.SoundType.Default;
	}
}
#endif