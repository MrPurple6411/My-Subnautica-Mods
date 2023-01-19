namespace BaseKits.Prefabs
{
    using SMLHelper.Assets;
    using SMLHelper.Handlers;
    using SMLHelper.Assets.Interfaces;
    using SMLHelper.Crafting;

    internal class KitFabricator: ICustomFabricator, IBuildable
    {
        internal PrefabInfo PrefabInfo { get; private set; }
        public TechCategory CategoryForPDA => TechCategory.InteriorModule;
        public TechGroup GroupForPDA => TechGroup.InteriorModules;
        public FabricatorModel FabricatorModel => FabricatorModel.Fabricator;
        public CraftTree.Type TreeTypeID { get; private set; }

        public RecipeData RecipeData { get; }

        internal KitFabricator(string KitFab)
        {
            PrefabInfo = PrefabInfo.Create(KitFab).CreateTechType().WithIcon(SpriteManager.Get(TechType.Fabricator))
                .WithLanguageLines("Base Kit Fabricator", "Used to compress Base construction materials into a Construction Kit");
            CraftDataHandler.SetBackgroundType(PrefabInfo.TechType, CraftData.BackgroundType.PlantAir);
            RecipeData = CraftDataHandler.GetRecipeData(TechType.Fabricator);
            TreeTypeID = EnumHandler.AddEntry<CraftTree.Type>(PrefabInfo.ClassID).CreateCraftTreeRoot(out var root).Value;
            root.AddTabNode(Main.RoomsMenu, "Rooms", SpriteManager.Get(TechType.BaseRoom));
            root.AddTabNode(Main.CorridorMenu, "Corridors", SpriteManager.Get(TechType.BaseCorridorX));
            root.AddTabNode(Main.ModuleMenu, "Modules", SpriteManager.Get(TechType.BaseBioReactor));
            root.AddTabNode(Main.UtilityMenu, "Utilities", SpriteManager.Get(TechType.BaseHatch));
            PrefabInfo.RegisterPrefab(this);
        }
    }
}
