namespace BuildingTweaks.Patches
{
    using HarmonyLib;
    using UnityEngine;

#if SN1
    [HarmonyPatch(typeof(Base), nameof(Base.BuildPillars))]
    public static class Base_BuildPillars_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(Base __instance)
        {
            if(__instance.isGhost)
                return;
            
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
                            else
                            {

                                Component lifepod = target.GetComponentInParent<EscapePod>();
                                if(lifepod != null)
                                {
                                    finalTarget = lifepod.gameObject;
                                }
                            }
                        }
                    }
                }
            }

            if(finalTarget != null)
            {
                var bounds = __instance.Bounds;
                var mins = bounds.mins;
                var maxs = bounds.maxs;
                Int3 cell = default;
                for(var i = mins.z; i <= maxs.z; i++)
                {
                    cell.z = i;
                    for(var j = mins.x; j <= maxs.x; j++)
                    {
                        cell.x = j;
                        var k = mins.y;
                        while(k <= maxs.y)
                        {
                            cell.y = k;
                            if(__instance.GetCell(cell) != Base.CellType.Empty)
                            {
                                var componentInChildren = __instance.GetCellObject(cell).GetComponentInChildren<BaseFoundationPiece>();
                                if(componentInChildren != null)
                                {
                                    componentInChildren.maxPillarHeight = 0f;
                                }
                                break;
                            }
                            else
                            {
                                k++;
                            }
                        }
                    }
                }
            }
        }
    }
#elif BZ

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
#endif
}
