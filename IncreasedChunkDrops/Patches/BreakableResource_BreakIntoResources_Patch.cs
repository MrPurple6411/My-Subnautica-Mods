using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

#if !SUBNAUTICA_STABLE
using UnityEngine.AddressableAssets;
using UWE;
#endif

namespace IncreasedChunkDrops.Patches
{
    [HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.BreakIntoResources))]
    internal class BreakableResource_BreakIntoResources_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(BreakableResource __instance)
        {
            int extraSpawns = Random.Range(Main.config.ExtraCount, Main.config.ExtraCountMax +1);
#if SUBNAUTICA_STABLE
            Vector3 position = __instance.gameObject.transform.position + (__instance.gameObject.transform.up * __instance.verticalSpawnOffset);
            while (extraSpawns > 0)
            {
                Rigidbody rigidbody = null;
                bool flag = false;
                for (int i = 0; i < __instance.numChances; i++)
                {
                    GameObject prefab = __instance.ChooseRandomResource();
                    if (prefab != null)
                    {
                        rigidbody = GameObject.Instantiate(prefab, position, Quaternion.identity).EnsureComponent<Rigidbody>();
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    rigidbody = GameObject.Instantiate(__instance.defaultPrefab, position, Quaternion.identity).EnsureComponent<Rigidbody>();
                }

                if(rigidbody != null)
                {
                    rigidbody.isKinematic = false;
                    rigidbody.maxDepenetrationVelocity = 0.5f;
                    rigidbody.maxAngularVelocity = 1f;
                    rigidbody.AddTorque(Vector3.right * Random.Range(6f, 12f));
                    rigidbody.AddForce(position * 0.2f);
                }
                extraSpawns--;
            }
        }

#else
            while (extraSpawns > 0)
            {
                bool flag = false;
                for (int i = 0; i < __instance.numChances; i++)
                {
                    AssetReferenceGameObject assetReferenceGameObject = __instance.ChooseRandomResource();
                    if (assetReferenceGameObject != null)
                    {
                        CoroutineHost.StartCoroutine(SpawnFromPrefeb(assetReferenceGameObject, __instance.gameObject.transform.position, __instance.gameObject.transform.up * __instance.verticalSpawnOffset));
                        extraSpawns--;
                        flag = true;
                    }
                }
                if (!flag)
                {
                    CoroutineHost.StartCoroutine(SpawnFromPrefeb(__instance.defaultPrefabReference, __instance.gameObject.transform.position, __instance.gameObject.transform.up * __instance.verticalSpawnOffset));
                    extraSpawns--;
                }
            }
        }

        private static IEnumerator SpawnFromPrefeb(AssetReferenceGameObject breakPrefab, Vector3 position, Vector3 up)
        {
            CoroutineTask<GameObject> result = AddressablesUtility.InstantiateAsync(breakPrefab.RuntimeKey as string, null, position, default(Quaternion), true);
            yield return result;
            GameObject result2 = result.GetResult();
            if (result2 == null)
            {
                Debug.LogErrorFormat("Failed to spawn {0}" + breakPrefab.RuntimeKey, Array.Empty<object>());
                yield break;
            }
            Debug.Log("broke, spawned " + result2.name);
            Rigidbody rigidbody = result2.EnsureComponent<Rigidbody>();
            UWE.Utils.SetIsKinematicAndUpdateInterpolation(rigidbody, false);
            rigidbody.maxDepenetrationVelocity = 0.5f;
            rigidbody.maxAngularVelocity = 1f;
            rigidbody.AddTorque(Vector3.right * (float)UnityEngine.Random.Range(3, 6));
            rigidbody.AddForce(up * 0.1f);
            yield break;
        }
#endif
    }
}