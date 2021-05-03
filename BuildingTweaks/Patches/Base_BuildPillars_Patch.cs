namespace BuildingTweaks.Patches
{
    using HarmonyLib;

#if SN1
    [HarmonyPatch(typeof(Base), nameof(Base.BuildPillars))]
    public static class Base_BuildPillars_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(Base __instance)
        {
            if(__instance.isGhost)
                return;

            if(__instance.gameObject.transform.parent?.name.Contains("(Clone)") ?? false)
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
