using Harmony;
using Oculus.Newtonsoft.Json;
using QModManager.API.ModLoading;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CustomHullPlates
{
    [QModCore]
    public class Main
    {
        private static readonly DirectoryInfo HullPlateFolder = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Posters"));
        private static readonly string ModName = Assembly.GetExecutingAssembly().GetName().Name;

        [QModPatch]
        public static void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            HarmonyInstance.Create($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);

            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "Posters", "Posters", SpriteManager.Get(TechType.PosterKitty));
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "Landscape", "Landscape", SpriteManager.Get(TechType.PosterAurora), "Posters");
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "Portrait", "Portrait", SpriteManager.Get(TechType.PosterExoSuit1), "Posters");
            foreach(string directory in Directory.GetDirectories(HullPlateFolder.FullName))
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
                        Texture2D hullPlateTexture = ImageUtils.LoadTextureFromFile(texture);

                        if(poster != null && icontexture != null && hullPlateTexture != null)
                        {
                            new BasicPostersPrefab(poster.InternalName, poster.DisplayName, poster.Description, poster.Orientation, icontexture, hullPlateTexture).Patch();
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

    public class PosterInfo
    {
        public string InternalName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Orientation { get; set; }
    }
}