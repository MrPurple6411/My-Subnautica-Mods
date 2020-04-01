using Harmony;
using Oculus.Newtonsoft.Json;
using QModManager.API.ModLoading;
using System;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;
using System.IO;

namespace SimpleRecipeRemoval
{
    [QModCore]
    public class Core
    {
        internal static Assembly assembly = Assembly.GetExecutingAssembly();
        internal static string assemblyName = assembly.GetName().Name;
        internal static Dictionary<TechCategory, List<TechType>> BlueprintHideList;
        internal static bool ConfigLoaded;
        internal static string ModID = $"MrPurple6411_{assemblyName}";
        internal static List<string> RecipeBlacklist;
        internal static List<string> TechCategoryHidelist;
        internal static List<string> TechTypeHidelist;
        internal static bool DebugCheck => File.Exists(Path.GetDirectoryName(Assembly.GetAssembly(typeof(Core)).Location) + "/Debug.txt");

        public static List<string> LoadConfig(string path)
        {
            string path2 = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Core)).Location) + "/" + path;

            List<string> strings = new List<string>();
            if(File.Exists(path2))
            {
                try
                {
                    using(StreamReader reader = new StreamReader(path2))
                    {
                        strings = JsonConvert.DeserializeObject<List<string>>(reader.ReadToEnd());
                    }
                }
                catch(Exception)
                {
                    Log($"[{ModID}] Failed to load file {path}", true);
                    return null;
                }
            }
            else
            {
                Log($"[{ModID}] Unable to find file {path} Creating Empty {path}", true);
                using(StreamWriter writer = new StreamWriter(path2))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(strings, Formatting.Indented));
                }
            }

            Log($"[{ModID}] Loading {path}" + Environment.NewLine + JsonConvert.SerializeObject(strings, Formatting.Indented));

            return strings;
        }

        public static void Log(string msg, bool showOnScreen = false)
        {
            if(DebugCheck)
            {
                Console.WriteLine(msg);
                if(showOnScreen)
                    ErrorMessage.AddDebug(msg);
            }
        }

        [QModPatch]
        public static void Patch()
        {
            try
            {
                if(ConfigLoaded)
                {
                    HarmonyInstance.Create(ModID).PatchAll(assembly);
                    Console.WriteLine($"[{ModID}] Patching Complete");
                }
                else
                {
                    Console.WriteLine($"[{ModID}] Failed to load Config files Bypassing Patching");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(Environment.NewLine + $"[{ModID}] Patching Failed with exception");
                Console.WriteLine(Environment.NewLine + e.ToString() + Environment.NewLine + Environment.NewLine);
            }
        }

        [QModPrePatch]
        public static void PrePatch()
        {
            ConfigLoaded = LoadConfigs();
        }

        private static bool LoadConfigs()
        {
            RecipeBlacklist = LoadConfig("RecipeBlacklist.json");
            TechTypeHidelist = LoadConfig("TypeHidelist.json");
            TechCategoryHidelist = LoadConfig("CategoryHidelist.json");

            if(RecipeBlacklist != null && TechTypeHidelist != null && TechCategoryHidelist != null)
                return true;
            else
                return false;
        }
    }

    [HarmonyPatch(typeof(uGUI_BlueprintsTab), nameof(uGUI_BlueprintsTab.UpdateEntries))]
    internal class BlueprintsTab_UpdateEntries
    {
        private static Dictionary<TechCategory, List<TechType>> GenerateDictionary(uGUI_BlueprintsTab instance, string path = "")
        {
            Dictionary<string, List<string>> defaults = new Dictionary<string, List<string>>();
            Dictionary<TechCategory, List<TechType>> dictionary = new Dictionary<TechCategory, List<TechType>>();
            foreach(TechCategory techCategory in instance.entries.Keys)
            {
                defaults[techCategory.ToString()] = new List<string>();
                dictionary[techCategory] = new List<TechType>();
                foreach(TechType techType in instance.entries[techCategory].entries.Keys)
                {
                    defaults[techCategory.ToString()].Add(techType.AsString());
                    if(Core.TechTypeHidelist.Contains(techType.AsString()) || Core.RecipeBlacklist.Contains(techType.AsString()) || Core.TechCategoryHidelist.Contains(techCategory.ToString()))
                        dictionary[techCategory].Add(techType);
                }
                if(defaults[techCategory.ToString()].Count == 0)
                    defaults.Remove(techCategory.ToString());
                if(dictionary[techCategory].Count == 0)
                    dictionary.Remove(techCategory);
            }

            if(path != "")
            {
                using(StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(defaults, Formatting.Indented));
                }
            }
            defaults.Clear();
            return dictionary;
        }

        private static void GenerateList(uGUI_BlueprintsTab instance)
        {
            string path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Core)).Location) + "/Defaults.Json";
            if(!File.Exists(path))
            {
                Core.BlueprintHideList = GenerateDictionary(instance, path);
            }
            if(Core.BlueprintHideList == null)
                Core.BlueprintHideList = GenerateDictionary(instance);
        }

        [HarmonyPostfix]
        private static void Postfix(uGUI_BlueprintsTab __instance)
        {
            if(Core.BlueprintHideList == null)
                GenerateList(__instance);

            foreach(KeyValuePair<TechCategory, List<TechType>> keyValuePair in Core.BlueprintHideList)
            {
                foreach(TechType techType in keyValuePair.Value)
                {
                    if(__instance.entries.ContainsKey(keyValuePair.Key) && __instance.entries[keyValuePair.Key].entries.ContainsKey(techType))
                    {
                        uGUI_BlueprintsTab.CategoryEntry categoryEntry = __instance.entries[keyValuePair.Key];
                        uGUI_BlueprintEntry entry = __instance.entries[keyValuePair.Key].entries[techType];

                        NotificationManager.main.UnregisterTarget(entry);
                        categoryEntry.entries.Remove(techType);
                        Object.Destroy(entry.gameObject);
                        if(categoryEntry.entries.Count == 0)
                        {
                            Object.Destroy(categoryEntry.title.gameObject);
                            Object.Destroy(categoryEntry.canvas.gameObject);
                            __instance.entries.Remove(keyValuePair.Key);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(uGUI_BuilderMenu), nameof(uGUI_BuilderMenu.UpdateItems))]
    internal class BuilderMenu_UpdateItems
    {
        [HarmonyPrefix]
        private static bool Prefix(uGUI_BuilderMenu __instance)
        {
            __instance.iconGrid.Clear();
            __instance.items.Clear();

            List<TechType> techTypesForGroup = __instance.GetTechTypesForGroup(__instance.selected);
            int num = 0;

            for(int i = 0; i < techTypesForGroup.Count; i++)
            {
                TechType techType = techTypesForGroup[i];
                if(!Core.RecipeBlacklist.Contains(techType.AsString()))
                {
                    TechUnlockState techUnlockState = KnownTech.GetTechUnlockState(techType);
                    if(techUnlockState == TechUnlockState.Available || techUnlockState == TechUnlockState.Locked)
                    {
                        string stringForInt = IntStringCache.GetStringForInt(num);
                        __instance.items.Add(stringForInt, techType);
                        __instance.iconGrid.AddItem(stringForInt, SpriteManager.Get(techType), SpriteManager.GetBackground(techType), techUnlockState == TechUnlockState.Locked, num);
                        __instance.iconGrid.RegisterNotificationTarget(stringForInt, NotificationManager.Group.Builder, techType.EncodeKey());
                        num++;
                    }
                }
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(CraftTree), nameof(CraftTree.AddToCraftableTech), new Type[] { typeof(CraftTree) })]
    internal class CraftNode_AddToCraftableTech
    {
        [HarmonyPrefix]
        private static bool Prefix(CraftTree tree)
        {
            using(IEnumerator<CraftNode> enumerator = tree.nodes.Traverse(false))
            {
                while(enumerator.MoveNext())
                {
                    CraftNode craftNode = enumerator.Current;
                    if(craftNode.action == TreeAction.Craft)
                    {
                        TechType techType = craftNode.techType0;
                        if(techType != TechType.None && !Core.RecipeBlacklist.Contains(techType.AsString()))
                        {
                            CraftTree.craftableTech.Add(techType);
                        }
                        else if(Core.RecipeBlacklist.Contains(techType.AsString()))
                        {
                            craftNode.techType0 = TechType.None;
                            Core.Log($"TechType: {techType.AsString()} found in BlackList", true);
                        }
                    }
                }
            }
            return false;
        }
    }
}