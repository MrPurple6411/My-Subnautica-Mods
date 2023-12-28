namespace BulletTime.Patches;

using HarmonyLib;
using UnityEngine;

[HarmonyPatch]
public class Patches
{
	/// <summary>
	/// Patch to make the player move at normal speed while timescale is slowed down.
	/// </summary>
	/// <param name="__instance"></param>
	/// <param name="__result"></param>
	[HarmonyPatch(typeof(GroundMotor), nameof(GroundMotor.GetMaxAcceleration))]
	[HarmonyPatch(typeof(UnderwaterMotor), nameof(UnderwaterMotor.AlterMaxSpeed))]
	[HarmonyPostfix]
	public static void UnderwaterMotor_AlterMaxSpeed_Postfix(ref float __result)
	{
		if (Time.timeScale == 0 || Time.timeScale >= 1)
			return;

		__result /= Time.timeScale;
	}

	/// <summary>
	/// This patch takes the change of velocity and divides it by the timescale and adds it back to input velocity to make the player move at normal speed while timescale is slowed down.
	/// </summary>
	/// <param name="velocity"></param>
	/// <param name="__result"></param>
	[HarmonyPatch(typeof(GroundMotor), nameof(GroundMotor.ApplyInputVelocityChange))]
	[HarmonyPostfix]
	public static void GroundMotor_ApplyInputVelocityChange_Postfix(Vector3 velocity, ref Vector3 __result)
	{
		if (Time.timeScale == 0 || Time.timeScale >= 1)
		{
			return;
		}
		var inputVelocityChange = velocity - __result;
		__result += inputVelocityChange / Time.timeScale;
		ErrorMessage.AddDebug($"__result: {__result.magnitude}");
	}

	[HarmonyPatch(typeof(PlayerController), nameof(PlayerController.Update))]
	[HarmonyPostfix]
	public static void PlayerController_Update_Postfix(PlayerController __instance)
	{
		if (Plugin.BulletTimeEnabled && !__instance.useRigidbody.isKinematic)
		{
			__instance.UpdateController();
		}
	}
}
