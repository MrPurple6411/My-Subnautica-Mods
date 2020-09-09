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
            data.GetPrefabData(CraftData.GetClassIdForTechType(TechType.DrillableUranium), out SrcData srcData);

            if(srcData != null)
            {
                List<TechType> Drillables = Enum.GetValues(typeof(TechType)).OfType<TechType>().Where((tt) => tt.AsString().Contains("Drillable")).ToList();
                foreach (TechType techType in Drillables)
                {
                    string classId = CraftData.GetClassIdForTechType(techType);

                    if (!PrefabDatabase.TryGetPrefabFilename(classId, out string filename))
                    {
                        continue;
                    }

                    if (WorldEntityDatabase.TryGetInfo(classId, out WorldEntityInfo info))
                    {
                        if (!data.GetPrefabData(classId, out _))
                        {
                            SMLHelper.V2.Handler.LootDistributionHandler.AddLootDistributionData(classId, srcData);
                        }
                    }
                }
            }
        }
    }
}