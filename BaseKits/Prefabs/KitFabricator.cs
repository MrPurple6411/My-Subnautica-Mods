namespace BaseKits.Prefabs
{
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using System.Collections.Generic;
    using UnityEngine;
#if SN1
    using RecipeData = SMLHelper.V2.Crafting.TechData;
    using Sprite = Atlas.Sprite;
#else
    using SMLHelper.V2.Crafting;
#endif


    internal class KitFabricator: CustomFabricator
    {
        private CraftTree craftTree;
        private readonly List<TechType> ClonedRoomKits;
        private readonly List<TechType> ClonedCorridorKits;
        private readonly List<TechType> ClonedModuleKits;
        private readonly List<TechType> ClonedUtilityKits;
        private const string LangFormat = "{0}Menu_{1}";
        private const string SpriteFormat = "{0}_{1}";
        private const string KitFab = "PurpleKitFabricator";

        internal KitFabricator(List<TechType> clonedRoomKits, List<TechType> clonedCorridorKits, List<TechType> clonedModuleKits, List<TechType> clonedUtilityKits) : base("PurpleKitFabricator", "Base Kit Fabricator", "Used to compress Base construction materials into a Construction Kit")
        {
            ClonedRoomKits = clonedRoomKits;
            ClonedCorridorKits = clonedCorridorKits;
            ClonedModuleKits = clonedModuleKits;
            ClonedUtilityKits = clonedUtilityKits;

            OnFinishedPatching += () =>
            {
                Root.CraftTreeCreation += CreateCraftingTree;

                LanguageHandler.SetLanguageLine(string.Format(LangFormat, KitFab, "RoomsMenu"), "Rooms");
                LanguageHandler.SetLanguageLine(string.Format(LangFormat, KitFab, "CorridorMenu"), "Corridors");
                LanguageHandler.SetLanguageLine(string.Format(LangFormat, KitFab, "ModuleMenu"), "Modules");
                LanguageHandler.SetLanguageLine(string.Format(LangFormat, KitFab, "UtilityMenu"), "Utilities");
                SpriteHandler.RegisterSprite(SpriteManager.Group.Category, string.Format(SpriteFormat, KitFab, "RoomsMenu"), SpriteManager.Get(TechType.BaseRoom));
                SpriteHandler.RegisterSprite(SpriteManager.Group.Category, string.Format(SpriteFormat, KitFab, "CorridorMenu"), SpriteManager.Get(TechType.BaseCorridorX));
                SpriteHandler.RegisterSprite(SpriteManager.Group.Category, string.Format(SpriteFormat, KitFab, "ModuleMenu"), SpriteManager.Get(TechType.BaseBioReactor));
                SpriteHandler.RegisterSprite(SpriteManager.Group.Category, string.Format(SpriteFormat, KitFab, "UtilityMenu"), SpriteManager.Get(TechType.BaseHatch));

            };
        }

        private CraftTree CreateCraftingTree()
        {
            if(craftTree != null)
                return craftTree;

            List<CraftNode> roomNodes = new List<CraftNode>();
            List<CraftNode> corridorNodes = new List<CraftNode>();
            List<CraftNode> moduleNodes = new List<CraftNode>();
            List<CraftNode> utilityNodes = new List<CraftNode>();

            foreach(TechType techType in ClonedRoomKits)
                roomNodes.Add(new CraftNode(techType.AsString(), TreeAction.Craft, techType));

            foreach(TechType techType in ClonedCorridorKits)
                corridorNodes.Add(new CraftNode(techType.AsString(), TreeAction.Craft, techType));

            foreach(TechType techType in ClonedModuleKits)
                moduleNodes.Add(new CraftNode(techType.AsString(), TreeAction.Craft, techType));

            foreach(TechType techType in ClonedUtilityKits)
                utilityNodes.Add(new CraftNode(techType.AsString(), TreeAction.Craft, techType));

            craftTree = new CraftTree("PurpleKitFabricator",
                new CraftNode("Root", TreeAction.None, TechType.None).AddNode(new CraftNode[]
                    {
                        new CraftNode("RoomsMenu", TreeAction.Expand, TechType.None).AddNode(roomNodes.ToArray()),
                        new CraftNode("CorridorMenu", TreeAction.Expand, TechType.None).AddNode(corridorNodes.ToArray()),
                        new CraftNode("ModuleMenu", TreeAction.Expand, TechType.None).AddNode(moduleNodes.ToArray()),
                        new CraftNode("UtilityMenu", TreeAction.Expand, TechType.None).AddNode(utilityNodes.ToArray())
                    })
                );

            return craftTree;
        }

        public override Models Model => Models.Fabricator;

        public override bool AllowedInBase => true;

        public override bool AllowedInCyclops => true;

        public override bool AllowedOutside => false;

        public override bool AllowedOnCeiling => false;

        public override bool AllowedOnGround => false;

        public override bool AllowedOnWall => true;

        public override bool RotationEnabled => false;

        public override bool UseCustomTint => true;

        public override Color ColorTint => new Color(0.33f, 0f, 0.33f, 1f);

        public override TechCategory CategoryForPDA => TechCategory.InteriorModule;

        public override TechGroup GroupForPDA => TechGroup.InteriorModules;

        protected override RecipeData GetBlueprintRecipe()
        {
            return CraftDataHandler.GetTechData(TechType.Fabricator);
        }


        protected override Sprite GetItemSprite()
        {
            return SpriteManager.Get(TechType.Fabricator);
        }
    }
}
