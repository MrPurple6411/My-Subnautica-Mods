using System;
using System.IO;
using System.Reflection;
using QModManager.API.ModLoading;
using SMLHelper.V2.Utility;
using UnityEngine;
#if SUBNAUTICA
    using Oculus.Newtonsoft.Json;
#elif BELOWZERO
using Newtonsoft.Json;
#endif

namespace CustomHullPlates
{
    [QModCore]
    public class Main
    {
        private static readonly DirectoryInfo HullPlateFolder = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "HullPlates"));
        private static readonly string ModName = Assembly.GetExecutingAssembly().GetName().Name;

        [QModPatch]
        public static void Load()
        {
            foreach (string directory in Directory.GetDirectories(HullPlateFolder.FullName))
            {
                string info = Path.Combine(directory, "info.json");
                string icon = Path.Combine(directory, "icon.png");
                string texture = Path.Combine(directory, "texture.png");
                if (File.Exists(info) && File.Exists(icon) && File.Exists(texture))
                {
                    try
                    {
                        HullPlateInfo hullPlate;
                        using (var reader = new StreamReader(info))
                        {
                            var serializer = new JsonSerializer();
                            hullPlate = serializer.Deserialize(reader, typeof(HullPlateInfo)) as HullPlateInfo;
                        }

                        Texture2D icontexture = ImageUtils.LoadTextureFromFile(icon);
                        Texture2D hullPlateTexture = ImageUtils.LoadTextureFromFile(texture);

                        if (hullPlate != null && icontexture != null && hullPlateTexture != null)
                        {
                            new BasicHullPlatePrefab(hullPlate.InternalName, hullPlate.DisplayName, hullPlate.Description, icontexture, hullPlateTexture).Patch();
                        }
                        else
                        {
                            Console.WriteLine($"[{ModName}] Unable to load Custom Hull Plate from {Path.GetDirectoryName(directory)}!");
                        }

                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"[{ModName}] Unable to load Custom Hull Plate from {Path.GetDirectoryName(directory)}!");
                    }
                }
            }
        }
    }
    public class HullPlateInfo
    {
        public string InternalName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}