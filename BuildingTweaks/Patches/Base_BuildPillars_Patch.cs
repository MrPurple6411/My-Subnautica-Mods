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
            
            GameObject target = UWE.Utils.GetEntityRoot(__instance.gameObject) ?? __instance.gameObject;
            GameObject finalTarget = null;

            if(target != null)
            {
                Pickupable pickupable = target.GetComponentInParent<Pickupable>();
                if(pickupable != null)
                {
                    finalTarget = pickupable.gameObject;
                }
                else
                {
                    Creature creature = target.GetComponentInParent<Creature>();
                    if(creature != null)
                    {
                        finalTarget = creature.gameObject;
                    }
                    else
                    {
                        SubRoot subRoot = target.GetComponentInParent<SubRoot>();
                        if(subRoot != null)
                        {
                            finalTarget = subRoot.modulesRoot.gameObject;
                        }
                        else
                        {
                            Vehicle vehicle = target.GetComponentInParent<Vehicle>();
                            if(vehicle != null)
                            {
                                finalTarget = vehicle.modulesRoot.gameObject;
                            }
                            else
                            {

                                Component lifepod =
#if SN1
                            target.GetComponentInParent<EscapePod>();
#elif BZ
                            placementTarget.GetComponentInParent<LifepodDrop>();
#endif
                                if(lifepod != null)
                                {
                                    finalTarget = lifepod.gameObject;
                                }
#if BZ
                                    else
                                    {
                                        SeaTruckSegment seaTruck = placementTarget.GetComponentInParent<SeaTruckSegment>();
                                        if(seaTruck != null)
                                            finalTarget = seaTruck.gameObject;
                                    }
#endif
                            }
                        }
                    }
                }
            }

            if(finalTarget != null)
            {
                Int3.Bounds bounds = __instance.Bounds;
                Int3 mins = bounds.mins;
                Int3 maxs = bounds.maxs;
                Int3 cell = default;
                for(int i = mins.z; i <= maxs.z; i++)
                {
                    cell.z = i;
                    for(int j = mins.x; j <= maxs.x; j++)
                    {
                        cell.x = j;
                        int k = mins.y;
                        while(k <= maxs.y)
                        {
                            cell.y = k;
                            if(__instance.GetCell(cell) != Base.CellType.Empty)
                            {
                                BaseFoundationPiece componentInChildren = __instance.GetCellObject(cell).GetComponentInChildren<BaseFoundationPiece>();
                                if(componentInChildren != null)
                                {
                                    componentInChildren.maxPillarHeight = 0f;
                                    break;
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
			if (__instance.gameObject.transform.parent?.name.Contains("(Clone)") ?? false)
			{
				if (!__instance.isGhost)
				{
					IBaseAccessoryGeometry[] componentsInChildren = __instance.gameObject.GetComponentsInChildren<IBaseAccessoryGeometry>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						IBaseAccessoryGeometry baseAccessoryGeometry = componentsInChildren[i];

						switch (baseAccessoryGeometry)
						{
							case BaseFoundationPiece baseFoundationPiece:
								baseFoundationPiece.maxPillarHeight = 0f;
								break;
						}
					}
					return;
				}
			}
		}
	}
#endif
}
