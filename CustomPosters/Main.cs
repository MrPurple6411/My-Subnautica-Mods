using BepInEx;

namespace CustomPosters
{
    using System;
    using System.IO;
    using System.Reflection;
    using SMCLib.Handlers;
    using SMCLib.Utility;
    using Poster;
#if SUBNAUTICA_STABLE
    using Oculus.Newtonsoft.Json;
#elif  SUBNAUTICA_EXP
    using Newtonsoft.Json;
#elif  BZ
    using UWE;
    using System.Collections;
    using UnityEngine;
    using Newtonsoft.Json;
#endif

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("SMCLib", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
        private static readonly DirectoryInfo PosterFolder = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Posters"));
        private static readonly string ModName = Assembly.GetExecutingAssembly().GetName().Name;

        public void Start()
        {
#if SN1
            CreateTabsAndLoadFiles();
#elif BZ
            CoroutineHost.StartCoroutine(WaitForSpriteManager());
#endif
        }

#if BZ
        private static IEnumerator WaitForSpriteManager()
        {
            while (!SpriteManager.hasInitialized)
                yield return new WaitForSecondsRealtime(1);

            CreateTabsAndLoadFiles();
        }
#endif

        private static void CreateTabsAndLoadFiles()
        {
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "Posters", "Posters", SpriteManager.Get(TechType.PosterKitty));
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "Landscape", "Landscape", SpriteManager.Get(TechType.PosterAurora), "Posters");
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "Portrait", "Portrait", SpriteManager.Get(TechType.PosterExoSuit1), "Posters");

            foreach(var directory in Directory.GetDirectories(PosterFolder.FullName))
            {
                var info = Path.Combine(directory, "info.json");
                var icon = Path.Combine(directory, "icon.png");
                var texture = Path.Combine(directory, "texture.png");
                if(File.Exists(info) && File.Exists(icon) && File.Exists(texture))
                {
                    try
                    {
                        PosterInfo poster;
                        using(var reader = new StreamReader(info))
                        {
                            var serializer = new JsonSerializer();
                            poster = serializer.Deserialize(reader, typeof(PosterInfo)) as PosterInfo;
                        }

                        var iconTexture = ImageUtils.LoadTextureFromFile(icon);
                        var posterTexture = ImageUtils.LoadTextureFromFile(texture);

                        if(poster != null && iconTexture != null && posterTexture != null)
                        {
                            var prefab = new BasicPostersPrefab(poster.InternalName, poster.DisplayName, poster.Description, poster.Orientation, iconTexture, posterTexture);
                            prefab.Patch();
                        }
                        else
                        {
                            Console.WriteLine($"[{ModName}] Unable to load Custom Poster from {Path.GetDirectoryName(directory)}!");
                        }

                    }
                    catch(Exception)
                    {
                        Console.WriteLine($"[{ModName}] Unable to load Custom Poster from {Path.GetDirectoryName(directory)}!");
                    }
                }
            }
        }
    }
}