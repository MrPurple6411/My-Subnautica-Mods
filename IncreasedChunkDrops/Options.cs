using SMLHelper.V2.Options;

namespace IncreasedChunkDrops
{
    public class Options : ModOptions
    {
        public Options() : base("Increased Chunk Drop Settings")
        {
            SliderChanged += ExtraCount_SliderChanged;
            SliderChanged += ExtraCountMax_SliderChanged;
        }

        public override void BuildModOptions()
        {
            AddSliderOption("ExtraCount", "Extra items min", 0, 100, Main.config.ExtraCount);
            AddSliderOption("ExtraCountMax", "Extra items max", 0, 100, Main.config.ExtraCountMax);
        }

        public void ExtraCount_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if (e.Id != "ExtraCount")
            {
                return;
            }

            Main.config.ExtraCount = e.IntegerValue;
            Main.config.Save();
        }
        public void ExtraCountMax_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if (e.Id != "ExtraCountMax")
            {
                return;
            }

            Main.config.ExtraCountMax = e.IntegerValue;
            Main.config.Save();
        }
    }
}