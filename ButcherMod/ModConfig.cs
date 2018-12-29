using StardewModdingAPI;

namespace AnimalHusbandryMod
{
    public class ModConfig
    {
        public bool Softmode;
        public SButton? AddMeatCleaverToInventoryKey;
        public SButton? AddInseminationSyringeToInventoryKey;
        public SButton? AddFeedingBasketToInventoryKey;
        public bool DisableFullBuildingForBirthNotification;
        public bool DisableTomorrowBirthNotification;
        public bool DisablePregnancy;
        public bool DisableMeat;
        public bool DisableTreats;
        public bool DisableRancherMeatPriceAjust;
        public bool DisableMoodInscreseWithTreats;
        public bool DisableFriendshipInscreseWithTreats;
        public bool EnableTreatsCountAsAnimalFeed;
        public bool DisableMeatFromDinosaur;
        public double PercentualAjustOnFriendshipInscreaseFromProfessions;

        public ModConfig()
        {
            this.Softmode = false;
            this.AddMeatCleaverToInventoryKey = null;
            this.AddInseminationSyringeToInventoryKey = null;
            this.AddFeedingBasketToInventoryKey = null;
            this.DisableFullBuildingForBirthNotification = false;
            this.DisableTomorrowBirthNotification = false;
            this.DisablePregnancy = false;
            this.DisableMeat = false;
            this.DisableTreats = false;
            this.DisableRancherMeatPriceAjust = false;
            this.DisableMoodInscreseWithTreats = false;
            this.DisableFriendshipInscreseWithTreats = false;
            this.EnableTreatsCountAsAnimalFeed = false;
            this.DisableMeatFromDinosaur = false;
            this.PercentualAjustOnFriendshipInscreaseFromProfessions = 0.25;
        }
    }
}
