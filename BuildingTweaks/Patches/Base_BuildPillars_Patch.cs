namespace BuildingTweaks.Patches;

using HarmonyLib;
using UnityEngine;


[HarmonyPatch(typeof(Base), nameof(Base.BuildAccessoryGeometry))]
	public static class Base_BuildPillars_Patch
	{
		[HarmonyPrefix]
		[HarmonyPriority(Priority.Last)]
		public static void Prefix(Base __instance)
    {
        if (__instance.isGhost) return;

        var target = UWE.Utils.GetEntityRoot(__instance.gameObject) ?? __instance.gameObject;
        GameObject finalTarget = null;

        if(target != null)
        {
            var pickupable = target.GetComponentInParent<Pickupable>();
            if(pickupable != null)
            {
                finalTarget = pickupable.gameObject;
            }
            else
            {
                var creature = target.GetComponentInParent<Creature>();
                if(creature != null)
                {
                    finalTarget = creature.gameObject;
                }
                else
                {
                    var subRoot = target.transform.parent?.gameObject.GetComponentInParent<SubRoot>();
                    if(subRoot != null)
                    {
                        finalTarget = subRoot.modulesRoot.gameObject;
                    }
                    else
                    {
                        var vehicle = target.GetComponentInParent<Vehicle>();
                        if(vehicle != null)
                        {
                            finalTarget = vehicle.modulesRoot.gameObject;
                        }
#if BELOWZERO
                        else
                        {

                            Component lifepod = target.GetComponentInParent<LifepodDrop>();
                            if(lifepod != null)
                            {
                                finalTarget = lifepod.gameObject;
                            }
                            else
                            {
                                SeaTruckSegment seaTruck = target.GetComponentInParent<SeaTruckSegment>();
                                if(seaTruck != null)
                                    finalTarget = seaTruck.gameObject;
                            }
                        }
#endif
                    }
                }
            }
        }

        if (finalTarget != null)
        {
            var componentsInChildren = __instance.gameObject.GetComponentsInChildren<IBaseAccessoryGeometry>();
            foreach (var baseAccessoryGeometry in componentsInChildren)
            {
                switch (baseAccessoryGeometry)
                {
                    case BaseFoundationPiece baseFoundationPiece:
                        baseFoundationPiece.maxPillarHeight = 0f;
                        break;
                }
            }
        }
    }
	}
