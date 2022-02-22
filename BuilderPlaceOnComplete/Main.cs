using BepInEx;

namespace BuilderPlaceOnComplete
{
    using HarmonyLib;
    using System.Reflection;

    public  class Main:BaseUnityPlugin
    {
        public  void Awak()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
        }
    }
}