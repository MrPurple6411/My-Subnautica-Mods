using SMLHelper.V2.Options;

namespace Increased_Resource_Spawns
{
    public class Options : ModOptions
    {
        public Options() : base("Increased Resource Spawn Settings")
        {
            SliderChanged += ResourceMultiplier_SliderChanged;
        }

        public override void BuildModOptions()
        {
            AddSliderOption("ResourceMultiplier", "Resource Multiplier", 1, 100, Main.config.ResourceMultiplier);
        }

        public void ResourceMultiplier_SliderChanged(object sender, SliderChangedEventArgs e)
        {
            if (e.Id != "ResourceMultiplier")
            {
                return;
            }

            Main.config.ResourceMultiplier = e.IntegerValue;
            Main.config.Save();
        }
    }
}