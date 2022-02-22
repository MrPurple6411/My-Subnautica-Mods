using BepInEx;

namespace Keep_Inventory_On_Death
{
    using HarmonyLib;
    using System.Reflection;

    public class Main:BaseUnityPlugin
    {
        internal static Assembly assembly = Assembly.GetExecutingAssembly();

        public void  Awake()
        {
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}