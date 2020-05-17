using System;
using System.Reflection;
using Harmony;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using UnityEngine;

namespace IncreasedChunkDrops
{
    [QModCore]
    public class Main
    {

        [QModPatch]
        public static void Load()
        {
            Config.Load();
            Assembly assembly = Assembly.GetExecutingAssembly();
            HarmonyInstance.Create($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
    public static class Config
    {
        public static int ExtraCount;

        public static void Load()
        {
            ExtraCount = PlayerPrefs.GetInt("ExtraCount", 0);
            OptionsPanelHandler.RegisterModOptions(new Options());
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("Increased Chunk Drop Settings")
        {
            SliderChanged += ExtraCount_SliderChanged;
        }

        public override void BuildModOptions()
        {
            AddSliderOption("ExtraCount", "Extra items", 0, 100, Config.ExtraCount);
        }

        public void ExtraCount_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if (e.Id != "ExtraCount")
            {
                return;
            }

            Config.ExtraCount = (int)e.Value;
            PlayerPrefs.SetInt("ExtraCount", (int)e.Value);
        }
    }


    [HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.BreakIntoResources))]
    internal class BreakableResource_BreakIntoResources_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(BreakableResource __instance)
        {
            int extraSpawns = Config.ExtraCount;
            while (extraSpawns > 0)
            {
                bool flag = false;
                for (int i = 0; i < __instance.numChances; i++)
                {
                    GameObject gameObject = __instance.ChooseRandomResource();
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
            GameObject gameObject = UnityEngine.Object.Instantiate(breakPrefab, instance.transform.position + instance.transform.up * instance.verticalSpawnOffset, Quaternion.identity);
            Debug.Log("broke, spawned " + breakPrefab.name);
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