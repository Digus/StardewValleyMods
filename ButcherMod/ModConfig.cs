using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod
{
    public class ModConfig
    {
        public bool Softmode;
        public string AddMeatCleaverToInventoryKey;
        public string AddInseminationSyringeToInventoryKey;
        public bool DisableFullBuildingForBirthNotification;
        public bool DisableTomorrowBirthNotification;
        public bool DisablePregnancy;
        public bool DisableMeat;

        public ModConfig()
        {
            this.Softmode = false;
            this.AddMeatCleaverToInventoryKey = null;
            this.AddInseminationSyringeToInventoryKey = null;
            this.DisableFullBuildingForBirthNotification = false;
            this.DisableTomorrowBirthNotification = false;
            this.DisablePregnancy = false;
            this.DisableMeat = false;
        }
    }
}
