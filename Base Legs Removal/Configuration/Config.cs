namespace Base_Legs_Removal.Configuration
{
    using SMLHelper.V2.Json;
    using SMLHelper.V2.Options.Attributes;
    using UnityEngine;

    [Menu("Base Legs Removal")]
    public class Config: ConfigFile
    {
        [Toggle("Foundation"), OnChange(nameof(RegenLegs))]
        public bool FoundationLegs = false;

        [Toggle("MoonPool"), OnChange(nameof(RegenLegs))]
        public bool MoonPoolLegs = false;

        [Toggle("Multi-Purpose Room"), OnChange(nameof(RegenLegs))]
        public bool RoomLegs = false;

#if BZ
        [Toggle("Large Room"), OnChange(nameof(RegenLegs))]
        public bool LargeRoomLegs = false;

#endif

        [Toggle("X-Corridor"), OnChange(nameof(RegenLegs))]
        public bool XCorridor = false;

        [Toggle("T-Corridor"), OnChange(nameof(RegenLegs))]
        public bool TCorridor = false;

        [Toggle("I-Corridor"), OnChange(nameof(RegenLegs))]
        public bool ICorridor = false;

        [Toggle("L-Corridor"), OnChange(nameof(RegenLegs))]
        public bool LCorridor = false;

        private static void RegenLegs()
        {
            GameObject.FindObjectsOfType<Base>()?.ForEach((x) => x.RebuildGeometry());
        }
    }
}
