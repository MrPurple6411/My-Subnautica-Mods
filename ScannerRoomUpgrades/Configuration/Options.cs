using QModManager.Utility;
using SMLHelper.V2.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScannerRoomUpgrades.Configuration
{
    internal class Options : ModOptions
    {
        private Config config = Main.config;

        public Options() : base("ScannerRoomUpgrades")
        {
            try
            {
                config.Load();
            }
            catch(Exception e)
            {
                Logger.Log(Logger.Level.Error, $"Failed to load Config file generating new one!", e);
                config.Save();
            }

        }

        public override void BuildModOptions()
        {

        }
    }
}
