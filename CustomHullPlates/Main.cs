

namespace CustomHullPlates;

using System;
using System.IO;
using System.Reflection;
using HullPlate;
using Nautilus.Utility;
using Newtonsoft.Json;
using BepInEx;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.SoftDependency)]
public class Main: BaseUnityPlugin
{
    private static readonly DirectoryInfo HullPlateFolder = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "HullPlates"));
    private static readonly string ModName = Assembly.GetExecutingAssembly().GetName().Name;

    private void Awake()
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
                    new BasicHullPlatePrefab(hullPlate.InternalName, hullPlate.DisplayName, hullPlate.Description, textureFromFile, hullPlateTexture);
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