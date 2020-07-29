using System.Collections.Generic;
using SMLHelper.V2.Options;

namespace BetterACU
{
    public class Options : ModOptions
    {
        public Options() : base("Better ACU")
        {
            SliderChanged += BetterACU_SliderChanged;
            ToggleChanged += BetterACU_ToggleChanged;
        }

        private void BetterACU_ToggleChanged(object sender, ToggleChangedEventArgs e)
        {
            var ids = new List<string>() { "OverFlowIntoOcean" };

            if (!ids.Contains(e.Id))
            {
                return;
            }

            switch (e.Id)
            {
                case "OverFlowIntoOcean":
                    Main.config.OverFlowIntoOcean = e.Value;
                    Main.config.Save();
                    break;

                default:
                    break;
            }
            Main.config.Save();
        }

        public void BetterACU_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            var ids = new List<string>() {
                "WaterParkSize",
#if BELOWZERO
                "LargeWaterParkSize"
#endif
            };

            if (!ids.Contains(e.Id))
            {
                return;
            }

            switch (e.Id)
            {
                case "WaterParkSize":
                    Main.config.WaterParkSize = e.IntegerValue;
                    Main.config.Save();
                    break;
#if BELOWZERO
                case "LargeWaterParkSize":
                    Main.config.LargeWaterParkSize = e.IntegerValue;
                Main.config.Save();
                    break;
#endif
                default:
                    break;
            }
        }

        public override void BuildModOptions()
        {
            AddSliderOption("WaterParkSize", "Alien Containment Limit", 10, 100, Main.config.WaterParkSize);
#if BELOWZERO
            AddSliderOption("LargeWaterParkSize", "Large Room Alien Containment Limit", 20, 200, Main.config.LargeWaterParkSize);
#endif
            AddToggleOption("OverFlowIntoOcean", "Allow Breed Into Ocean", Main.config.OverFlowIntoOcean);
        }
    }
}
