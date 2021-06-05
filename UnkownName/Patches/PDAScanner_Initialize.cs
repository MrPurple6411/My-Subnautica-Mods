namespace UnKnownName.Patches
{
    using System;
    using HarmonyLib;
    using SMLHelper.V2.Handlers;

#if SN1
    using RecipeData = SMLHelper.V2.Crafting.TechData;
#endif


    [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Initialize))]
    public class PDAScanner_Initialize
    {
        [HarmonyPostfix]
        [HarmonyAfter(new[] { "com.ahk1221.smlhelper" })]
        public static void Postfix()
        {
            if(Main.Config.Hardcore)
            {
                var map = PDAScanner.mapping;
                foreach(TechType techType in Enum.GetValues(typeof(TechType)))
                {
                    var data = CraftDataHandler.GetTechData(techType);
                    map.TryGetValue(techType, out var entryData);

                    if(data is null && entryData is {isFragment: false} && entryData.blueprint == TechType.None)
                    {
                        entryData.blueprint = techType;
                        entryData.locked = true;
                        continue;
                    }

                    if(data != null && entryData is null && (data.ingredientCount == 0 || techType == TechType.Titanium))
                    {
                        map[techType] = new PDAScanner.EntryData()
                        {
                            key = techType,
                            blueprint = techType,
                            destroyAfterScan = false,
                            isFragment = false,
                            locked = true,
                            scanTime = 2f,
                            totalFragments = 1
                        };
                        continue;
                    }

                    if(data != null && entryData is {isFragment: false} && entryData.blueprint == TechType.None && (data.ingredientCount == 0 || techType == TechType.Titanium))
                    {
                        entryData.blueprint = techType;
                        entryData.locked = true;
                        continue;
                    }

                    if(data is null && entryData is null)
                    {
                        map[techType] = new PDAScanner.EntryData()
                        {
                            key = techType,
                            blueprint = techType,
                            destroyAfterScan = false,
                            isFragment = false,
                            locked = true,
                            scanTime = 2f,
                            totalFragments = 1
                        };
                    }
                }
            }
        }
    }

}