using Harmony;
using Oculus.Newtonsoft.Json;
using Oculus.Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UWE;

namespace UnkownName
{
    [HarmonyPatch(typeof(MainMenuRightSide), nameof(MainMenuRightSide.Awake))]
    public class UnknownNameConfig
    {
        public static readonly string config = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json");
        public static string UnkownLabel = "";
        public static string UnkownTitle = "Unanalyzed Item";
        public static string UnkownDescription = "This item has not been analyzed yet. \nTo discover its usefulness please scan it at your earliest convenience.";
        public static bool ScanOnPickup = false;
        public static bool Hardcore = false;

        [HarmonyPostfix]
        public static void Postfix()
        {
            if(File.Exists(config))
            {
                using(StreamReader reader = new StreamReader(config))
                {
                    JObject json = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
                    if(json["UnkownLabel"] != null)
                    {
                        UnkownLabel = json["UnkownLabel"].ToString();
                    }
                    if(json["UnkownTitle"] != null)
                    {
                        UnkownTitle = json["UnkownTitle"].ToString();
                    }
                    if(json["UnkownDescription"] != null)
                    {
                        UnkownDescription = json["UnkownDescription"].ToString();
                    }
                    if(json["ScanOnPickup"] != null)
                    {
                        ScanOnPickup = json["ScanOnPickup"].ToString().ToLower() == "true";
                    }
                    if(json["Hardcore"] != null)
                    {
                        Hardcore = json["Hardcore"].ToString().ToLower() == "true";
                    }
                }
            }
            else
            {
                using(StreamWriter writer = new StreamWriter(config))
                {
                    JObject json = new JObject
                    {
                        ["UnkownLabel"] = UnkownLabel,
                        ["UnkownTitle"] = UnkownTitle,
                        ["UnkownDescription"] = UnkownDescription,
                        ["ScanOnPickup"] = ScanOnPickup,
                        ["Hardcore"] = Hardcore
                    };
                    writer.Write(json.ToString());
                }
            }
        }
    }

    [HarmonyPatch(typeof(CrafterLogic), nameof(CrafterLogic.IsCraftRecipeUnlocked))]
    public class CraftNode_SetVisable
    {
        [HarmonyPostfix]
        public static void Postfix(TechType techType, ref bool __result)
        {
            if(UnknownNameConfig.Hardcore && GameModeUtils.RequiresBlueprints() && __result)
            {
#if SUBNAUTICA
                ITechData data = CraftData.Get(techType, true);
                if(data != null)
                {
                    int ingredientCount = data.ingredientCount;
                    for(int i = 0; i < ingredientCount; i++)
                    {
                        IIngredient ingredient = data.GetIngredient(i);
                        if(!CrafterLogic.IsCraftRecipeUnlocked(ingredient.techType))
                        {
                            __result = false;
                            return;
                        }
                    }
                }
#elif BELOWZERO
                List<Ingredient> data = TechData.GetIngredients(techType)?.ToList() ?? new List<Ingredient>();
                int ingredientCount = data.Count;
                for(int i = 0; i < ingredientCount; i++)
                {
                    Ingredient ingredient = data[i];
                    if(!CrafterLogic.IsCraftRecipeUnlocked(ingredient.techType))
                    {
                        __result = false;
                        return;
                    }
                }
#endif
            }
        }
    }

    [HarmonyPatch(typeof(IntroVignette), nameof(IntroVignette.ShouldPlayIntro))]
    public class IntroVignette_ShouldPlayIntro
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            if(UnknownNameConfig.Hardcore)
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Start))]
    public class Player_FirstStart
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            if(File.Exists(UnknownNameConfig.config))
            {
                using(StreamReader reader = new StreamReader(UnknownNameConfig.config))
                {
                    JObject json = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
                    if(json["UnkownLabel"] != null)
                    {
                        UnknownNameConfig.UnkownLabel = json["UnkownLabel"].ToString();
                    }
                    if(json["UnkownTitle"] != null)
                    {
                        UnknownNameConfig.UnkownTitle = json["UnkownTitle"].ToString();
                    }
                    if(json["UnkownDescription"] != null)
                    {
                        UnknownNameConfig.UnkownDescription = json["UnkownDescription"].ToString();
                    }
                    if(json["ScanOnPickup"] != null)
                    {
                        UnknownNameConfig.ScanOnPickup = json["ScanOnPickup"].ToString().ToLower() == "true";
                    }
                    if(json["Hardcore"] != null)
                    {
                        UnknownNameConfig.Hardcore = json["Hardcore"].ToString().ToLower() == "true";
                    }
                }
            }
            if(UnknownNameConfig.Hardcore)
            {
                GameModeUtils.ActivateCheat(GameModeOption.NoHints);
            }
            if(UnknownNameConfig.Hardcore && !Utils.GetContinueMode())
            {
                Pickupable pickupable = CraftData.InstantiateFromPrefab(TechType.Scanner).GetComponent<Pickupable>();
                ScannerTool scannerTool = pickupable.GetComponent<ScannerTool>();
                Pickupable pickupable1 = CraftData.InstantiateFromPrefab(TechType.Battery).GetComponent<Pickupable>();
                scannerTool.energyMixin.batterySlot.AddItem(pickupable1);
                Inventory.main.container.AddItem(pickupable);
            }
        }
    }

    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.WriteIngredients))]
    public class TooltipFactory_WriteIngredients
    {
#if SUBNAUTICA

        [HarmonyPostfix]
        public static void Postfix(ITechData data, ref List<TooltipIcon> icons)
        {
            int ingredientCount = data.ingredientCount;
            for(int i = 0; i < ingredientCount; i++)
            {
                IIngredient ingredient = data.GetIngredient(i);
                TechType techType = ingredient.techType;
                if(!KnownTech.Contains(techType) && PDAScanner.ContainsCompleteEntry(techType))
                {
                    KnownTech.Add(techType);
                }
                if(!KnownTech.Contains(techType) && GameModeUtils.RequiresBlueprints())
                {
                    TooltipIcon icon = icons.Find((TooltipIcon) => TooltipIcon.sprite == SpriteManager.Get(techType) && TooltipIcon.text.Contains(Language.main.GetOrFallback(TooltipFactory.techTypeIngredientStrings.Get(techType), techType)));
                    if(icons.Contains(icon))
                    {
                        icons.Remove(icon);
                        TooltipIcon tooltipIcon = new TooltipIcon() { sprite = SpriteManager.Get(TechType.None), text = UnknownNameConfig.UnkownTitle };
                        icons.Add(tooltipIcon);
                    }
                }
            }
        }

#elif BELOWZERO

        [HarmonyPostfix]
        public static void Postfix(IList<Ingredient> ingredients, ref List<TooltipIcon> icons)
        {
            int ingredientCount = ingredients.Count;
            for(int i = 0; i < ingredientCount; i++)
            {
                TechType techType = ingredients[i].techType;
                if(!KnownTech.Contains(techType) && PDAScanner.ContainsCompleteEntry(techType))
                {
                    KnownTech.Add(techType, true);
                }
                if(!KnownTech.Contains(techType) && GameModeUtils.RequiresBlueprints())
                {
                    TooltipIcon icon = icons.Find((TooltipIcon) => TooltipIcon.sprite == SpriteManager.Get(techType) && TooltipIcon.text.Contains(Language.main.GetOrFallback(TooltipFactory.techTypeIngredientStrings.Get(techType), techType)));
                    if(icons.Contains(icon))
                    {
                        icons.Remove(icon);
                        TooltipIcon tooltipIcon = new TooltipIcon() { sprite = SpriteManager.Get(TechType.None), text = UnknownNameConfig.UnkownTitle };
                        icons.Add(tooltipIcon);
                    }
                }
            }
        }

#endif
    }

    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.Recipe))]
    public class TooltipFactory_Recipe
    {
        [HarmonyPostfix]
        public static void Postfix(bool locked, ref string tooltipText)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if(locked && GameModeUtils.RequiresBlueprints())
            {
                TooltipFactory.WriteTitle(stringBuilder, UnknownNameConfig.UnkownTitle);
                TooltipFactory.WriteDescription(stringBuilder, UnknownNameConfig.UnkownDescription);
                tooltipText = stringBuilder.ToString();
            }
        }
    }

    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.ItemCommons))]
    public class TooltipFactory_InventoryItem
    {
        [HarmonyPostfix]
        public static void Postfix(ref StringBuilder sb, TechType techType, GameObject obj)
        {
            if(!PDAScanner.CanScan(obj) || PDAScanner.ContainsCompleteEntry(techType) || KnownTech.Contains(techType))
            {
                return;
            }
            sb.Clear();
            TooltipFactory.WriteTitle(sb, UnknownNameConfig.UnkownTitle);
            TooltipFactory.WriteDescription(sb, UnknownNameConfig.UnkownDescription);
        }
    }

    [HarmonyPatch(typeof(LanguageCache), nameof(LanguageCache.GetPickupText))]
    public class LanguageCache_GetPickupText
    {
        [HarmonyPostfix]
        public static void Postfix(ref string __result, TechType techType)
        {
            if(!KnownTech.Contains(techType) && GameModeUtils.RequiresBlueprints())
            {
                __result = UnknownNameConfig.UnkownLabel;
            }
        }
    }

    [HarmonyPatch(typeof(ScannerTool), nameof(ScannerTool.OnHover))]
    public class ScannerTool_OnHover
    {
        [HarmonyPostfix]
        public static void Postfix(ScannerTool __instance)
        {
            PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
            PDAScanner.Result result = PDAScanner.CanScan();
            PDAScanner.EntryData entryData = PDAScanner.GetEntryData(PDAScanner.scanTarget.techType);

            if((entryData != null && KnownTech.Contains(entryData.blueprint)) || PDAScanner.ContainsCompleteEntry(scanTarget.techType) || __instance.energyMixin.charge <= 0f || !scanTarget.isValid || result != PDAScanner.Result.Scan || !GameModeUtils.RequiresBlueprints())
            {
                return;
            }
#if SUBNAUTICA
            HandReticle.main.SetInteractText(UnknownNameConfig.UnkownLabel, false, HandReticle.Hand.None);
#elif BELOWZERO
            HandReticle.main.SetText(HandReticle.TextType.Hand, UnknownNameConfig.UnkownLabel, true, GameInput.Button.None);
#endif
        }

    }

    [HarmonyPatch(typeof(Inventory), nameof(Inventory.Pickup))]
    public class Inventory_Pickup
    {
        [HarmonyPostfix]
        public static void Postfix(Pickupable pickupable)
        {
            TechType techType = pickupable.GetTechType();
            PDAScanner.EntryData entryData = PDAScanner.GetEntryData(techType);
            GameObject gameObject = pickupable.gameObject;
            if(UnknownNameConfig.ScanOnPickup && Inventory.main.container.Contains(TechType.Scanner))
            {
                bool flag6 = false;
                bool flag7 = false;
                if(entryData != null)
                {
                    flag7 = true;
                    if(!PDAScanner.GetPartialEntryByKey(techType, out PDAScanner.Entry entry))
                    {
                        entry = PDAScanner.Add(techType, 0);
                    }
                    if(entry != null)
                    {
                        flag6 = true;
                        PDAScanner.partial.Remove(entry);
                        PDAScanner.complete.Add(entry.techType);
                        PDAScanner.NotifyRemove(entry);
                    }
                }
                if(gameObject != null)
                {
                    gameObject.SendMessage("OnScanned", null, SendMessageOptions.DontRequireReceiver);
                }
                ResourceTracker.UpdateFragments();
                if(flag6 || flag7)
                {
                    PDAScanner.Unlock(entryData, flag6, flag7, true);
                    KnownTech.Add(techType, true);
                }
            }
        }
    }

    [HarmonyPatch(typeof(KnownTech), nameof(KnownTech.Analyze))]
    public class KnownTech_Analyze
    {
        [HarmonyPrefix]
        public static bool Prefix(TechType techType)
        {
            PDAScanner.EntryData entryData = PDAScanner.GetEntryData(techType);
            return techType == TechType.None
                ? true
                : entryData == null
                ? true
                : PDAScanner.ContainsCompleteEntry(techType)
                ? true
                : string.IsNullOrEmpty(entryData.encyclopedia) || PDAEncyclopedia.ContainsEntry(entryData.encyclopedia);
        }
    }

    [HarmonyPatch(typeof(GUIHand), nameof(GUIHand.OnUpdate))]
    public class GUIHand_OnUpdate
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
            PDAScanner.EntryData entryData = PDAScanner.GetEntryData(scanTarget.techType);

            if((entryData != null && KnownTech.Contains(entryData.blueprint)) || !scanTarget.isValid || PDAScanner.CanScan() != PDAScanner.Result.Scan || !GameModeUtils.RequiresBlueprints())
            {
                return;
            }
#if SUBNAUTICA
            HandReticle.main.SetInteractText(UnknownNameConfig.UnkownLabel, false, HandReticle.Hand.None);
#elif BELOWZERO
            HandReticle.main.SetText(HandReticle.TextType.Hand, UnknownNameConfig.UnkownLabel, true, GameInput.Button.None);
#endif
        }
    }

    [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Scan), new Type[] { })]
    public class PDAScanner_Scan
    {
        public static TechType techType;
        [HarmonyPrefix]
        public static void Prefix()
        {
            if(PDAScanner.scanTarget.techType != TechType.None)
            {
                techType = PDAScanner.scanTarget.techType;
            }
        }

        [HarmonyPostfix]
        public static void Postfix(ref PDAScanner.Result __result)
        {
            if(__result != PDAScanner.Result.Scan && !KnownTech.Contains(techType))
            {
                KnownTech.Add(techType);
#if SUBNAUTICA
                TechType techType2 = CraftData.GetHarvestOutputData(techType);
#elif BELOWZERO
                TechType techType2 = TechData.GetHarvestOutput(techType);
#endif
                if(techType2 != TechType.None)
                {
                    KnownTech.Add(techType2);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Builder),nameof(Builder.Begin))]
    public class Builder_Begin
    {
        [HarmonyPrefix]
        public static bool Prefix()
    }
}