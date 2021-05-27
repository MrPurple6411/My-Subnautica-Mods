namespace Base_Legs_Removal.Patches
{
    using HarmonyLib;
    using UnityEngine;

#if SN1
    [HarmonyPatch(typeof(BaseFoundationPiece), nameof(BaseFoundationPiece.OnGenerate))]
    public static class BaseFoundationPiece_OnGenerate_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(BaseFoundationPiece __instance)
        {

            var target = __instance.GetComponentInParent<Base>().gameObject;
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
                            else
                            {

                                Component lifePod =
#if SN1
                            target.GetComponentInParent<EscapePod>();
#elif BZ
                            placementTarget.GetComponentInParent<LifepodDrop>();
#endif
                                if(lifePod != null)
                                {
                                    finalTarget = lifePod.gameObject;
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

            if (finalTarget != null)
            {
                ErrorMessage.AddMessage($"{finalTarget.gameObject.name}");
                return;
            }

            var maxHeight = 0f;
            var config = Main.Config;

            switch(__instance.name)
            {
                case "BaseRoomAdjustableSupport(Clone)":
                    maxHeight = config.RoomLegs ? 0 : 20f;
                    break;
                case "BaseMoonpool(Clone)":
                    maxHeight = config.MoonPoolLegs ? 0 : 20f;
                    break;
                case "BaseFoundationPiece(Clone)":
                    maxHeight = config.FoundationLegs ? 0 : 20f;
                    break;
                case "BaseCorridorXShapeAdjustableSupport(Clone)":
                    maxHeight = config.XCorridor ? 0 : 20f;
                    break;
                case "BaseCorridorTShapeAdjustableSupport(Clone)":
                    maxHeight = config.TCorridor ? 0 : 20f;
                    break;
                case "BaseCorridorLShapeAdjustableSupport(Clone)":
                    maxHeight = config.LCorridor ? 0 : 20f;
                    break;
                case "BaseCorridorIShapeAdjustableSupport(Clone)":
                    maxHeight = config.ICorridor ? 0 : 20f;
                    break;
            }

            __instance.maxPillarHeight = maxHeight;
        }
    }
#elif BZ

	[HarmonyPatch(typeof(Base), nameof(Base.BuildAccessoryGeometry))]
	public static class Base_BuildPillars_Patch
	{
		[HarmonyPrefix]
		public static void Prefix(Base __instance)
		{
			var componentsInChildren = __instance.gameObject.GetComponentsInChildren<IBaseAccessoryGeometry>();
			for (var i = 0; i < componentsInChildren.Length; i++)
			{
				var baseAccessoryGeometry = componentsInChildren[i];
				switch (baseAccessoryGeometry)
				{
					case BaseFoundationPiece baseFoundationPiece:
						var maxHeight = 0f;
						var config = Main.Config;
						var buildTweaksCheck = __instance.gameObject.transform.parent?.name.Contains("(Clone)") ?? false;

						if(!buildTweaksCheck)
						{
							switch(baseFoundationPiece.name)
							{
								case "BaseRoomAdjustableSupport(Clone)":
									maxHeight = config.RoomLegs ? 0 : 20f;
									break;
								case "BaseLargeRoomAdjustableSupport(Clone)":
									maxHeight = config.LargeRoomLegs ? 0 : 20f;
									break;
								case "BaseMoonpool(Clone)":
									maxHeight = config.MoonPoolLegs ? 0 : 20f;
									break;
								case "BaseFoundationPiece(Clone)":
									maxHeight = config.FoundationLegs ? 0 : 20f;
									break;
								case "BaseCorridorXShapeAdjustableSupport(Clone)":
									maxHeight = config.XCorridor ? 0 : 20f;
									break;
								case "BaseCorridorTShapeAdjustableSupport(Clone)":
									maxHeight = config.TCorridor ? 0 : 20f;
									break;
								case "BaseCorridorLShapeAdjustableSupport(Clone)":
									maxHeight = config.LCorridor ? 0 : 20f;
									break;
								case "BaseCorridorIShapeAdjustableSupport(Clone)":
									maxHeight = config.ICorridor ? 0 : 20f;
									break;
							}
						}
						else
						{
							maxHeight = 0;
						}

						baseFoundationPiece.maxPillarHeight = maxHeight;
						break;
				}
			}
		}
	}
#endif
}
