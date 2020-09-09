using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using UnityEngine;

namespace DropUpgradesOnDestroy
{
    [QModCore]
    public class Main
    {
        [QModPatch]
        public static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }

        internal static IEnumerator SpawnModuleNearby(List<InventoryItem> equipment, Vector3 position)
        {
            foreach(InventoryItem item in equipment)
            {
                TechType techType = item.item.GetTechType();
                CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techType);
                yield return task;

                GameObject gameObject = GameObject.Instantiate(task.GetResult());
                gameObject.transform.position = new Vector3(position.x + UnityEngine.Random.Range(-3, 3), position.y + UnityEngine.Random.Range(5, 8), position.z + UnityEngine.Random.Range(-3, 3));
                gameObject.SetActive(true);

            }
            yield break;
        }
    }
}
