using FMOD;
using Harmony;
using PriorityQueueInternal;
using QModManager.API.ModLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UnkownName
{
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
                TooltipFactory.WriteTitle(stringBuilder, "???????");
                TooltipFactory.WriteDescription(stringBuilder, "???????");
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
            TooltipFactory.WriteTitle(sb, "???????");
            TooltipFactory.WriteDescription(sb, "???????");
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
                __result = "???????";
            }
        }
    }

    [HarmonyPatch(typeof(ScannerTool), nameof(ScannerTool.OnHover))]
    public class ScannerTool_OnHover
    {
#if SUBNAUTICA

        [HarmonyPostfix]
        public static void Postfix(ScannerTool __instance)
        {
            PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
            PDAScanner.Result result = PDAScanner.CanScan();

            if(PDAScanner.ContainsCompleteEntry(scanTarget.techType) || __instance.energyMixin.charge <= 0f || !scanTarget.isValid || result != PDAScanner.Result.Scan || __instance.stateCurrent == ScannerTool.ScanState.SelfScan || !GameModeUtils.RequiresBlueprints())
            {
                return;
            }
            HandReticle main = HandReticle.main;
            main.SetInteractText("???????", true, HandReticle.Hand.Right);
        }

#elif BELOWZERO

        [HarmonyPostfix]
        public static void Postfix(ScannerTool __instance)
        {
            PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
            PDAScanner.Result result = PDAScanner.CanScan();
            if(PDAScanner.ContainsCompleteEntry(scanTarget.techType) || __instance.energyMixin.charge <= 0f || !scanTarget.isValid || result != PDAScanner.Result.Scan || !GameModeUtils.RequiresBlueprints())
            {
                return;
            }
            HandReticle main = HandReticle.main;
            main.SetText(HandReticle.TextType.Hand, "???????", true, GameInput.Button.RightHand);
        }

#endif
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
            if(Inventory.main.container.Contains(TechType.Scanner) || techType == TechType.AcidMushroom || techType == TechType.Copper)
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
                }
                KnownTech.Add(techType, true);
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
                : !PDAScanner.ContainsCompleteEntry(techType)
                ? true
                : string.IsNullOrEmpty(entryData.encyclopedia) || PDAEncyclopedia.ContainsEntry(entryData.encyclopedia);
        }
    }

    [HarmonyPatch(typeof(GUIHand), nameof(GUIHand.OnUpdate))]
    public class GUIHand_OnUpdate
    {
#if SUBNAUTICA

        [HarmonyPostfix]
        public static void Postfix()
        {
            PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
            if(scanTarget.isValid && PDAScanner.CanScan() == PDAScanner.Result.Scan && !PDAScanner.scanTarget.isPlayer && GameModeUtils.RequiresBlueprints())
            {
                HandReticle.main.SetInteractText("???????", false, HandReticle.Hand.None);
            }
        }

#elif BELOWZERO

        [HarmonyPostfix]
        public static void Postfix()
        {
            PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
            if(scanTarget.isValid && PDAScanner.CanScan() == PDAScanner.Result.Scan && GameModeUtils.RequiresBlueprints())
            {
                HandReticle.main.SetText(HandReticle.TextType.Hand, "???????", true, GameInput.Button.None);
            }
        }

#endif
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
            }
        }
    }
}