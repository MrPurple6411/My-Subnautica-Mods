using BepInEx;

namespace CreaturesFleeLess
{
    using HarmonyLib;
    using System.Reflection;

    public class Main:BaseUnityPlugin
    {
        public  void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Harmony.CreateAndPatchAll(assembly, $"MrPurple6411_{assembly.GetName().Name}");
        }
    }
}