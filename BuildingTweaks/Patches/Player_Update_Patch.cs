using HarmonyLib;
using UnityEngine;

namespace BuildingTweaks.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    public static class Player_Update_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {

            if (Input.GetKeyDown(Main.config.AttachToTargetToggle))
            {
                Main.config.AttachToTarget = !Main.config.AttachToTarget;
                ErrorMessage.AddMessage($"Attach as target override = {Main.config.AttachToTarget}");
            }

            if (Input.GetKeyDown(Main.config.FullOverrideToggle))
            {
                Main.config.FullOverride = !Main.config.FullOverride;
                ErrorMessage.AddMessage($"Full Override = {Main.config.FullOverride}");
            }

            if (__instance.currentWaterPark?.GetComponentInParent<Creature>() != null)
            {
                Vector3 vector3 = __instance.currentWaterPark.transform.position;
                __instance.SetPosition(vector3);
                return;
            }

#if SN1
            SubRoot currentSubRoot = __instance.GetCurrentSub();
            if (currentSubRoot != null && currentSubRoot is BaseRoot && __instance.playerController.velocity.y < -20f)
            {
                RespawnPoint componentInChildren = currentSubRoot.gameObject.GetComponentInChildren<RespawnPoint>();
                if (componentInChildren)
                {
                    __instance.SetPosition(componentInChildren.GetSpawnPosition());
                    return;
                }
            }

            EscapePod escapePod = __instance.currentEscapePod;
            if(escapePod != null && __instance.playerController.velocity.y < -20f)
            {
                __instance.SetPosition(escapePod.playerSpawn.transform.position, escapePod.playerSpawn.transform.rotation);
                return;
            }
#elif BZ
            IInteriorSpace interiorSpace = __instance.currentInterior;
            if (interiorSpace != null && __instance.playerController.velocity.y < -20f)
            {
                RespawnPoint respawnPoint = interiorSpace.GetRespawnPoint();
                if (respawnPoint)
                {
                    __instance.SetPosition(respawnPoint.GetSpawnPosition());
                    return;
                }
            }
#endif
        }
    }
}
