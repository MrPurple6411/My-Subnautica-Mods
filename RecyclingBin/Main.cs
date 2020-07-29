using System;
using System.Reflection;
using HarmonyLib;
using QModManager.API;
using QModManager.API.ModLoading;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Handlers;
using UnityEngine;

namespace RecyclingBin
{
    [QModCore]
    public class Main
    {
        [QModPatch]
        public void Load()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                new Harmony($"MrPurple6411_{assembly.GetName().Name}").PatchAll(assembly);
                LanguageHandler.SetTechTypeName(TechType.Trashcans, "Recycling Bin");
                LanguageHandler.SetTechTypeTooltip(TechType.Trashcans, "Breaks items down to the most basic materials. \nNote: Batteries and Tools must be fully charged to be recycled.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public static bool BatteryCheck(Pickupable pickupable)
        {
            EnergyMixin energyMixin = pickupable.gameObject.GetComponentInChildren<EnergyMixin>();
            if (energyMixin != null)
            {
                GameObject gameObject = energyMixin.GetBattery();
                bool defaultCheck = false;
                if (gameObject != null)
                {
                    defaultCheck = energyMixin.defaultBattery == CraftData.GetTechType(gameObject);
                }

                if (gameObject == null && QModServices.Main.ModPresent("NoBattery"))
                {
                    return true;
                }
                if (gameObject != null && (defaultCheck || QModServices.Main.ModPresent("NoBattery")))
                {
                    IBattery battery = gameObject.GetComponent<IBattery>();
#if SUBNAUTICA
                    TechData techData = CraftDataHandler.GetTechData(pickupable.GetTechType());
#elif BELOWZERO
                    RecipeData techData = CraftDataHandler.GetRecipeData(pickupable.GetTechType());
#endif
                    bool recipeCheck = techData.Ingredients.FindAll((ingredient) => ingredient.techType == TechType.Battery || ingredient.techType == TechType.PrecursorIonBattery || ingredient.techType == TechType.LithiumIonBattery || ingredient.techType == TechType.PowerCell || ingredient.techType == TechType.PrecursorIonPowerCell).Count == 0;
                    if (battery != null && QModServices.Main.ModPresent("NoBattery") && recipeCheck)
                    {
                        ErrorMessage.AddMessage($"{pickupable.GetTechType().ToString()} has a battery in it. Cannot Recycle.");
                        return false;
                    }
                    else if (battery != null && defaultCheck && battery.charge > (battery.capacity * 0.99))
                    {
                        return true;
                    }
                    else
                    {
                        if (gameObject != null && !defaultCheck)
                        {
                            ErrorMessage.AddMessage($"{CraftData.GetTechType(gameObject).ToString()} is not the default battery for {pickupable.GetTechType().ToString()}.");
                        }
                        else
                        {
                            ErrorMessage.AddMessage($"{pickupable.GetTechType().ToString()} is not fully charged and cannot be recycled.");
                        }

                        return false;
                    }
                }
                else
                {
                    if (gameObject != null)
                    {
                        ErrorMessage.AddMessage($"{CraftData.GetTechType(gameObject).ToString()} is not the default battery for {pickupable.GetTechType().ToString()}.");
                    }
                    else
                    {
                        ErrorMessage.AddMessage($"{pickupable.GetTechType().ToString()} has no battery.");
                    }

                    return false;
                }
            }

            IBattery b2 = pickupable.GetComponent<IBattery>();
            if (b2 != null)
            {
                if (b2.charge > (b2.capacity * 0.99))
                {
                    return true;
                }
                else
                {
                    ErrorMessage.AddMessage($"{pickupable.GetTechType().ToString()} is not fully charged and cannot be recycled.");
                    return false;
                }
            }
            return true;
        }
    }
}