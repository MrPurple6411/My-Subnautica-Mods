#if SN1
namespace ScannableTimeCapsules.Patches
{
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch(typeof(TimeCapsule))]
    public static class TimeCapsule_Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(TimeCapsule.Start))]
        public static void Start_Postfix(TimeCapsule __instance)
        {
            var resourceTracker = __instance.gameObject.EnsureComponent<ResourceTracker>();

            resourceTracker.prefabIdentifier = __instance.GetComponent<PrefabIdentifier>();
            resourceTracker.techType = TechType.TimeCapsule;
            resourceTracker.overrideTechType = TechType.TimeCapsule;
            resourceTracker.rb = __instance.gameObject.GetComponent<Rigidbody>();
            resourceTracker.pickupable = __instance.gameObject.GetComponent<Pickupable>();
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(TimeCapsule.Collect))]
        public static void Collect_Prefix(TimeCapsule __instance)
        {
            var resourceTracker = __instance.gameObject.GetComponent<ResourceTracker>();

            if(resourceTracker != null)
            {
                resourceTracker.Unregister();
            }
        }
    }
}
#endif