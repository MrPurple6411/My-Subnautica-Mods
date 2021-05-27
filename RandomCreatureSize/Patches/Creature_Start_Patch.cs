namespace RandomCreatureSize.Patches
{
    using HarmonyLib;
    using Configuration;
    using SMLHelper.V2.Handlers;
    using System;
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

            if (__instance.gameObject != null && __instance.gameObject.GetComponent<WaterParkCreature>() != null &&
                __instance.gameObject.GetComponent<WaterParkCreature>().IsInsideWaterPark()) return;
            try
            {
                var id = __instance.GetComponent<PrefabIdentifier>().Id;
                float scale;
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

                var creatureActions = __instance.gameObject.GetComponentsInChildren<CreatureAction>().Where((x) => x.GetType().GetField("swimVelocity") != null).ToList();

                scale = scale < 1f ? 1f : scale > 5f ? 5f : scale;

                foreach(var creatureAction in creatureActions)
                {
                    var swimVelocity = Traverse.Create(creatureAction).Field("swimVelocity");
                    if (!swimVelocity.FieldExists()) continue;
                    var currentSpeed = swimVelocity.GetValue<float>();
                    swimVelocity.SetValue(currentSpeed * scale);
                }
            }
            catch
            {
                // ignored
            }
        }
    }

}
