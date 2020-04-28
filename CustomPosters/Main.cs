using Harmony;
using Oculus.Newtonsoft.Json;
using QModManager.API.ModLoading;
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
        internal static readonly List<string> customPosters = new List<string>();

        [QModPatch]
        public static void Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            HarmonyInstance.Create($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);

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
    [HarmonyPatch(typeof(Builder), nameof(Builder.TryPlace))]
    public class Builder_TryPlace_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result)
        {
            if(Main.customPosters.Contains(Builder.prefab?.name ?? ""))
            {
                Builder.Initialize();
                if(Builder.prefab == null || !Builder.canPlace)
                {
                    return true;
                }
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Builder.prefab);
                bool flag = false;
                bool flag2 = false;
                SubRoot currentSub = Player.main.GetCurrentSub();
                if(currentSub != null)
                {
                    flag = currentSub.isBase;
                    flag2 = currentSub.isCyclops;
                    gameObject.transform.parent = currentSub.GetModulesRoot();
                }
                else if(Builder.placementTarget != null && Builder.allowedOutside)
                {
                    SubRoot componentInParent2 = Builder.placementTarget.GetComponentInParent<SubRoot>();
                    if(componentInParent2 != null)
                    {
                        gameObject.transform.parent = componentInParent2.GetModulesRoot();
                    }
                }
                Transform transform = gameObject.transform;
                transform.position = Builder.placePosition;
                transform.rotation = Builder.placeRotation;
                Constructable componentInParent3 = gameObject.GetComponentInParent<Constructable>();
                if(!GameModeUtils.RequiresIngredients() || Inventory.main.DestroyItem(TechType.Titanium))
                {
                    ErrorMessage.AddMessage($"{!GameModeUtils.RequiresIngredients()}, {Inventory.main.DestroyItem(TechType.Titanium)}");
                    componentInParent3.SetState(true, true);
                    componentInParent3.SetIsInside(flag || flag2);
                    SkyEnvironmentChanged.Send(gameObject, currentSub);
                    __result = true;
                }
                else
                {
                    UnityEngine.Object.Destroy(gameObject);
                    __result = false;
                }
                if(Builder.ghostModel != null)
                {
                    UnityEngine.Object.Destroy(Builder.ghostModel);
                }
                return false;
            }
            return true;
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