using SeamothCloneTest.MonoBehaviours;
using SMCLib.Assets;
using SMCLib.Crafting;
using SMCLib.Handlers;
using SMCLib.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;
using UWE;
#if SN1
using RecipeData = SMCLib.Crafting.TechData;
using Sprite = Atlas.Sprite;
#endif

namespace SeamothCloneTest.Prefabs
{
    internal class SeamothClone : Craftable
    {
        public SeamothClone() : base("SeamothClone", TechType.Seamoth.AsString(), "Cloned Seamoth")
        {
        }

        public override WorldEntityInfo EntityInfo => GetEntityInfo();

        public override TechType RequiredForUnlock => base.RequiredForUnlock;

        public override TechGroup GroupForPDA => TechGroup.Constructor;

        public override TechCategory CategoryForPDA => TechCategory.Constructor;

        public override CraftTree.Type FabricatorType => CraftTree.Type.Constructor;

        public override string[] StepsToFabricatorTab => new string[] { "Vehicles" };

        public override float CraftingTime => GetCraftTime();

        private WorldEntityInfo GetEntityInfo()
        {
            WorldEntityInfo newEntityInfo;

            if (WorldEntityDatabase.TryGetInfo(CraftData.GetClassIdForTechType(TechType.Seamoth), out WorldEntityInfo entityInfo))
            {
                newEntityInfo = new WorldEntityInfo()
                {
                    cellLevel = entityInfo.cellLevel,
                    classId = this.ClassID,
                    localScale = entityInfo.localScale,
                    prefabZUp = entityInfo.prefabZUp,
                    slotType = entityInfo.slotType,
                    techType = this.TechType
                };

                return newEntityInfo;
            }

            newEntityInfo = new WorldEntityInfo() {
                cellLevel = LargeWorldEntity.CellLevel.Global,
                classId = this.ClassID,
                localScale = Vector3.one,
                prefabZUp = false,
                slotType = EntitySlot.Type.Medium,
                techType = this.TechType
            };

            return newEntityInfo;
        }

        private float GetCraftTime()
        {
            #if SN1
            if (CraftData.GetCraftTime(TechType.Seamoth, out float result))
            #else
            if(TechData.GetCraftTime(TechType.Exosuit, out float result))
            #endif
                return result;

            return 0f;
        }
        
#if SUBNAUTICA_STABLE
        public override GameObject GetGameObject()
        {
            if(PrefabDatabase.TryGetPrefabFilename(CraftData.GetClassIdForTechType(TechType.Seamoth), out string seamothFileName))
            {
                GameObject prefab = Resources.Load<GameObject>(seamothFileName);
                GameObject gameObject = GameObject.Instantiate(prefab);
                prefab.SetActive(false);
                gameObject.SetActive(false);

                SeaMoth seaMoth = gameObject.GetComponent<SeaMoth>();

                SeamothCloneBehaviour seamothCloneBehaviour = gameObject.EnsureComponent<SeamothCloneBehaviour>();

                typeof(SeaMoth).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic| BindingFlags.FlattenHierarchy| BindingFlags.Static).ForEach((x) => { try { x.SetValue(seamothCloneBehaviour, x.GetValue(seaMoth)); } catch { } });

                seamothCloneBehaviour.energyInterface = seaMoth.energyInterface;

                GameObject.DestroyImmediate(seaMoth);

                gameObject.SetActive(true);
                return gameObject;
            }

            return GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
#endif
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(TechType.Seamoth);
            yield return request;
            GameObject prefab = request.GetResult();
            GameObject go = GameObject.Instantiate(prefab);

            go.EnsureComponent<SeamothCloneBehaviour>();
            gameObject.Set(go);
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData() { craftAmount = 1, Ingredients = new List<Ingredient>(), LinkedItems = new List<TechType>() };
        }

        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.Seamoth);
        }
    }
}
