namespace Base_Legs_Removal.Patches
{
    using HarmonyLib;

#if SN1
    [HarmonyPatch(typeof(BaseFoundationPiece), nameof(BaseFoundationPiece.OnGenerate))]
    public static class BaseFoundationPiece_OnGenerate_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(BaseFoundationPiece __instance)
        {
            if(__instance.gameObject.transform.parent?.name.Contains("(Clone)") ?? false)
                return;

            float maxHeight = 0f;
            Configuration.Config config = Main.Config;

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
			IBaseAccessoryGeometry[] componentsInChildren = __instance.gameObject.GetComponentsInChildren<IBaseAccessoryGeometry>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				IBaseAccessoryGeometry baseAccessoryGeometry = componentsInChildren[i];
				switch (baseAccessoryGeometry)
				{
					case BaseFoundationPiece baseFoundationPiece:
						float maxHeight = 0f;
						var config = Main.Config;
						bool buildTweaksCheck = __instance.gameObject.transform.parent?.name.Contains("(Clone)") ?? false;

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
