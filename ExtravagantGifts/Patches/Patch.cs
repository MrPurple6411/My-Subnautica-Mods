#if BZ
namespace ExtravagantGifts.Patches
{
    using HarmonyLib;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using Story;

    public static class Patches
    {
        [HarmonyPatch(typeof(SeaMonkey), nameof(SeaMonkey.IsGiftBehaviorEnabled))]
        [HarmonyPostfix]
        public static void Postfix(ref bool __result)
        {
            if(__result)
                return;

            if(StoryGoalManager.main != null && StoryGoalManager.main.completedGoals.Contains(StoryGoalManager.main.alanTransferedGoal))
                __result = true;

        }

        [HarmonyPatch(typeof(SeaMonkeySpawnRandomItem), nameof(SeaMonkeySpawnRandomItem.TrySpawnAsync))]
        [HarmonyPrefix]
        public static void Prefix(ref object forcePinnedIngredientSpawn)
        {
            if(PinManager.Count > 0)
            {
                forcePinnedIngredientSpawn = true;
            }
        }

        [HarmonyPatch(typeof(SeaMonkeySpawnRandomItem), nameof(SeaMonkeySpawnRandomItem.GetPinnedIngredient))]
        [HarmonyPostfix]
        public static void Postfix(ref TechType __result)
        {
            if(__result == TechType.None)
                return;

            RecipeData recipeData;
            TechType techType2;
            bool eggCheck;
            while(!(eggCheck = TechTypeExtensions.FromString(__result.AsString() + "Egg", out techType2, true)) && (recipeData = CraftDataHandler.GetTechData(__result)) != null && recipeData.ingredientCount > 0)
            {
                try{ __result = recipeData.Ingredients.GetRandom().techType; }
                catch
                {
                    // ignored
                }
            }

            if(eggCheck)
                __result = techType2;
        }

        [HarmonyPatch(typeof(SeaMonkeyData), nameof(SeaMonkeyData.IsAllowedGift))]
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, ref TechType techType)
        {
            if(PinManager.Count > 0)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(SeaMonkeyBringGift), nameof(SeaMonkeyBringGift.Evaluate))]
        [HarmonyPrefix]
        public static void Prefix(SeaMonkeyBringGift __instance)
        {
            __instance.minInterval = 0f;
            __instance.maxRange = 200;
            __instance.maxDuration = 70;
        }

        [HarmonyPatch(typeof(SeaMonkeyBringGift), nameof(SeaMonkeyBringGift.Evaluate))]
        [HarmonyPostfix]
        public static void Postfix(SeaMonkeyBringGift __instance, ref float __result, float time)
        {
            if(PinManager.Count == 0)
                return;

            if(__result > 0f)
            {
                __result *= 100f;
                return;
            }

            if(GameModeUtils.IsInvisible())
            {
                if(!SeaMonkey.IsGiftBehaviorEnabled())
                {
                    __result = 0f;
                    return;
                }
                var main = Player.main;
                if(main.IsInside() || !main.IsUnderwater())
                {
                    __result = 0f;
                    return;
                }
                if(SeaMonkeyBringGift.activeSeaMonkeyGiftAction != null && SeaMonkeyBringGift.activeSeaMonkeyGiftAction != __instance)
                {
                    __result = 0f;
                    return;
                }
                if(__instance.state == SeaMonkeyBringGift.State.Animation || __instance.state == SeaMonkeyBringGift.State.AnimationEnd)
                {
                    __result = __instance.finishAnimationPriority;
                    return;
                }
                switch(__instance.state)
                {
                    case SeaMonkeyBringGift.State.Inactive:
                        if(time < __instance.timeLastGiftAnimation + __instance.minInterval || time < SeaMonkeyBringGift.timeLastGlobalSeaMonkeyGift + __instance.minGlobalGiftInterval)
                        {
                            __result = 0f;
                            return;
                        }
                        if((Utils.GetLocalPlayerPos() - __instance.transform.position).sqrMagnitude > __instance.maxRange * __instance.maxRange)
                        {
                            __result = 0f;
                            return;
                        }
                        break;
                    case SeaMonkeyBringGift.State.Swim:
                        if(time > __instance.timeActionStart + __instance.maxDuration)
                        {
                            __result = 0f;
                            return;
                        }
                        break;
                    case SeaMonkeyBringGift.State.End:
                        __result = 0f;
                        return;
                }
                if(!__instance.HasGift() && !__instance.spawnTempItem.TrySpawn(__instance.IsFirstGift(), main.GetBiomeString()))
                {
                    __result = 0f;
                    return;
                }

                __result = __instance.GetEvaluatePriority();
            }

        }
    }
}
#endif