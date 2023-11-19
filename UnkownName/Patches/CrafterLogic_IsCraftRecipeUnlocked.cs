namespace UnknownName.Patches;

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Nautilus.Handlers;

[HarmonyPatch(typeof(CrafterLogic), nameof(CrafterLogic.IsCraftRecipeUnlocked))]
public class CrafterLogic_IsCraftRecipeUnlocked
{
	public static List<TechType> blackList = new() { TechType.Titanium, TechType.Copper };

	[HarmonyPostfix]
	public static void Postfix(TechType techType, ref bool __result)
	{

#if SUBNAUTICA
		if (!GameModeUtils.RequiresBlueprints())
		{
			__result = true;
			return;
		}
#else
		if (!GameModeManager.GetOption<bool>(GameOption.TechRequiresUnlocking))
		{
			__result = true;
			return;
		}
#endif

		if (Main.Config.Hardcore && __result)
		{
			if (!BepInEx.Bootstrap.Chainloader.PluginInfos.Values.Any(x => x.Metadata.Name == "UITweaks"))
			{
				var data = CraftDataHandler.GetRecipeData(techType);
				var ingredientCount = data?.ingredientCount ?? 0;
				for (var i = 0; i < ingredientCount; i++)
				{
					var ingredient = data.Ingredients[i];
					if (!blackList.Contains(techType) && !CrafterLogic.IsCraftRecipeUnlocked(ingredient.techType))
					{
						__result = false;
						return;
					}
				}
			}
			else if (CraftDataHandler.GetRecipeData(techType) is { } data)
			{
				var ingredientCount = data?.ingredientCount ?? 0;
				for (var i = 0; i < ingredientCount; i++)
				{
					var ingredient = data.GetIngredient(i);
					if (!blackList.Contains(techType) && !CrafterLogic.IsCraftRecipeUnlocked(ingredient.techType))
					{
						__result = false;
						return;
					}
				}
			}
		}
	}
}