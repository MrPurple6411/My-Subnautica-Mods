using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedScannerRoomHudChip.Patches
{
    [HarmonyPatch(typeof(MapRoomFunctionality))]
    class MapRoomFunctionality_Patches
    {
        internal class ScannerChipFunctionality : MapRoomFunctionality
        {

        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MapRoomFunctionality.GetScanRange))]
        public static bool GetScanRangePrefix(MapRoomFunctionality __instance, ref float __result)
        {
            if(__instance is ScannerChipFunctionality)
            {
                __result = Mathf.Min(5000f, MapRoomFunctionality.defaultRange + (float)Inventory.main.container.GetCount(TechType.MapRoomUpgradeScanRange) * MapRoomFunctionality.rangePerUpgrade);
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MapRoomFunctionality.StartScanning))]
        public static bool StartScanningPrefix(MapRoomFunctionality __instance, TechType newTypeToScan)
        {
            if (__instance is ScannerChipFunctionality)
            {
                __instance.typeToScan = newTypeToScan;
                __instance.ObtainResourceNodes(__instance.typeToScan);
                __instance.scanActive = __instance.typeToScan > TechType.None;
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MapRoomFunctionality.Start))]
        public static bool StartPrefix(MapRoomFunctionality __instance)
        {
            if (__instance is ScannerChipFunctionality)
            {
                __instance.wireFrameWorld = Player.main.transform;
                if(__instance.typeToScan != TechType.None)
                    __instance.StartScanning(__instance.typeToScan);
                else
                    __instance.StartScanning(TechType.TimeCapsule);

                ResourceTracker.onResourceDiscovered += __instance.OnResourceDiscovered;
                ResourceTracker.onResourceRemoved += __instance.OnResourceRemoved;
                MapRoomFunctionality.mapRooms.Add(__instance);

                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MapRoomFunctionality.Update))]
        public static bool UpdatePrefix(MapRoomFunctionality __instance)
        {
            return !(__instance is ScannerChipFunctionality);
        }


        [HarmonyPrefix]
        [HarmonyPatch(nameof(MapRoomFunctionality.OnResourceDiscovered))]
        public static bool OnResourceDiscoveredPrefix(MapRoomFunctionality __instance, ResourceTracker.ResourceInfo info)
        {
            if (__instance is ScannerChipFunctionality)
            {
                if (__instance.typeToScan == info.techType)
                {
                    __instance.resourceNodes.Add(info);
                    __instance.numNodesScanned++;
                }
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(MapRoomFunctionality.OnResourceRemoved))]
        public static bool OnResourceRemovedPrefix(MapRoomFunctionality __instance, ResourceTracker.ResourceInfo info)
        {
            if (__instance is ScannerChipFunctionality)
            {
                if (__instance.typeToScan == info.techType)
                {
                    __instance.resourceNodes.Remove(info);
                    __instance.numNodesScanned--;
                }
                return false;
            }

            return true;
        }
    }
}
