using Harmony;
using QModManager.API.ModLoading;
using System;
using System.Collections.Generic;
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

        [HarmonyPrefix]
        public static bool Prefix(ITechData data)
        {
            int ingredientCount = data.ingredientCount;
            for(int i = 0; i < ingredientCount; i++)
            {
                IIngredient ingredient = data.GetIngredient(i);
                TechType techType = ingredient.techType;
                if(!KnownTech.Contains(techType) && GameModeUtils.RequiresBlueprints())
                {
                    return false;
                }
            }

            return true;
        }

#elif BELOWZERO

        [HarmonyPrefix]
        public static bool Prefix(IList<Ingredient> ingredients)
        {
            int ingredientCount = ingredients.Count;
            for(int i = 0; i < ingredientCount; i++)
            {
                if(!KnownTech.Contains(ingredients[i].techType) && GameModeUtils.RequiresBlueprints())
                {
                    return false;
                }
            }

            return true;
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
            if(__instance.energyMixin.charge <= 0f || !scanTarget.isValid || PDAScanner.CanScan() != PDAScanner.Result.Scan || __instance.stateCurrent == ScannerTool.ScanState.SelfScan || !GameModeUtils.RequiresBlueprints())
            {
                return;
            }

            HandReticle main = HandReticle.main;
            main.SetInteractText("???????", true, HandReticle.Hand.Right);
            main.SetIcon(HandReticle.IconType.Scan, 1.5f);
            __instance.UpdateScreen(ScannerTool.ScreenState.Ready, 0f);
        }

#elif BELOWZERO

        [HarmonyPostfix]
        public static void Postfix(ScannerTool __instance)
        {
            PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
            if(__instance.energyMixin.charge <= 0f || !scanTarget.isValid || PDAScanner.CanScan() != PDAScanner.Result.Scan || !GameModeUtils.RequiresBlueprints())
            {
                return;
            }

            HandReticle main = HandReticle.main;
            main.SetText(HandReticle.TextType.Hand, "???????", true, GameInput.Button.RightHand);
            main.SetIcon(HandReticle.IconType.Scan, 1.5f);
            __instance.UpdateScreen(ScannerTool.ScreenState.Ready, 0f);
        }

#endif
    }

    [HarmonyPatch(typeof(Inventory), nameof(Inventory.Pickup))]
    public class Inventory_Pickup
    {
        [HarmonyPostfix]
        public static void Postfix(Pickupable pickupable)
        {
            if(!KnownTech.Contains(pickupable.GetTechType()) && GameModeUtils.RequiresBlueprints())
            {
                KnownTech.Add(pickupable.GetTechType());
            }
        }
    }

    [HarmonyPatch(typeof(GUIHand), nameof(GUIHand.OnUpdate))]
    public class GUIHand_Send
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
        [HarmonyPostfix]
        public static void Postfix(ref PDAScanner.Result __result)
        {
            if((__result == PDAScanner.Result.Done || __result == PDAScanner.Result.Researched) && !KnownTech.Contains(PDAScanner.scanTarget.techType))
            {
                KnownTech.Add(PDAScanner.scanTarget.techType);
            }
        }
    }
}