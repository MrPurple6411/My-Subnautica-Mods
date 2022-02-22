using BepInEx;

namespace NoMask
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