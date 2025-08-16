namespace NoEatingSounds.Patches;

using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#if BELOWZERO
[HarmonyPatch(typeof(TechData), nameof(TechData.GetSoundType))]
internal class TechData_GetSoundType_Patch
{
	[HarmonyPostfix]
	internal static void Postfix(ref TechData.SoundType __result)
	{
		// Silence eating/drinking sounds by remapping them to Default
		if (__result == TechData.SoundType.Food)
			__result = TechData.SoundType.Default;
		if (__result == TechData.SoundType.FilteredWater || __result == TechData.SoundType.DisinfectedWater || __result == TechData.SoundType.BigWaterBottle)
			__result = TechData.SoundType.Default;
	}
}
#elif SUBNAUTICA
// SN2025 plays use/eat sounds via Survival.Eat -> FMODUWE.PlayOneShot(TechData.GetSoundUse(...)).
// We strip that PlayOneShot call using a transpiler so only eating/drinking sounds are silenced.
[HarmonyPatch(typeof(Survival), nameof(Survival.Eat))]
internal class Survival_Eat_Patch
{
	[HarmonyTranspiler]
	internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		var list = new List<CodeInstruction>(instructions);
		for (int i = 0; i < list.Count; i++)
		{
			var instr = list[i];
			if ((instr.opcode == OpCodes.Call || instr.opcode == OpCodes.Callvirt) && instr.operand is MethodInfo mi)
			{
				// Match the exact PlayOneShot(string, Vector3, float) signature
				if (mi.DeclaringType != null && mi.DeclaringType.Name == "FMODUWE" && mi.Name == "PlayOneShot")
				{
					var pars = mi.GetParameters();
					if (pars.Length == 3 && pars[0].ParameterType == typeof(string) && pars[1].ParameterType == typeof(UnityEngine.Vector3) && pars[2].ParameterType == typeof(float))
					{
						// Replace the call with pops for its three arguments
						list[i] = new CodeInstruction(OpCodes.Pop); // pop float
						list.Insert(i + 1, new CodeInstruction(OpCodes.Pop)); // pop Vector3
						list.Insert(i + 2, new CodeInstruction(OpCodes.Pop)); // pop string
					}
				}
			}
		}
		return list;
	}
}
#endif