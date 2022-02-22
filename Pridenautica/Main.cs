using BepInEx;

namespace Pridenautica
{
    using HarmonyLib;
    using System.Reflection;

    public class Main:BaseUnityPlugin
    {
        public void  Awake()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}