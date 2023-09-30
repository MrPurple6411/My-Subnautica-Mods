namespace BaseLegsRemoval.Patches;

using HarmonyLib;
using UnityEngine;


	[HarmonyPatch(typeof(Base), nameof(Base.BuildAccessoryGeometry))]
	public static class Base_BuildPillars_Patch
	{
		[HarmonyPrefix]
		public static void Prefix(Base __instance)
		{
        var target = __instance.gameObject;
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
	                    var parent = target.transform.parent;
	                    var subRoot = parent != null ? parent.GetComponentInParent<SubRoot>() : null;
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

                            var lifePod = target.GetComponentInParent<LifepodDrop>();
                            if(lifePod != null)
                            {
                                finalTarget = lifePod.gameObject;
                            }
                            else
                            {
                                var seaTruck = target.GetComponentInParent<SeaTruckSegment>();
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
            return;
        }
			var componentsInChildren = __instance.gameObject.GetComponentsInChildren<IBaseAccessoryGeometry>();
			foreach (var baseAccessoryGeometry in componentsInChildren)
        {
            switch (baseAccessoryGeometry)
            {
                case BaseFoundationPiece baseFoundationPiece:
                    var maxHeight = 0f;
                    var config = Main.SMLConfig;
                    maxHeight = baseFoundationPiece.name switch
                    {
                        "BaseRoomAdjustableSupport(Clone)" => config.RoomLegs ? 0 : 20f,
#if BELOWZERO
                        "BaseLargeRoomAdjustableSupport(Clone)" => config.LargeRoomLegs ? 0 : 20f,
#endif
                        "BaseMoonpool(Clone)" => config.MoonPoolLegs ? 0 : 20f,
                        "BaseFoundationPiece(Clone)" => config.FoundationLegs ? 0 : 20f,
                        "BaseCorridorXShapeAdjustableSupport(Clone)" => config.XCorridor ? 0 : 20f,
                        "BaseCorridorTShapeAdjustableSupport(Clone)" => config.TCorridor ? 0 : 20f,
                        "BaseCorridorLShapeAdjustableSupport(Clone)" => config.LCorridor ? 0 : 20f,
                        "BaseCorridorIShapeAdjustableSupport(Clone)" => config.ICorridor ? 0 : 20f,
                        _ => maxHeight
                    };

                    baseFoundationPiece.maxPillarHeight = maxHeight;
                    break;
            }
        }
		}
	}

                    
