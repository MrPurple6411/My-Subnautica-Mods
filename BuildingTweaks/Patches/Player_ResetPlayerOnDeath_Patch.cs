using HarmonyLib;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UWE;

namespace BuildingTweaks.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.ResetPlayerOnDeath))]
    public static class Player_ResetPlayerOnDeath_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
			SubRoot lastValidSub = __instance.lastValidSub;

			if (lastValidSub != null && __instance.CheckSubValid(lastValidSub))
			{
				CoroutineHost.StartCoroutine(WaitThenMovePlayer(__instance, lastValidSub));
			}
		}

		public static IEnumerator WaitThenMovePlayer(Player __instance, SubRoot lastValidSub)
		{
			while (!__instance.playerController.inputEnabled)
			{
				yield return CoroutineUtils.waitForNextFrame;
			}

			RespawnPoint componentInChildren = lastValidSub.gameObject.GetComponentInChildren<RespawnPoint>();
			if (componentInChildren)
			{
				__instance.SetPosition(componentInChildren.GetSpawnPosition());
				__instance.SetCurrentSub(lastValidSub);
			}

			yield break;
		}
	}
}