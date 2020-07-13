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
        public static int ExtraCountMax;

        public static void Load()
        {
            ExtraCount = PlayerPrefs.GetInt("ExtraCount", 0);
            ExtraCountMax = PlayerPrefs.GetInt("ExtraCountMax", 0);
            OptionsPanelHandler.RegisterModOptions(new Options());
        }
    }

    public class Options : ModOptions
    {
        public Options() : base("Increased Chunk Drop Settings")
        {
            SliderChanged += ExtraCount_SliderChanged;
            SliderChanged += ExtraCountMax_SliderChanged;
        }

        public override void BuildModOptions()
        {
            AddSliderOption("ExtraCount", "Extra items min", 0, 100, Config.ExtraCount);
            AddSliderOption("ExtraCountMax", "Extra items max", 0, 100, Config.ExtraCountMax);
        }

        public void ExtraCount_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if (e.Id != "ExtraCount")
            {
                return;
            }

            Config.ExtraCount = e.IntegerValue;
            PlayerPrefs.SetInt("ExtraCount", e.IntegerValue);
        }
        public void ExtraCountMax_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if (e.Id != "ExtraCountMax")
            {
                return;
            }

            Config.ExtraCountMax = e.IntegerValue;
            PlayerPrefs.SetInt("ExtraCountMax", e.IntegerValue);
        }
    }


    [HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.BreakIntoResources))]
    internal class BreakableResource_BreakIntoResources_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(BreakableResource __instance)
        {
            int extraSpawns = UnityEngine.Random.Range(Config.ExtraCount, Config.ExtraCountMax + 1);
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