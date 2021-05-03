namespace RandomCreatureSize.Patches
{
    using HarmonyLib;
    using RandomCreatureSize.Configuration;
    using SMLHelper.V2.Handlers;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [HarmonyPatch(typeof(Creature), nameof(Creature.Start))]
    public static class Creature_Start_Patch
    {

        public static void Prefix(Creature __instance)
        {
            if(Main.CreatureConfig is null)
            {
                Main.CreatureConfig = new CreatureConfig();
                Main.CreatureConfig.Load();
                IngameMenuHandler.RegisterOnSaveEvent(Main.CreatureConfig.Save);
            }

            if(!__instance.gameObject?.GetComponent<WaterParkCreature>()?.IsInsideWaterPark() ?? true)
            {
                try
                {
                    string id = __instance.GetComponent<PrefabIdentifier>().Id;
                    float scale = 1;
                    if(!Main.CreatureConfig.CreatureSizes.ContainsKey(id))
                    {
                        UnityEngine.Random.InitState(DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond);
                        scale = UnityEngine.Random.Range(Main.Config.minsize, Main.Config.maxsize);
                        __instance.SetScale(scale);
                        Main.CreatureConfig.CreatureSizes.Add(__instance.GetComponent<PrefabIdentifier>().Id, scale);
                    }
                    else
                    {
                        scale = Main.CreatureConfig.CreatureSizes[id];
                        __instance.SetScale(scale);
                    }

                    List<CreatureAction> creatureActions = __instance.gameObject.GetComponentsInChildren<CreatureAction>().Where((x) => x.GetType().GetField("swimVelocity") != null)?.ToList() ?? new List<CreatureAction>();

                    scale = scale < 1f ? 1f : scale > 5f ? 5f : scale;

                    foreach(CreatureAction creatureAction in creatureActions)
                    {
                        Traverse swimVelocity = Traverse.Create(creatureAction).Field("swimVelocity");
                        if(swimVelocity.FieldExists())
                        {
                            float currentSpeed = swimVelocity.GetValue<float>();
                            swimVelocity.SetValue(currentSpeed * scale);
                        }
                    }
                }
                catch { };
            }
        }
    }

}
