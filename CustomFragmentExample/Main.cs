using QModManager.API.ModLoading;

namespace CustomFragmentExample
{
    [QModCore]
    public class Main
    {
        [QModPatch]
        public static void Load()
        {
            FragmentPrefab Fragment = new FragmentPrefab("MrPurpleFragment", "MrPurple6411 Example Fragment", "This is a fragment that will spawn out in the game", prefab: CraftData.GetPrefabForTechType(TechType.ExosuitFragment, false), techToCopy: TechType.ScrapMetal);
            Fragment.Patch();

            Exosuite2 exosuit2 = new Exosuite2("PurpleUnlockable", "Exampe Unlockable", "This is a basic example unlockable item", techToCopy: TechType.Exosuit, unlockedBy: Fragment.TechType);
            exosuit2.Patch();

        }
    }
}