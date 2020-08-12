using SMLHelper.V2.Options;

namespace WorldLoad.Configuration
{
    public class Options : ModOptions
    {
        public Options() : base("World Load Settings")
        {
            SliderChanged += WorldLoadOptions_SliderChanged;
        }

        public override void BuildModOptions()
        {
            AddSliderOption("WorldLoad", "Load Distance", 2, 50, Main.config.IncreasedWorldLoad, 8);
        }

        public void WorldLoadOptions_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if (e.Id == "WorldLoad")
            {
                Main.config.IncreasedWorldLoad = e.IntegerValue;
                Main.config.Save();
            }
        }
    }
}