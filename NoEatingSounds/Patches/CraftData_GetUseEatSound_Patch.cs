namespace NoEatingSounds.Patches;

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using System.Diagnostics;

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
internal static class NoEatingSoundsDiag
{
	// Toggle to re-enable detailed Info logging quickly.
	internal const bool Verbose = false;
}
// SN2025 plays use/eat sounds via Survival.Eat -> FMODUWE.PlayOneShot(TechData.GetSoundUse(...)).
// We strip that PlayOneShot call using a transpiler so only eating/drinking sounds are silenced.
[HarmonyPatch(typeof(Survival), nameof(Survival.Eat))]
internal class Survival_Eat_Patch
{
	[HarmonyTranspiler]
	internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		return CommonTranspiler.RemoveFMODPlayOneShot(instructions, nameof(Survival.Eat));
	}
}

// Some consumables (e.g. medkits, cure items) or future changes may trigger sounds through Survival.Use.
// We also strip PlayOneShot here to cover edge cases (user reported sound still playing after Eat patch logged success).
[HarmonyPatch(typeof(Survival), nameof(Survival.Use))]
internal class Survival_Use_Patch
{
	[HarmonyTranspiler]
	internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		return CommonTranspiler.RemoveFMODPlayOneShot(instructions, nameof(Survival.Use));
	}
}

internal static class CommonTranspiler
{
	internal static IEnumerable<CodeInstruction> RemoveFMODPlayOneShot(IEnumerable<CodeInstruction> instructions, string methodContext)
	{
		var list = new List<CodeInstruction>(instructions);
		int removedCalls = 0;
		for (int i = 0; i < list.Count; i++)
		{
			var instr = list[i];
			if ((instr.opcode == OpCodes.Call || instr.opcode == OpCodes.Callvirt) && instr.operand is MethodInfo mi)
			{
				try
				{
					if (mi.DeclaringType != null && mi.DeclaringType.Name == "FMODUWE" && mi.Name == "PlayOneShot")
					{
						var pars = mi.GetParameters();
						// Neutralize call and pop parameters so stack stays balanced
						list[i] = new CodeInstruction(OpCodes.Nop).WithLabels(instr.labels).WithBlocks(instr.blocks);
						for (int p = 0; p < pars.Length; p++)
						{
							list.Insert(i + 1, new CodeInstruction(OpCodes.Pop));
						}
						removedCalls++;
						if (NoEatingSoundsDiag.Verbose)
							Main.Log?.LogInfo($"NoEatingSounds: Stripped FMODUWE.PlayOneShot (params={pars.Length}) in {methodContext} at IL index {i}"); // verbose
					}
				}
				catch (Exception ex)
				{
					Main.Log?.LogWarning($"NoEatingSounds: Exception while scanning instruction {i} in {methodContext}: {ex}");
				}
			}
		}
		if (removedCalls == 0)
			Main.Log?.LogWarning($"NoEatingSounds: Did not find any FMODUWE.PlayOneShot calls inside {methodContext} to remove (signature may have changed)");
		else if (NoEatingSoundsDiag.Verbose)
			Main.Log?.LogInfo($"NoEatingSounds: Removed {removedCalls} FMODUWE.PlayOneShot call(s) from {methodContext}"); // verbose
		return list;
	}
}

// Fallback suppression: If another mod/postfix (e.g., Nautilus) replays a use/eat sound after our transpiler removed the original
// call, intercept FMODUWE.PlayOneShot and skip when the call originates from Survival.Eat or Survival.Use.
// This is guarded only in SUBNAUTICA build where we observed the issue.
[HarmonyPatch(typeof(FMODUWE), nameof(FMODUWE.PlayOneShot), typeof(FMODAsset), typeof(Vector3), typeof(float))]
internal static class FMODUWE_PlayOneShot_SuppressConsumption
{
	[HarmonyPrefix]
	private static bool Prefix(FMODAsset asset)
	{
		try
		{
			if (asset != null)
			{
				string path = asset.path;
				if (!string.IsNullOrEmpty(path) && FMODUWE_PlayOneShot_String_SuppressConsumption.SuppressPaths.Contains(path))
				{
					if (NoEatingSoundsDiag.Verbose) Main.Log?.LogInfo($"NoEatingSounds: Suppressing (by path) FMOD asset event '{path}'"); // verbose
					return false;
				}
			}
			var st = new StackTrace();
			for (int i = 1; i < st.FrameCount; i++)
			{
				var m = st.GetFrame(i).GetMethod();
				if (m == null)
					continue;
				var dt = m.DeclaringType;
				if (dt != null && dt.Name == nameof(Survival) && (m.Name == nameof(Survival.Eat) || m.Name == nameof(Survival.Use)))
				{
					if (NoEatingSoundsDiag.Verbose) Main.Log?.LogInfo($"NoEatingSounds: Suppressing FMOD event '{asset?.path}' from Survival.{m.Name}"); // verbose
					return false; // skip original playback
				}
			}
		}
		catch (Exception ex)
		{
			Main.Log?.LogWarning($"NoEatingSounds: Exception in FMODUWE.PlayOneShot prefix: {ex}");
		}
		return true; // allow normal playback otherwise
	}
}

// Obsolete string overload patch (some other mods or indirect calls may use this) plus stack-trace suppression.
[HarmonyPatch(typeof(FMODUWE), nameof(FMODUWE.PlayOneShot), typeof(string), typeof(Vector3), typeof(float))]
internal static class FMODUWE_PlayOneShot_String_SuppressConsumption
{
	internal static readonly HashSet<string> SuppressPaths = new(StringComparer.OrdinalIgnoreCase)
	{
		"event:/player/eat",
		"event:/player/drink" // guess; add more if discovered
	};

	[HarmonyPrefix]
	private static bool Prefix(string eventPath)
	{
		return SuppressIfConsumption(eventPath, "stringOverload");
	}

	private static bool SuppressIfConsumption(string eventPath, string tag)
	{
		try
		{
			if (!string.IsNullOrEmpty(eventPath) && SuppressPaths.Contains(eventPath))
			{
					if (NoEatingSoundsDiag.Verbose) Main.Log?.LogInfo($"NoEatingSounds: Suppressing (by path) FMOD event '{eventPath}'"); // verbose
				return false;
			}
			var st = new StackTrace();
			for (int i = 1; i < st.FrameCount; i++)
			{
				var m = st.GetFrame(i).GetMethod();
				if (m == null) continue;
				var dt = m.DeclaringType;
				if (dt != null && dt.Name == nameof(Survival) && (m.Name == nameof(Survival.Eat) || m.Name == nameof(Survival.Use)))
				{
					if (NoEatingSoundsDiag.Verbose) Main.Log?.LogInfo($"NoEatingSounds: Suppressing (via {tag}) FMOD event '{eventPath}' from Survival.{m.Name}"); // verbose
					return false;
				}
			}
			if (NoEatingSoundsDiag.Verbose) Main.Log?.LogInfo($"NoEatingSounds: Allowing FMOD event '{eventPath}' (tag={tag})"); // verbose
		}
		catch (Exception ex)
		{
			Main.Log?.LogWarning($"NoEatingSounds: Exception in PlayOneShot string overload prefix: {ex}");
		}
		return true;
	}
}

// Internal implementation patch for diagnostic logging. We can't Harmony private methods by name unless accessible via reflection; Harmony handles it.
[HarmonyPatch(typeof(FMODUWE), "PlayOneShotImpl", typeof(string), typeof(Vector3), typeof(float))]
internal static class FMODUWE_PlayOneShotImpl_Diagnostics
{
	[HarmonyPrefix]
	private static bool Prefix(string eventPath)
	{
		try
		{
			var st = new StackTrace();
			bool fromConsumption = false;
			for (int i = 1; i < st.FrameCount; i++)
			{
				var m = st.GetFrame(i).GetMethod();
				if (m == null) continue;
				var dt = m.DeclaringType;
				if (dt != null && dt.Name == nameof(Survival) && (m.Name == nameof(Survival.Eat) || m.Name == nameof(Survival.Use)))
				{
					fromConsumption = true;
					break;
				}
			}
			if (NoEatingSoundsDiag.Verbose) Main.Log?.LogInfo($"NoEatingSounds: PlayOneShotImpl eventPath='{eventPath}' fromConsumption={fromConsumption}"); // verbose
			if (fromConsumption)
			{
				// Let earlier prefixes handle suppression; we just log here.
			}
		}
		catch (Exception ex)
		{
			Main.Log?.LogWarning($"NoEatingSounds: Exception in PlayOneShotImpl diagnostics prefix: {ex}");
		}
		return true; // never suppress here (suppression handled earlier for clarity)
	}
}
#endif