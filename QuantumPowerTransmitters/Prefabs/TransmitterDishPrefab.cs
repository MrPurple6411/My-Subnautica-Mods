using FMOD;
using Oculus.Newtonsoft.Json;
using QModManager.Utility;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UWE;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Utility;

//Subnautica and Below Zero handle Crafting recipes and Sprites differently. this makes it easier to write the code below.
#if SUBNAUTICA
using Data = SMLHelper.V2.Crafting.TechData;
using Sprite = Atlas.Sprite;
#elif BELOWZERO
using Data = SMLHelper.V2.Crafting.RecipeData;
#endif

namespace QuantumPowerTransmitters.Prefabs
{
    internal class TransmitterDishPrefab : Buildable
    {
        //Load my AssetBundle that I have embedded into the mod's DLL that contains the Prefab I created/edited in Unity Editor.
        private static AssetBundle assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("QuantumPowerTransmitters.Resources.powertransmitterdish"));

        public TransmitterDishPrefab() : base("QuantumPowerTransmitter", "Quantum Power Transmitter", "Base Power Transfer not limited by distance.")
        {
        }

        public override WorldEntityInfo EntityInfo => new WorldEntityInfo() { cellLevel = LargeWorldEntity.CellLevel.Global, classId = this.ClassID, localScale = Vector3.one, prefabZUp = false, slotType = EntitySlot.Type.Medium, techType = this.TechType };

        public override TechType RequiredForUnlock => TechType.PrecursorIonCrystal;

        public override TechGroup GroupForPDA => TechGroup.ExteriorModules;

        public override TechCategory CategoryForPDA => TechCategory.ExteriorModule;

        public override GameObject GetGameObject()
        {
            //Instantiates a copy of the prefab that is loaded from the AssetBundle loaded above.
            GameObject gameObject = GameObject.Instantiate(assetBundle.LoadAsset<GameObject>("Dish.prefab"));

            ApplySubnauticaShaders(gameObject);

            gameObject.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Global;
            gameObject.EnsureComponent<PrefabIdentifier>().ClassId = ClassID;

            // Required to be able to build the object using the Builder tool. Also sets limits to where it can be built, if it can be removed after construction and if rotating it is allowed.
            Constructable cs = gameObject.EnsureComponent<Constructable>();
            cs.model = gameObject.transform.GetChild(0).gameObject;
            cs.allowedInBase = false;
            cs.allowedInSub = false;
            cs.allowedOutside = true;
            cs.allowedOnConstructables = true;
            cs.allowedOnGround = true;
            cs.allowedOnWall = false;
            cs.deconstructionAllowed = true;
            cs.rotationEnabled = true;
            cs.constructedAmount = 1;
            cs.techType = TechType;

            return gameObject;
        }


        /// <summary>
        /// This game uses its own shader system and as such the shaders from UnityEditor do not work and will leave you with a black object unless in direct sunlight.
        /// Note: When copying prefabs from the game itself this is already setup and is only needed when importing new prefabs to the game.
        /// </summary>
        /// <param name="gameObject"></param>
        private static void ApplySubnauticaShaders(GameObject gameObject)
        {
            Shader shader = Shader.Find("MarmosetUBER");
            List<Renderer> Renderers = gameObject.GetComponentsInChildren<Renderer>().ToList();

            foreach (Renderer renderer in Renderers)
            {
                foreach (Material material in renderer.materials)
                {
                    //get the old emission before overwriting the shader
                    Texture emissionTexture = material.GetTexture("_EmissionMap");

                    //overwrites your prefabs shader with the shader system from the game.
                    material.shader = shader;

                    //These enable the item to emit a glow of its own using Subnauticas shader system.
                    material.EnableKeyword("MARMO_EMISSION");
                    material.SetFloat(ShaderPropertyID._EnableGlow, 1f);
                    material.SetTexture(ShaderPropertyID._Illum, emissionTexture);
                    material.SetColor(ShaderPropertyID._GlowColor, new Color(0,0.75f,0,0));
                }
            }

            //This applies the games sky lighting to the object when in the game but also only really works combined with the above code as well.
            SkyApplier skyApplier = gameObject.EnsureComponent<SkyApplier>();
            skyApplier.renderers = Renderers.ToArray();
            skyApplier.anchorSky = Skies.Auto;
        }

        protected override Data GetBlueprintRecipe()
        {
            return new Data() 
            { 
                craftAmount = 1, 
                Ingredients = new List<Ingredient>() 
                { 
                    new Ingredient(TechType.Titanium, 2), 
                    new Ingredient(TechType.EnameledGlass, 1), 
                    new Ingredient(TechType.WiringKit, 1), 
                    new Ingredient(TechType.ComputerChip, 1) 
                }
            };
        }

        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.PowerTransmitter);
        }
    }
}
