
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using UnityEngine;
using UWE;
using static LootDistributionData;

namespace CustomFragmentExample
{
    public class FragmentPrefab : PdaItem
    {
        private GameObject Prefab { get; set; }
        private List<BiomeData> BiomeData { get; set; }
        private TechType TechToCopy { get; set; }

        /// <summary>
        /// Using this constructor you can choose to pass through a <see cref="GameObject"/> Prefab of your own or call one of the prefabs already ingame by passing through a <see cref="TechType"/>.
        /// You can also Pass through a list of <see cref="LootDistributionData.BiomeData"/> to manualy set where your prefab will spawn OR if you pass through a <see cref="TechType"/> without a list then it will copy the spawn locations from that type if it does naturally spawn.
        /// NOTE! you cannot use the TechType clone to copy the loot distribution of mod added fragments.
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="friendlyName"></param>
        /// <param name="description"></param>
        /// <param name="biomeData"></param>
        /// <param name="techToCopy"></param>
        public FragmentPrefab(string classId, string friendlyName, string description, GameObject prefab = null, TechType techToCopy = TechType.None, List<BiomeData> biomeData = null) : base(classId, friendlyName, description)
        {
            Prefab = prefab ?? (techToCopy != TechType.None ? CraftData.GetPrefabForTechType(techToCopy, false) : CraftData.GetPrefabForTechType(TechType.Fragment, false));
            BiomeData = biomeData ?? new List<BiomeData>();
            TechToCopy = techToCopy;
        }

        public override List<BiomeData> BiomesToSpawnIn => GetBiomeData();

        public override WorldEntityInfo EntityInfo => GetWEI();

        public override TechType RequiredForUnlock => TechType;

        public override string DiscoverMessage => "WOOOOHOOOOO!";

        private WorldEntityInfo GetWEI()
        {
            WorldEntityInfo worldEntityInfo = new WorldEntityInfo()
            {
                classId = ClassID,
                techType = TechType,
                cellLevel = LargeWorldEntity.CellLevel.Medium,
                localScale = new Vector3(1f, 1f, 1f),
                prefabZUp = false,
                slotType = EntitySlot.Type.Small
            };

            return worldEntityInfo;
        }

        internal List<BiomeData> GetBiomeData()
        {
            if (this.TechToCopy != TechType.None && !BiomeData.Any())
            {
                // This will load the games distribution data and then return the biome distribution list for the techtype you tried to copy from if it has one.
                LootDistributionData data = LootDistributionData.Load("Balance/EntityDistributions");
                if (data.GetPrefabData(CraftData.GetClassIdForTechType(TechToCopy), out SrcData srcData))
                {
                    return srcData.distribution;
                }
            }

            return BiomeData ?? new List<BiomeData>();
        }

        /// <summary>
        /// Returns your GameObject.  This is where you would add extra components to the prefab or make changes to it.
        /// </summary>
        /// <returns></returns>
        public override GameObject GetGameObject()
        {
            GameObject _GameObject = UnityEngine.Object.Instantiate(Prefab);
            _GameObject.name = ClassID;

            return _GameObject;
        }

#if SUBNAUTICA
        /// <summary>
        /// Use this to set the sprite of your fragment.
        /// </summary>
        /// <returns></returns>
        protected override Atlas.Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechToCopy != TechType.None ? TechToCopy : TechType.Fragment);
        }

        /// <summary>
        /// These really should just be an empty techdata unless you want your fragments to be craftable items.
        /// </summary>
        /// <returns></returns>
        protected override TechData GetBlueprintRecipe()
        {
            return new TechData();
        }

#elif BELOWZERO
        /// <summary>
        /// Use this to set the sprite of your fragment.
        /// </summary>
        /// <returns></returns>
        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.Seaglide);
        }
        
        /// <summary>
        /// These really should just be an empty techdata unless you want your fragments to be craftable items.
        /// </summary>
        /// <returns></returns>
        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData;
        }

#endif
    }
}
