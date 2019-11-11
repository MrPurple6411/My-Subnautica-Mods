using Harmony;
using System;
using System.Reflection;
using UnityEngine;

namespace BuilderModuleInputFix
{
    public class Main
    {
        public static void Load()
        {
            try
            {
                HarmonyInstance.Create("MrPurple6411.BuilderModuleInputFix").PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    [HarmonyPatch(typeof(Builder))]
    [HarmonyPatch("Update")]
    internal class Builder_Update_Patch
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            if (Player.main.GetVehicle() != null)
            {
                Builder.Initialize();
                Builder.canPlace = false;
                if (Builder.prefab == null)
                {
                    return true;
                }
                if (Builder.CreateGhost())
                {

                }
                Builder.canPlace = Builder.UpdateAllowed();
                Transform transform = Builder.ghostModel.transform;
                transform.position = Builder.placePosition + Builder.placeRotation * Builder.ghostModelPosition;
                transform.rotation = Builder.placeRotation * Builder.ghostModelRotation;
                transform.localScale = Builder.ghostModelScale;
                Color value = (!Builder.canPlace) ? Builder.placeColorDeny : Builder.placeColorAllow;
                IBuilderGhostModel[] components = Builder.ghostModel.GetComponents<IBuilderGhostModel>();
                for (int i = 0; i < components.Length; i++)
                {
                    components[i].UpdateGhostModelColor(Builder.canPlace, ref value);
                }
                Builder.ghostStructureMaterial.SetColor(ShaderPropertyID._Tint, value);

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
