using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.integrations;

namespace AnimalHusbandryMod.common
{
    internal class ConfigMenuController
    {
        public static void CreateConfigMenu(IManifest manifest)
        {
            GenericModConfigMenuApi api = DataLoader.Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api == null) return;

            api.Register(manifest, () => DataLoader.ModConfig = new ModConfig(), () =>
            {
                DataLoader.Helper.WriteConfig(DataLoader.ModConfig);
                DataLoader.InvalidateCache();
            });

            api.AddSectionTitle(manifest, () => "Main Features:", () => "Properties to disable the mod main features.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableMeat, (bool val) => DataLoader.ModConfig.DisableMeat = val, () => "Disable Meat", () => "Disable all features related to meat. Meat Cleaver/Wand will not be delivered , and if already owned, will not work. Meat items and meat dishes will not be loaded. Any item still on the inventory will be bugged. You should sell/trash all of them before disabling meat. Meat Friday will not show on TV. You will not receive any more meat recipe letter from the villagers. Learned recipes will still be known, but will not show on the cooking menu. If re-enabled, they will show again. Restart the game after changing this.");
                
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisablePregnancy, (bool val) => DataLoader.ModConfig.DisablePregnancy = val, () => "Disable Pregnancy", () => "Disable all features related to pregnancy. Syringe will not be delivered, and if already owned, it will not work. Pregnancy status will not update but will not reset. Animals that were pregnant will be with random pregnancy disabled unless changed. If re-enabled, everything will resume as it was before. Restart the game after changing this.");
                
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableTreats, (bool val) => DataLoader.ModConfig.DisableTreats = val, () => "Disable Treats", () => "Disable all features related to treats. The basket will not be delivered, and if already owned, it will not work. Treat status will update while the treat feature is disable. Animals that were feed treats before will be able to eat again if the appropriate amount of days has passed when the mod was disabled. Restart the game after changing this.");
                
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableAnimalContest, (bool val) => DataLoader.ModConfig.DisableAnimalContest = val, () => "Disable Animal Contest", () => "Disable all features related to the animal contest. You won't receive any more participant ribbons. Bonus from previous winners will still apply though. Restart the game after changing this.");

            api.AddSectionTitle(manifest, () => "Meat Properties:", () => "Properties to configure the meat feature.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.Softmode, (bool val) => DataLoader.ModConfig.Softmode = val, () => "Softmode", () => "Enable the Softmode. When enabled the Meat Cleaver is replaced with the Meat Want. They work the same, but sound, text and effects are changed to resemble magic. Restart the game after changing this.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableRancherMeatPriceAjust, (bool val) => DataLoader.ModConfig.DisableRancherMeatPriceAjust = val, () => "Disable Rancher Affect Meat", () => "Disable the patch that make Rancher Profession work on meat items.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableMeatInBlundle, (bool val) => DataLoader.ModConfig.DisableMeatInBlundle = val, () => "Disable Meat In Bundle", () => "Disable the addition of meat to the Animal Bundle in the Community Center. Needs to start a new game so it can take effect.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableMeatFromDinosaur, (bool val) => DataLoader.ModConfig.DisableMeatFromDinosaur = val, () => "Disable Meat From Dinosaur", () => "Disable dinosaur giving a random kind of meat.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableSellPriceAdjustOverMeatLimit, (bool val) => DataLoader.ModConfig.DisableSellPriceAdjustOverMeatLimit = val, () => "Disable Sell Price Adjust", () => "Disable the amount of meat given by an animal going over the configured limit to adjust to the animal sell price.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableMeatToolLetter, (bool val) => DataLoader.ModConfig.DisableMeatToolLetter = val, () => "Disable Meat Tool Letter", () => "Disable the sending of the meat cleaver or the meat wand. Meat will only be able to be obtained through the meat button.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableMeatButton, (bool val) => DataLoader.ModConfig.DisableMeatButton = val, () => "Disable Meat Button", () => "Disable the meat button showing in the animal query menu. Meat will only be able to be obtained using the tools.");

            api.AddKeybind(manifest, () => DataLoader.ModConfig.AddMeatCleaverToInventoryKey ?? SButton.None, (SButton val) => DataLoader.ModConfig.AddMeatCleaverToInventoryKey = val == SButton.None ? (SButton?)null : val, () => "Add Meat Tool Key", () => "Set a keyboard key to directly add the Meat Cleaver/Want to your inventory.");
            api.AddSectionTitle(manifest, () => "Pregnancy Properties:", () => "Properties to configure the insemination feature.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableFullBuildingForBirthNotification, (bool val) => DataLoader.ModConfig.DisableFullBuildingForBirthNotification = val, () => "Disable Full Build Notif.", () => "Disable notifications for when an animals can't give birth because their building is full.");
                
            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableTomorrowBirthNotification, (bool val) => DataLoader.ModConfig.DisableTomorrowBirthNotification = val, () => "Disable Birth Notif.", () => "Disable notifications for when an animal will give birth tomorrow.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableAutomaticNameBabies, (bool val) => DataLoader.ModConfig.EnableAutomaticNameBabies = val, () => "Enable Auto Name Babies", () => "Enable automatic naming babies. This way you won't receive the pop-up notification that the baby was born and prompted for the name of the baby at the start of the day. The notification will show up in the chat box.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableInseminationSyringeLetter, (bool val) => DataLoader.ModConfig.DisableInseminationSyringeLetter = val, () => "Disable Insemination Letter", () => "Disable the sending of the Insemination Syringe. You will need to get the syringe another way.");

            api.AddKeybind(manifest, () => DataLoader.ModConfig.AddInseminationSyringeToInventoryKey ?? SButton.None, (SButton val) => DataLoader.ModConfig.AddInseminationSyringeToInventoryKey = val == SButton.None ? (SButton?)null : val, () => "Add Insemination Syringe Key", () => "Set a keyboard key to directly add the Insemination Syringe to your inventory.");

            api.AddSectionTitle(manifest, () => "Treats Properties:", () => "Properties to configure the treats feature.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableFriendshipInscreseWithTreats, (bool val) => DataLoader.ModConfig.DisableFriendshipInscreseWithTreats = val, () => "Disable Friendship Increase", () => "Disable animal friendship being increased when given a treat.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableMoodInscreseWithTreats, (bool val) => DataLoader.ModConfig.DisableMoodInscreseWithTreats = val, () => "Disable Mood Increase", () => "Disable animal mood being set to max when given a treat.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableTreatsCountAsAnimalFeed, (bool val) => DataLoader.ModConfig.EnableTreatsCountAsAnimalFeed = val, () => "Enable Treats Count As Feed", () => "Enable animal feed status being set to max when given a treat.");

            api.AddNumberOption(manifest, () => (float)DataLoader.ModConfig.PercentualAjustOnFriendshipInscreaseFromProfessions, (float val) => DataLoader.ModConfig.PercentualAjustOnFriendshipInscreaseFromProfessions = val, () => "Professions Adjust", () => "Change the percentage adjust for friendship increase when giving treats when you have the coopmaster or shepherd professions. 0.25 means 25% more than usual.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableFeedingBasketLetter, (bool val) => DataLoader.ModConfig.DisableFeedingBasketLetter = val, () => "Disable Feeding Basket Letter", () => "Disable the sending of the Feeding Basket. You will need to get the basket another way.");

            api.AddKeybind(manifest, () => DataLoader.ModConfig.AddFeedingBasketToInventoryKey ?? SButton.None, (SButton val) => DataLoader.ModConfig.AddFeedingBasketToInventoryKey = val == SButton.None ? (SButton?)null : val, () => "Add Feeding Basket Key", () => "Set a keyboard key to directly add the Feeding Basket to your inventory.");

            api.AddSectionTitle(manifest, () => "Animal Contest Properties:", () => "Properties to configure the animal contest feature.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableContestBonus, (bool val) => DataLoader.ModConfig.DisableContestBonus = val, () => "Disable Contest Bonus", () => "Disable the fertility and the production bonuses from the contest. If enabled again, all winners will receive the bonus again, no matter if the bonus was disabled when they won.");

            api.AddSectionTitle(manifest, () => "Misc. Properties:", () => "Miscellaneous Properties.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.ForceDrawAttachmentOnAnyOS, (bool val) => DataLoader.ModConfig.ForceDrawAttachmentOnAnyOS = val, () => "Force Draw Attachment", () => "Force the patch that draw the hover menu for the feeding basket and insemination syringe on any OS. Restart the game after changing this.");

            api.AddBoolOption(manifest, () => DataLoader.ModConfig.DisableTvChannels, (bool val) => DataLoader.ModConfig.DisableTvChannels = val, () => "Disable TV Channels", () => "Disable all TV channels added by this mod.");
        }
    }
}
