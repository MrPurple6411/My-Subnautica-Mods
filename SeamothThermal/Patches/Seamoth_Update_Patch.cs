#if SN1
namespace SeamothThermal.Patches
{
    using HarmonyLib;
    using System.Collections;
    using UnityEngine;

    [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Update))]
    public class Seamoth_Update_Patch
    {
        private static TaskResult<GameObject> m_TaskResult = new TaskResult<GameObject>();
        private static IEnumerator exosuitPrefabCoroutine;

        private static AnimationCurve ExosuitThermalReactorCharge { get; set; }

        private static void Prefix(SeaMoth __instance)
        {
            if (ExosuitThermalReactorCharge != null)
            {
                // If we have the SeamothThermalModule equipped.
                var count = __instance.modules.GetCount(Main.thermalModule.TechType);
                if (count > 0)
                {
                    // Evaluate the energy to add based on temperature
                    var temperature = __instance.GetTemperature();
                    var energyToAdd = ExosuitThermalReactorCharge.Evaluate(temperature);

                    // Add the energy by invoking private method using Reflection.
                    __instance.AddEnergy(energyToAdd * Time.deltaTime);
                }
            }
            else if (exosuitPrefabCoroutine == null)
            {
                exosuitPrefabCoroutine = CraftData.GetPrefabForTechTypeAsync(TechType.Exosuit, false, m_TaskResult);
                __instance.StartCoroutine(GetPrefab());
            }
        }

        private static IEnumerator GetPrefab()
        {
            yield return exosuitPrefabCoroutine;

            var go = m_TaskResult.Get();

            if (go != null && go.TryGetComponent(out Exosuit exosuit))
            {
                ExosuitThermalReactorCharge = exosuit.thermalReactorCharge;
            }

        }
    }
}
#endif