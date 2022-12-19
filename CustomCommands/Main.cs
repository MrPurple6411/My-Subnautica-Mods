namespace CustomCommands
{
    using MonoBehaviours;
    using QModManager.API.ModLoading;

    [QModCore]
    public static class Main
    {

        [QModPatch]
        public static void Load()
        {
            Placeholder.Awake();
        }
    }
}