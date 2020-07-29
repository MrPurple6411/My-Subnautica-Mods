using SMLHelper.V2.Options;

namespace Time_Eternal
{
    public class Options : ModOptions
    {
        public Options() : base("Time Eternal")
        {
            ChoiceChanged += Options_DayToggleChanged;
        }

        public override void BuildModOptions()
        {
            AddChoiceOption("DayNightToggle", "Freeze Lighting", new string[] { "Normal", "Noon", "MidNight" }, Main.config.freezeTimeChoice);
        }

        public void Options_DayToggleChanged(object sender, ChoiceChangedEventArgs e)
        {
            if (e.Id != "DayNightToggle")
            {
                return;
            }

            Main.config.freezeTimeChoice = e.Index;
        }
    }
}