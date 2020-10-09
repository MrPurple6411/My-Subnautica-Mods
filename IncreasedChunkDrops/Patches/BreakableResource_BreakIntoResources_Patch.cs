using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UWE;

namespace IncreasedChunkDrops.Patches
{
    [HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.BreakIntoResources))]
    internal class BreakableResource_BreakIntoResources_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(BreakableResource __instance)
        {
            Vector3 position = __instance.gameObject.transform.position + (__instance.gameObject.transform.up * __instance.verticalSpawnOffset);

            int extraSpawns = Random.Range(Main.config.ExtraCount, Main.config.ExtraCountMax +1);
            while (extraSpawns > 0)
            {
                Rigidbody rigidbody = null;
                bool flag = false;
                for (int i = 0; i < __instance.numChances; i++)
                {
                    GameObject prefab = __instance.ChooseRandomResource();
                    if (prefab != null)
                    {
                        rigidbody = Object.Instantiate(prefab, position, Quaternion.identity).EnsureComponent<Rigidbody>();
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    rigidbody = Object.Instantiate(__instance.defaultPrefab, position, Quaternion.identity).EnsureComponent<Rigidbody>();
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

    }

}