namespace TechPistol.Module
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Utility;
    using SMLHelper.V2.Handlers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UWE;
#if SN1
    using Sprite = Atlas.Sprite;
#endif

    internal class PistolFragmentPrefab: Spawnable
    {

        private static GameObject processedPrefab;
        private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");
        private static readonly int MetallicGlossMap = Shader.PropertyToID("_MetallicGlossMap");
        private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");

        public PistolFragmentPrefab() : base(
            "TechPistolFragment",
            "Damaged Pistol Fragment",
            "Incomplete or Broken fragment of an advanced pistol of unknown origins."
            )
        {
            OnFinishedPatching += () =>
            {
                CoordinatedSpawnsHandler.RegisterCoordinatedSpawns(new List<SpawnInfo>
                {
                    //OreConsumersFrags
                    new SpawnInfo(TechType, new Vector3(-301.1f, -1.5f, 262.5f),
                        new Quaternion(-0.3f, 0.2f, 0.2f, 0.9f)),
                    new SpawnInfo(TechType, new Vector3(-301.1f, -2.5f, 262.5f),
                        new Quaternion(-0.3f, 0.2f, 0.2f, 0.9f)),
                    new SpawnInfo(TechType, new Vector3(-301.1f, -3.5f, 262.5f),
                        new Quaternion(-0.3f, 0.9f, -0.2f, 0.3f)),
                });
            };
        }

        public override WorldEntityInfo EntityInfo => new() { cellLevel = LargeWorldEntity.CellLevel.Medium, classId = ClassID, localScale = Vector3.one, prefabZUp = false, slotType = EntitySlot.Type.Small, techType = TechType };

        public override GameObject GetGameObject()
        {
            if (processedPrefab is not null) return processedPrefab;
            
            var prefab = Main.assetBundle.LoadAsset<GameObject>("TechPistol.prefab");
            var gameObject = Object.Instantiate(prefab);
            gameObject.SetActive(false);
            prefab.SetActive(false);

            gameObject.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            gameObject.transform.localPosition += Vector3.up * 2;

            var componentsInChildren = gameObject.transform.Find("HandGun").gameObject.GetComponentsInChildren<Renderer>();
            foreach(var renderer in componentsInChildren)
            {
                if (!renderer.name.StartsWith("Gun") && !renderer.name.StartsWith("Target")) continue;
                var emissionMap = renderer.material.GetTexture(EmissionMap);
                var specMap = renderer.material.GetTexture(MetallicGlossMap);

                renderer.material.shader = Shader.Find("MarmosetUBER");
                renderer.material.EnableKeyword("MARMO_EMISSION");
                renderer.material.EnableKeyword("MARMO_SPECMAP");
                renderer.material.SetTexture(ShaderPropertyID._Illum, emissionMap);
                renderer.material.SetTexture(ShaderPropertyID._SpecTex, specMap);
                renderer.material.SetColor(GlowColor, new Color(1f, 1f, 1f));
                renderer.material.SetFloat(ShaderPropertyID._GlowStrength, 1f);
                renderer.material.SetFloat(ShaderPropertyID._GlowStrengthNight, 1f);
            }

            Object.Destroy(gameObject.transform.Find(PistolBehaviour.GunMain + "/ModeChange")?.gameObject);
            Object.Destroy(gameObject.transform.Find(PistolBehaviour.Point)?.gameObject);
            Object.Destroy(gameObject.GetComponent<PistolBehaviour>());
            Object.Destroy(gameObject.GetComponent<EnergyMixin>());
            Object.Destroy(gameObject.GetComponent<VFXFabricating>());

            var prefabIdentifier = gameObject.GetComponent<PrefabIdentifier>();
            prefabIdentifier.ClassId = ClassID;
            gameObject.GetComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.VeryFar;
            gameObject.GetComponent<TechTag>().type = TechType;

            var pickupable = gameObject.GetComponent<Pickupable>();
            pickupable.isPickupable = false;

            var resourceTracker = gameObject.EnsureComponent<ResourceTracker>();
            resourceTracker.prefabIdentifier = prefabIdentifier;
            resourceTracker.techType = TechType;
            resourceTracker.overrideTechType = TechType.Fragment;
            resourceTracker.rb = gameObject.GetComponent<Rigidbody>();
            resourceTracker.rb.isKinematic = true;
            resourceTracker.pickupable = pickupable;

            processedPrefab = gameObject;
            return processedPrefab;
        }

        public override IEnumerator GetGameObjectAsync(IOut<GameObject> pistolFragment)
        {
            pistolFragment.Set(GetGameObject());
            yield break;
        }

        protected override Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromTexture(Main.assetBundle.LoadAsset<Texture2D>("Icon"));
        }
    }
}
