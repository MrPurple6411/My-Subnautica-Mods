using HarmonyLib;
using UnityEngine;

namespace IncreasedChunkDrops.Patches
{
    [HarmonyPatch(typeof(BreakableResource), "BreakIntoResources")]
    internal class BreakableResource_BreakIntoResources_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(BreakableResource __instance)
        {
            int extraSpawns = UnityEngine.Random.Range(Main.config.ExtraCount, Main.config.ExtraCountMax +1);
            while (extraSpawns > 0)
            {
                bool flag = false;
                for (int i = 0; i < __instance.numChances; i++)
                {
                    GameObject gameObject = (GameObject)AccessTools.Method(typeof(BreakableResource), "ChooseRandomResource").Invoke(__instance, null);
                    if (gameObject)
                    {
                        SpawnResourceFromPrefab(__instance, gameObject);
                        flag = true;
                    }
                }
                if (!flag)
                {
                    SpawnResourceFromPrefab(__instance, __instance.defaultPrefab);
                }

                extraSpawns--;
            }
        }

        private static void SpawnResourceFromPrefab(BreakableResource instance, GameObject breakPrefab)
        {
            Vector3 position = instance.transform.position + instance.transform.up * instance.verticalSpawnOffset;

            position.x += UnityEngine.Random.Range(-1f, 1f);
            position.y += UnityEngine.Random.Range(-1f, 1f);
            position.z += UnityEngine.Random.Range(-1f, 1f);

            GameObject gameObject = UnityEngine.Object.Instantiate(breakPrefab, position, Quaternion.identity);
            if (!gameObject.GetComponent<Rigidbody>())
            {
                gameObject.AddComponent<Rigidbody>();
            }
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            gameObject.GetComponent<Rigidbody>().AddTorque(Vector3.right * (float)UnityEngine.Random.Range(3, 6));
            gameObject.GetComponent<Rigidbody>().AddForce(instance.transform.up * 0.1f);
        }
    }

}