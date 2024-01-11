namespace IncreasedChunkDrops.Patches;

using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AddressableAssets;
using Nautilus.Extensions;
using UnityEngine.ResourceManagement.AsyncOperations;

[HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.BreakIntoResources))]
internal class BreakableResource_BreakIntoResources_Patch
{
	[HarmonyPostfix, HarmonyPriority(Priority.Last)]
	public static void Postfix(BreakableResource __instance, bool __runOriginal)
	{
		if (!__runOriginal)
			return;

		var go = __instance.gameObject;
		var position = go.transform.position + go.transform.up * __instance.verticalSpawnOffset;
		var extraSpawns = Random.Range(Main.SMLConfig.ExtraCount, Main.SMLConfig.ExtraCountMax + 1);

		while (extraSpawns > 0)
		{
			AssetReferenceGameObject assetReferenceGameObject = null;
			var flag = false;
			for (var i = 0; i < __instance.numChances; i++)
			{
				assetReferenceGameObject = __instance.ChooseRandomResource();
				if (assetReferenceGameObject != null)
				{
					extraSpawns--;
					flag = true;
					break;
				}
			}

			if (!flag)
			{
				assetReferenceGameObject = __instance.defaultPrefabReference;
				extraSpawns--;
			}

			if (assetReferenceGameObject == null)
				continue;

			AsyncOperationHandle<GameObject> spawnTask = assetReferenceGameObject.ForceValid().InstantiateAsync(position, Quaternion.identity);
			spawnTask.Completed += (task) =>
			{
				if (task.Status != AsyncOperationStatus.Succeeded)
					return;

				var go = task.Result;
				if (go is null)
					return;

				go.SetActive(true);

				var rigidbody = go.GetComponent<Rigidbody>();

				if (rigidbody == null)
					return;

				rigidbody.isKinematic = false;
				rigidbody.maxDepenetrationVelocity = 0.5f;
				rigidbody.maxAngularVelocity = 1f;
				rigidbody.AddTorque(Vector3.right * Random.Range(6f, 12f));
				rigidbody.AddForce(position * 0.2f);
			};
		}
	}
}