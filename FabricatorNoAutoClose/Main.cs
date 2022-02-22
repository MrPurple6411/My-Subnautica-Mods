using BepInEx;

namespace FabricatorNoAutoClose
{
    using HarmonyLib;
    using System.Reflection;

    public class Main:BaseUnityPlugin
    {
        public void  Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}