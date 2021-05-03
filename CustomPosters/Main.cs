namespace CustomPosters
{
    using System;
    using System.IO;
    using System.Reflection;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Utility;
    using UnityEngine;
    using CustomPosters.Poster;
#if SUBNAUTICA_STABLE
    using Oculus.Newtonsoft.Json;
#else
    using Newtonsoft.Json;
    using UWE;
    using System.Collections;
#endif

    [QModCore]
    public class Main
    {
        private static readonly DirectoryInfo PosterFolder = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Posters"));
        private static readonly string ModName = Assembly.GetExecutingAssembly().GetName().Name;

        [QModPatch]
        public static void Load()
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
            yield break;
        }
#endif

        private static void CreateTabsAndLoadFiles()
        {
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "Posters", "Posters", SpriteManager.Get(TechType.PosterKitty));
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "Landscape", "Landscape", SpriteManager.Get(TechType.PosterAurora), "Posters");
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "Portrait", "Portrait", SpriteManager.Get(TechType.PosterExoSuit1), "Posters");

            foreach(string directory in Directory.GetDirectories(PosterFolder.FullName))
            {
                string info = Path.Combine(directory, "info.json");
                string icon = Path.Combine(directory, "icon.png");
                string texture = Path.Combine(directory, "texture.png");
                if(File.Exists(info) && File.Exists(icon) && File.Exists(texture))
                {
                    try
                    {
                        PosterInfo poster;
                        using(StreamReader reader = new StreamReader(info))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            poster = serializer.Deserialize(reader, typeof(PosterInfo)) as PosterInfo;
                        }

                        Texture2D icontexture = ImageUtils.LoadTextureFromFile(icon);
                        Texture2D posterTexture = ImageUtils.LoadTextureFromFile(texture);

                        if(poster != null && icontexture != null && posterTexture != null)
                        {
                            BasicPostersPrefab prefab = new BasicPostersPrefab(poster.InternalName, poster.DisplayName, poster.Description, poster.Orientation, icontexture, posterTexture);
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