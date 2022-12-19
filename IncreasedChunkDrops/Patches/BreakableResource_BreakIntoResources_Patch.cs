namespace IncreasedChunkDrops.Patches
{
    using HarmonyLib;
    using UnityEngine;
    using Random = UnityEngine.Random;
    using UnityEngine.AddressableAssets;

    [HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.BreakIntoResources))]
    internal class BreakableResource_BreakIntoResources_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(BreakableResource __instance)
        {
            var go = __instance.gameObject;
            var position = go.transform.position + go.transform.up * __instance.verticalSpawnOffset;
            var extraSpawns = Random.Range(Main.Config.ExtraCount, Main.Config.ExtraCountMax + 1);
            while(extraSpawns > 0)
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

                if (assetReferenceGameObject == null) continue;
                var loadPrefab = Addressables.LoadAssetAsync<GameObject>(assetReferenceGameObject.RuntimeKey as string);
                loadPrefab.Completed += (prefabTask) => {

                    var prefab = prefabTask.Result;
                    if (prefab is null)
                        return;

                    var rigidbody = Object.Instantiate(prefab, position, Quaternion.identity).GetComponent<Rigidbody>();

                    if (rigidbody == null) return;
                    rigidbody.isKinematic = false;
                    rigidbody.maxDepenetrationVelocity = 0.5f;
                    rigidbody.maxAngularVelocity = 1f;
                    rigidbody.AddTorque(Vector3.right * Random.Range(6f, 12f));
                    rigidbody.AddForce(position * 0.2f);

                };
            }
        }
    }
}