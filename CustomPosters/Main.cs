namespace CustomPosters
{
    using System;
    using System.IO;
    using System.Reflection;
    using SMLHelper.Handlers;
    using SMLHelper.Utility;
    using Poster;
    using UWE;
    using System.Collections;
    using UnityEngine;
    using Newtonsoft.Json;
    using BepInEx;
    
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main: BaseUnityPlugin
    {
        private static readonly DirectoryInfo PosterFolder = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "Posters"));
        private static readonly string ModName = Assembly.GetExecutingAssembly().GetName().Name;

        #region[Declarations]

        public const string
            MODNAME = "CustomPosters",
            AUTHOR = "MrPurple6411",
            GUID = AUTHOR + "_" + MODNAME,
            VERSION = "1.0.0.0";

        #endregion

        private void Awake()
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