using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
using UnityEngine;
#if SUBNAUTICA
using Oculus.Newtonsoft.Json;
#elif BELOWZERO
using Newtonsoft.Json;
#endif

namespace CustomPosters
{
    [QModCore]
    public class Main
    {
        private static readonly DirectoryInfo PosterFolder = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Posters"));
        private static readonly string ModName = Assembly.GetExecutingAssembly().GetName().Name;

        [QModPatch]
        public static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);

            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "Posters", "Posters", SpriteManager.Get(TechType.PosterKitty));
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "Landscape", "Landscape", SpriteManager.Get(TechType.PosterAurora), "Posters");
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "Portrait", "Portrait", SpriteManager.Get(TechType.PosterExoSuit1), "Posters");
            foreach (string directory in Directory.GetDirectories(PosterFolder.FullName))
            {
                string info = Path.Combine(directory, "info.json");
                string icon = Path.Combine(directory, "icon.png");
                string texture = Path.Combine(directory, "texture.png");
                if (File.Exists(info) && File.Exists(icon) && File.Exists(texture))
                {
                    try
                    {
                        PosterInfo poster;
                        using (var reader = new StreamReader(info))
                        {
                            var serializer = new JsonSerializer();
                            poster = serializer.Deserialize(reader, typeof(PosterInfo)) as PosterInfo;
                        }

                        Texture2D icontexture = ImageUtils.LoadTextureFromFile(icon);
                        Texture2D posterTexture = ImageUtils.LoadTextureFromFile(texture);

                        if (poster != null && icontexture != null && posterTexture != null)
                        {
                            var prefab = new BasicPostersPrefab(poster.InternalName, poster.DisplayName, poster.Description, poster.Orientation, icontexture, posterTexture);
                            prefab.Patch();
                            CraftDataHandler.SetQuickSlotType(prefab.TechType, QuickSlotType.Selectable);
                        }
                        else
                        {
                            Console.WriteLine($"[{ModName}] Unable to load Custom Poster from {Path.GetDirectoryName(directory)}!");
                        }

                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"[{ModName}] Unable to load Custom Poster from {Path.GetDirectoryName(directory)}!");
                    }
                }
            }
        }
    }
}