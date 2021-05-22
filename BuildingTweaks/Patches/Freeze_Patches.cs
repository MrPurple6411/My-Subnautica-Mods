namespace BuildingTweaks.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch]
    public static class Freeze_Patches
    {
        public static bool Freeze 
        { 
            get => freeze; 
            set 
            { 
                if(freeze != value)
                {
                    Time.timeScale = value ? 0.0f : 1;
                }

                freeze = value;
            } 
        }

        private static bool freeze;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Builder), nameof(Builder.Update))]
        public static bool Prefix()
        {

            if(Input.GetKeyDown(KeyCode.P))
            {
                if(Builder.ghostModel?.transform.parent != null)
                {
                    ErrorMessage.AddMessage("Removed Parent.");
                    Builder.ghostModel.transform.SetParent(null);
                }
                else if(Main.Config.AttachToTarget && Builder.ghostModel != null)
                {
                    Transform aimTransform = Builder.GetAimTransform();
                    Builder.placementTarget = null;
                    if(Physics.Raycast(aimTransform.position, aimTransform.forward, out RaycastHit hit, Builder.placeMaxDistance, Builder.placeLayerMask.value, QueryTriggerInteraction.Ignore))
                    {
                        Collider hitCollider = hit.collider;
                        Builder.placementTarget = hitCollider.gameObject;
                        GameObject parent = UWE.Utils.GetEntityRoot(Builder.placementTarget) ?? Builder.placementTarget;
                        ErrorMessage.AddMessage($"Set Parent: {parent.transform}");
                        Builder.ghostModel.transform.SetParent(parent.transform);
                    }
                }
            }

            return !Freeze;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Builder), nameof(Builder.TryPlace))]
        public static void Postfix(bool __result)
        {
            if(Freeze && __result)
                Freeze = !Freeze;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Builder), nameof(Builder.End))]
        public static void Postfix()
        {
            Freeze = false;
        }
    }
}
