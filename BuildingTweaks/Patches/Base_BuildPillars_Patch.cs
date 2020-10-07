using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingTweaks.Patches
{
    [HarmonyPatch(typeof(Base), "BuildPillars")]
    public static class Base_BuildPillars_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(Base __instance)
        {
            if(__instance.gameObject.transform.parent.name.Contains("(Clone)"))
			{
				if (__instance.isGhost)
				{
					return;
				}
				Int3.Bounds bounds = __instance.Bounds;
				Int3 mins = bounds.mins;
				Int3 maxs = bounds.maxs;
				Int3 cell = default(Int3);
				for (int i = mins.z; i <= maxs.z; i++)
				{
					cell.z = i;
					for (int j = mins.x; j <= maxs.x; j++)
					{
						cell.x = j;
						int k = mins.y;
						while (k <= maxs.y)
						{
							cell.y = k;
							if (__instance.GetCell(cell) != Base.CellType.Empty)
							{
								BaseFoundationPiece componentInChildren = __instance.GetCellObject(cell).GetComponentInChildren<BaseFoundationPiece>();
								if (componentInChildren != null)
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
}
