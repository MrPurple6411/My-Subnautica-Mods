using SMLHelper.V2.Options;

namespace SpecialtyManifold
{
    public class Options : ModOptions
    {
        public Options() : base("Specialty Manifold")
        {
            ToggleChanged += Options_multipleTanksChanged;
        }

        public override void BuildModOptions()
        {
            AddToggleOption("multipleTanks", "Effects of multiple tanks?", Main.config.multipleTanks);
        }

        public void Options_multipleTanksChanged(object sender, ToggleChangedEventArgs e)
        {
            if (e.Id != "multipleTanks")
            {
                return;
            }

            Main.config.multipleTanks = e.Value;
        }
    }
}