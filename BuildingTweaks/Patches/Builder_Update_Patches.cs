namespace BuildingTweaks.Patches
{
    using HarmonyLib;
    using UnityEngine;

    [HarmonyPatch]
    public static class Builder_Update_Patches
    {
        public static bool Freeze 
        { 
            get => freeze; 
            set 
            { 
                if(freeze != value)
                    Time.timeScale = value ? 0.0f : 1;

                freeze = value;
            } 
        }

        private static bool freeze;
        public static bool UpdatePlacement = true;
        private static Vector3 PlayerOrigin;
        private static Vector3 Origin;
        private static Quaternion OriginRotation;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Builder), nameof(Builder.Update))]
        public static void Prefix(ref Vector3 ___placePosition, ref Quaternion ___placeRotation)
        {
            if(Input.GetKeyDown(KeyCode.KeypadDivide))
            {
                UpdatePlacement = !UpdatePlacement;
                PlayerOrigin = Builder.GetAimTransform().position;
                Origin = ___placePosition;
                OriginRotation = ___placeRotation;
            }

            if (UpdatePlacement || Builder.ghostModel == null) return;
            var dist = Origin - PlayerOrigin;
            dist = new Vector3(dist.x, 0f, dist.z).normalized;
            var speed = Time.deltaTime * 10;
            var forward = dist * speed;
            var up = Vector3.Cross(Vector3.right, dist) * speed;
            var right = Vector3.Cross(Vector3.up, dist) * speed;

            if(Input.GetKey(KeyCode.Keypad8))
            {
                Origin += forward;
                PlayerOrigin += forward;
            }
            if(Input.GetKey(KeyCode.Keypad2))
            {
                Origin -= forward;
                PlayerOrigin -= forward;
            }
            if(Input.GetKey(KeyCode.KeypadMinus))
            {
                Origin -= up;
                PlayerOrigin -= up;

            }
            if(Input.GetKey(KeyCode.KeypadPlus))
            {
                Origin += up;
                PlayerOrigin += up;
            }
            if(Input.GetKey(KeyCode.Keypad4))
            {
                Origin -= right;
                PlayerOrigin -= right;

            }
            if(Input.GetKey(KeyCode.Keypad6))
            {
                Origin += right;
                PlayerOrigin += right;
            }

            if(Builder.forceUpright)
            {
                forward = Builder.ghostModel.transform.forward;
                forward.y = 0f;
                forward.Normalize();
                up = Vector3.up;
            }
            else
            {
                forward = Builder.ghostModel.transform.forward;
                up = Builder.ghostModel.transform.up;
            }
            OriginRotation = Quaternion.LookRotation(forward, up);
            if(Builder.rotationEnabled)
            {
                OriginRotation = Quaternion.AngleAxis(Builder.additiveRotation, up) * OriginRotation;
            }
            Builder.placePosition = Origin;
            Builder.placeRotation = OriginRotation;
        }


        [HarmonyPatch(typeof(ConstructableBase), nameof(ConstructableBase.UpdateGhostModel))]
        [HarmonyPrefix]
        public static bool Prefix(ref bool geometryChanged, ref bool __result)
        {
            if (UpdatePlacement) return UpdatePlacement;
            __result = true;
            geometryChanged = false;
            return UpdatePlacement;

        }


        [HarmonyPatch(typeof(Builder),nameof(Builder.SetDefaultPlaceTransform))]
        [HarmonyPrefix]
        public static bool SetDefaultPlaceTransform_Prefix()
        {
            return UpdatePlacement;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Builder), nameof(Builder.TryPlace))]
        public static void Postfix(bool __result)
        {
            if (!__result) return;
            if(Freeze)
                Freeze = !Freeze;

            if(!UpdatePlacement)
                UpdatePlacement = !UpdatePlacement;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Builder), nameof(Builder.End))]
        public static void End_Prefix()
        {
            if (Builder.prefab == null) return;
            Freeze = false;
            UpdatePlacement = true;
        }
    }
}
