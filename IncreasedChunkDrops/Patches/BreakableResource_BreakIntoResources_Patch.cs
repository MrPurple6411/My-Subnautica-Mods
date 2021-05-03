namespace IncreasedChunkDrops.Patches
{
    using HarmonyLib;
    using UnityEngine;
    using Random = UnityEngine.Random;
#if !SUBNAUTICA_STABLE
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
#endif

    [HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.BreakIntoResources))]
    internal class BreakableResource_BreakIntoResources_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(BreakableResource __instance)
        {
            Vector3 position = __instance.gameObject.transform.position + (__instance.gameObject.transform.up * __instance.verticalSpawnOffset);
            int extraSpawns = Random.Range(Main.Config.ExtraCount, Main.Config.ExtraCountMax + 1);
            while(extraSpawns > 0)
            {
#if SUBNAUTICA_STABLE
                Rigidbody rigidbody = null;
                bool flag = false;
                for(int i = 0; i < __instance.numChances; i++)
                {
                    GameObject prefab = __instance.ChooseRandomResource();
                    if(prefab != null)
                    {
                        rigidbody = GameObject.Instantiate(prefab, position, Quaternion.identity).EnsureComponent<Rigidbody>();
                        flag = true;
                        break;
                    }
                }
                if(!flag)
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
                AssetReferenceGameObject assetReferenceGameObject = null;
                bool flag = false;
                for (int i = 0; i < __instance.numChances; i++)
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

                if(assetReferenceGameObject != null)
                {
                    AsyncOperationHandle<GameObject> loadPrefab = Addressables.LoadAssetAsync<GameObject>(assetReferenceGameObject.RuntimeKey as string);
                    loadPrefab.Completed += (prefabTask) => {

                        GameObject prefab = prefabTask.Result;
                        if (prefab is null)
                            return;

                        Rigidbody rigidbody = GameObject.Instantiate(prefab, position, Quaternion.identity).GetComponent<Rigidbody>();

                        if (rigidbody != null)
                        {
                            rigidbody.isKinematic = false;
                            rigidbody.maxDepenetrationVelocity = 0.5f;
                            rigidbody.maxAngularVelocity = 1f;
                            rigidbody.AddTorque(Vector3.right * Random.Range(6f, 12f));
                            rigidbody.AddForce(position * 0.2f);
                        }

                    };
                }
            }
        }
#endif
    }
}