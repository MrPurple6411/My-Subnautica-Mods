

namespace CustomHullPlates
{
    using System;
    using System.IO;
    using System.Reflection;
    using HullPlate;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Utility;
#if SUBNAUTICA_STABLE
    using Oculus.Newtonsoft.Json;
#else
    using Newtonsoft.Json;
#endif

    [QModCore]
    public class Main
    {
        private static readonly DirectoryInfo HullPlateFolder = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "HullPlates"));
        private static readonly string ModName = Assembly.GetExecutingAssembly().GetName().Name;

        [QModPatch]
        public static void Load()
        {
            foreach(var directory in Directory.GetDirectories(HullPlateFolder.FullName))
            {
                var info = Path.Combine(directory, "info.json");
                var icon = Path.Combine(directory, "icon.png");
                var texture = Path.Combine(directory, "texture.png");
                if (!File.Exists(info) || !File.Exists(icon) || !File.Exists(texture)) continue;
                try
                {
                    HullPlateInfo hullPlate;
                    using(var reader = new StreamReader(info))
                    {
                        var serializer = new JsonSerializer();
                        hullPlate = serializer.Deserialize(reader, typeof(HullPlateInfo)) as HullPlateInfo;
                    }

                    var textureFromFile = ImageUtils.LoadTextureFromFile(icon);
                    var hullPlateTexture = ImageUtils.LoadTextureFromFile(texture);

                    if(hullPlate != null && textureFromFile != null && hullPlateTexture != null)
                    {
                        new BasicHullPlatePrefab(hullPlate.InternalName, hullPlate.DisplayName, hullPlate.Description, textureFromFile, hullPlateTexture).Patch();
                    }
                    else
                    {
                        Console.WriteLine($"[{ModName}] Unable to load Custom Hull Plate from {Path.GetDirectoryName(directory)}!");
                    }

                }
                catch(Exception)
                {
                    Console.WriteLine($"[{ModName}] Unable to load Custom Hull Plate from {Path.GetDirectoryName(directory)}!");
                }
            }
        }
    }
}