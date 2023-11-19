#if SUBNAUTICA
namespace SeamothThermal.Patches;

    using HarmonyLib;
    using System.Collections;
    using UnityEngine;

    [HarmonyPatch(typeof(SeaMoth), nameof(SeaMoth.Update))]
    public class Seamoth_Update_Patch
    {
        private static readonly TaskResult<GameObject> _taskResult = new();
        private static IEnumerator _exosuitPrefabCoroutine;

        private static AnimationCurve ExosuitThermalReactorCharge { get; set; }

        private static void Prefix(SeaMoth __instance)
        {
            if (ExosuitThermalReactorCharge != null)
            {
                // If we have the SeamothThermalModule equipped.
                var count = __instance.modules.GetCount(Main.thermalModule);
                if (count > 0)
                {
                    // Evaluate the energy to add based on temperature
                    var temperature = __instance.GetTemperature();
                    var energyToAdd = ExosuitThermalReactorCharge.Evaluate(temperature);

                    // Add the energy by invoking private method using Reflection.
                    __instance.AddEnergy(energyToAdd * Time.deltaTime);
                }
            }
            else if (_exosuitPrefabCoroutine == null)
            {
                _exosuitPrefabCoroutine = CraftData.GetPrefabForTechTypeAsync(TechType.Exosuit, false, _taskResult);
                __instance.StartCoroutine(GetPrefab());
            }
        }

        private static IEnumerator GetPrefab()
        {
            yield return _exosuitPrefabCoroutine;

            var go = _taskResult.Get();

            if (go != null && go.TryGetComponent(out Exosuit exosuit))
            {
                ExosuitThermalReactorCharge = exosuit.thermalReactorCharge;
            }

        }
    }
#endif