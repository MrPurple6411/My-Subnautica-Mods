namespace CustomPosters;

using System;
using System.IO;
using System.Reflection;
using Nautilus.Handlers;
using Nautilus.Utility;
using Poster;
using UWE;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json;

using BepInEx;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(Nautilus.PluginInfo.PLUGIN_GUID, Nautilus.PluginInfo.PLUGIN_VERSION)]
public class Main: BaseUnityPlugin
{
    private static readonly DirectoryInfo _posterFolder = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Posters"));

    internal void Awake()
    {
#if SUBNAUTICA
        CreateTabsAndLoadFiles();
#elif BELOWZERO
        CoroutineHost.StartCoroutine(WaitForSpriteManager());
#endif
    }

#if BELOWZERO
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

        foreach(var directory in Directory.GetDirectories(_posterFolder.FullName))
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
                        new BasicPostersPrefab(poster.InternalName, poster.DisplayName, poster.Description, poster.Orientation, iconTexture, posterTexture);
					}
                    else
                    {
                        Console.WriteLine($"[{MyPluginInfo.PLUGIN_NAME}] Unable to load Custom Poster from {Path.GetDirectoryName(directory)}!");
                    }
                }
                catch(Exception)
                {
                    Console.WriteLine($"[{MyPluginInfo.PLUGIN_NAME}] Unable to load Custom Poster from {Path.GetDirectoryName(directory)}!");
                }
            }
        }
    }
}