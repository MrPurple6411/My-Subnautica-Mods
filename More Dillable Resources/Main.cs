using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QModManager.API.ModLoading;
using UWE;
using static LootDistributionData;

namespace More_Drillable_Resources
{
    [QModCore]
    public class Main
    {
        [QModPatch]
        public static void Load()
        {
            LootDistributionData data = LootDistributionData.Load("Balance/EntityDistributions");
            List<TechType> Drillables = Enum.GetValues(typeof(TechType)).OfType<TechType>().Where((tt) => tt.AsString().Contains("Drillable")).ToList();
            data.GetPrefabData(CraftData.GetClassIdForTechType(TechType.DrillableUranium), out SrcData UraniumData);

            if(UraniumData != null)
            {
                foreach (TechType techType in Drillables)
                {
                    string classId = CraftData.GetClassIdForTechType(techType);

                    if (CraftData.GetPrefabForTechType(techType, false) == null)
                    {
                        continue;
                    }

                    if (WorldEntityDatabase.TryGetInfo(classId, out WorldEntityInfo info))
                    {
                        if (!data.GetPrefabData(classId, out SrcData srcData))
                        {
                            SMLHelper.V2.Handler.LootDistributionHandler.AddLootDistributionData(classId, UraniumData);
                        }
                    }
                }
            }
        }
    }
}