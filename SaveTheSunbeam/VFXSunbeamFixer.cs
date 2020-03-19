using Harmony;
using UnityEngine;

namespace SaveTheSunbeam
{
    class VFXSunbeamFixer
    {

        [HarmonyPatch(typeof(VFXSunbeam))]
        [HarmonyPatch("PlaySequence")]
        internal class PlaySequenceFixer
        {
            [HarmonyPrefix]
            public static bool Prefix(VFXSunbeam __instance)
            {
                if (StoryGoalCustomEventHandler.main.gunDisabled)
                {
                    MonoBehaviour main = __instance;
                    GameObject prefab = __instance.shipPrefab;
                    Transform parent = main.transform;
                    GameObject gameObject = Utils.SpawnZeroedAt(prefab, parent, false);
                    ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
                    component.Play();
                    return false;
                }
                return true;
            }
        }
    }
}
