using HarmonyLib;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BZDeconstructFix.Patches
{

    [HarmonyPatch(typeof(BaseDeconstructable), nameof(BaseDeconstructable.Awake))]
    public class BaseDeconstructable_Deconstruct_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(BaseDeconstructable __instance)
        {
            if (BaseDeconstructable.baseDeconstructablePrefab is null)
            {
                AsyncOperationHandle<GameObject> request = AddressablesUtility.LoadAsync<GameObject>("Base/Ghosts/BaseDeconstructable.prefab");
                request.Completed += (x) =>
                {
                    if(x.Status == AsyncOperationStatus.Succeeded)
                    {
                        BaseDeconstructable.baseDeconstructablePrefab = x.Result;
                    }
                };
            }
        }
    }
}
