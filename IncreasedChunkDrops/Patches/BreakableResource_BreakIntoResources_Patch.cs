namespace IncreasedChunkDrops.Patches
{
    using HarmonyLib;
    using UnityEngine;
    using Random = UnityEngine.Random;
    using UnityEngine.AddressableAssets;
    using BepInEx.Logging;
    using UWE;
    using System.Collections;

    [HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.BreakIntoResources))]
    internal class BreakableResource_BreakIntoResources_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(BreakableResource __instance)
        {
            var go = __instance.gameObject;
            var position = go.transform.position + go.transform.up * __instance.verticalSpawnOffset;
            var extraSpawns = Random.Range(Main.SMLConfig.ExtraCount, Main.SMLConfig.ExtraCountMax + 1);
            while(extraSpawns > 0)
            {
                AssetReferenceGameObject assetReferenceGameObject = null;
                var flag = false;
                for(var i = 0; i < __instance.numChances; i++)
                {
                    assetReferenceGameObject = __instance.ChooseRandomResource();
                    if(assetReferenceGameObject != null)
                    {
                        extraSpawns--;
                        flag = true;
                        break;
                    }
                }
                if(!flag)
                {
                    assetReferenceGameObject = __instance.defaultPrefabReference;
                    extraSpawns--;
                }

                if(assetReferenceGameObject == null)
                    continue;

                CoroutineHost.StartCoroutine(SpawnObject(assetReferenceGameObject, position));
            }
        }

        private static IEnumerator SpawnObject(AssetReferenceGameObject assetReferenceGameObject, Vector3 position)
        {
            var log = assetReferenceGameObject.RuntimeKey is string key && !key.EndsWith("prefab");
            var task = AddressablesUtility.InstantiateAsync(assetReferenceGameObject.RuntimeKey as string, null, position, Quaternion.identity);
            yield return task;
            var go = task.GetResult();

            if(log)
            {
                Main.logSource.LogMessage($"key: {assetReferenceGameObject.RuntimeKey}");
                Main.logSource.LogMessage($"go: {go != null}");
            }

            if(go is null)
                yield break;

            var rigidbody = go.GetComponent<Rigidbody>();
            if(log)
            {
                Main.logSource.LogMessage($"rigidbody: {rigidbody != null}");
            }

            if(rigidbody == null)
                yield break;

            if(log)
            {
                Main.logSource.LogMessage($"Distance: {Vector3.Distance(Player.main.transform.position, go.transform.position)}");
            }

            rigidbody.isKinematic = false;
            rigidbody.maxDepenetrationVelocity = 0.5f;
            rigidbody.maxAngularVelocity = 1f;
            rigidbody.AddTorque(Vector3.right * Random.Range(6f, 12f));
            rigidbody.AddForce(position * 0.2f);
        }
    }
}