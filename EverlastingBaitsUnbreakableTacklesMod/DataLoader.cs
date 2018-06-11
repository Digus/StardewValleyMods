using System;
using System.Collections.Generic;
using System.Linq;
using MailFrameworkMod;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;

namespace EverlastingBaitsAndUnbreakableTacklesMod
{
    public class DataLoader : IAssetEditor
    {
        public static IModHelper Helper;
        public static ITranslationHelper I18N;

        public DataLoader(IModHelper helper)
        {
            Helper = helper;

            var editors = Helper.Content.AssetEditors;
            editors.Add(this);
            I18N = helper.Translation;

            
            AddLetter(BaitTackle.EverlastingBait, (l)=>Game1.player.FishingLevel >= 10 && GetNpcFriendship("Willy") >= 10 * 250 && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.EverlastingBait.GetDescription()));
            AddLetter(BaitTackle.EverlastingWildBait, (l)=> Game1.player.craftingRecipes.ContainsKey("Wild Bait") && Game1.player.craftingRecipes.ContainsKey(BaitTackle.EverlastingBait.GetDescription()) && Game1.player.craftingRecipes[BaitTackle.EverlastingBait.GetDescription()] > 0 && GetNpcFriendship("Linus") >= 10 * 250 && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.EverlastingWildBait.GetDescription()));
            AddLetter(BaitTackle.EverlastingMagnet, (l)=> Game1.player.FishingLevel >= 10  && GetNpcFriendship("Wizard") >= 10 * 250 && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.EverlastingMagnet.GetDescription()), null,2);
            MailDao.SaveLetter
            (
                new Letter
                (
                    "UnbreakableTackleIntroduction"
                    , I18N.Get("UnbreakableTackleIntroduction.Letter")
                    , (l)=> !Game1.player.mailReceived.Contains(l.Id) && Game1.player.achievements.Contains(21) && Game1.player.FishingLevel >= 8 && GetNpcFriendship("Willy") >= 6 * 250 && GetNpcFriendship("Clint") >= 6 * 250
                    , (l)=>
                    {
                        Game1.player.mailReceived.Add(l.Id);
                    }
                )
            );
            AddLetter
            (
                BaitTackle.UnbreakableSpinner
                , (l) => Game1.player.achievements.Contains(21) && Game1.player.FishingLevel >= 8 && GetNpcFriendship("Willy") >= 6 * 250 && GetNpcFriendship("Clint") >= 6 * 250 && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.UnbreakableSpinner.GetDescription())
                , (l) => LoadTackleQuest(BaitTackle.UnbreakableSpinner)
            );
            AddLetter
            (
                BaitTackle.UnbreakableLeadBobber
                , (l) => Game1.player.mailReceived.Contains(BaitTackle.UnbreakableSpinner.GetQuestName()) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.UnbreakableLeadBobber.GetDescription())
                , (l) => LoadTackleQuest(BaitTackle.UnbreakableLeadBobber)
            );
            AddLetter
            (
                BaitTackle.UnbreakableTrapBobber
                , (l) => Game1.player.mailReceived.Contains(BaitTackle.UnbreakableLeadBobber.GetQuestName()) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.UnbreakableTrapBobber.GetDescription())
                , (l) => LoadTackleQuest(BaitTackle.UnbreakableTrapBobber)
            );
            AddLetter
            (
                BaitTackle.UnbreakableCorkBobber
                , (l) => Game1.player.mailReceived.Contains(BaitTackle.UnbreakableTrapBobber.GetQuestName()) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.UnbreakableCorkBobber.GetDescription())
                , (l) => LoadTackleQuest(BaitTackle.UnbreakableCorkBobber)
            );
            AddLetter
            (
                BaitTackle.UnbreakableTreasureHunter
                , (l) => Game1.player.mailReceived.Contains(BaitTackle.UnbreakableCorkBobber.GetQuestName()) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.UnbreakableTreasureHunter.GetDescription())
                , (l) => LoadTackleQuest(BaitTackle.UnbreakableTreasureHunter)
            );
            AddLetter
            (
                BaitTackle.UnbreakableBarbedHook
                , (l) => Game1.player.mailReceived.Contains(BaitTackle.UnbreakableTreasureHunter.GetQuestName()) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.UnbreakableBarbedHook.GetDescription())
                , (l) => LoadTackleQuest(BaitTackle.UnbreakableBarbedHook)
            );
            AddLetter
            (
                BaitTackle.UnbreakableDressedSpinner
                , (l) => Game1.player.mailReceived.Contains(BaitTackle.UnbreakableBarbedHook.GetQuestName()) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.UnbreakableDressedSpinner.GetDescription())
                , (l) => LoadTackleQuest(BaitTackle.UnbreakableDressedSpinner)
            );
            MailDao.SaveLetter
            (
                new Letter
                (
                    "UnbreakableTackleReward"
                    , I18N.Get("UnbreakableTackleReward.Letter")
                    , new List<Item> { new StardewValley.Object(74,1) }
                    , (l) => Game1.player.mailReceived.Contains(BaitTackle.UnbreakableDressedSpinner.GetQuestName()) && !Game1.player.mailReceived.Contains(l.Id)
                    , (l) =>
                    {
                        Game1.player.mailReceived.Add(l.Id);
                    }
                )
            );
        }

        private static void LoadTackleQuest(BaitTackle baitTackle)
        {
            Quest quest = new Quest();
            quest.questType.Value = 1;
            string baitTackleName = I18N.Get($"{baitTackle.ToString()}.Name");
            quest.questTitle = baitTackleName;
            quest.questDescription = I18N.Get("Quest.Description", new { Item = baitTackleName });
            quest.currentObjective = I18N.Get("Quest.FirstObjective", new {Item = baitTackleName });
            quest.showNew.Value = true;
            quest.moneyReward.Value = 0;
            quest.rewardDescription.Value = (string) null;
            quest.canBeCancelled.Value = false;
            Game1.player.questLog.Add(quest);
            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2011"), 2));
        }

        private void AddLetter(BaitTackle baitTackle, Func<Letter, bool> condition, Action<Letter> callback = null,int whichBG = 0)
        {
            MailDao.SaveLetter(new Letter(baitTackle.ToString() + "Recipe", I18N.Get(baitTackle.ToString() + ".Letter"), baitTackle.GetDescription(), condition, callback, whichBG));
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\CraftingRecipes");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data\\CraftingRecipes"))
            {
                var data = asset.AsDictionary<string, string>().Data;
                AddRecipeData(data, BaitTackle.EverlastingBait, "684 10 74 1");
                AddRecipeData(data, BaitTackle.EverlastingWildBait, "771 100 684 50 766 20 74 1");
                AddRecipeData(data, BaitTackle.EverlastingMagnet, "335 10 74 1");
                AddRecipeData(data, BaitTackle.UnbreakableSpinner, "335 4 337 2");
                AddRecipeData(data, BaitTackle.UnbreakableLeadBobber, "117 1 337 3");
                AddRecipeData(data, BaitTackle.UnbreakableTrapBobber, "334 2 92 20 369 5 309 1 337 3");
                AddRecipeData(data, BaitTackle.UnbreakableCorkBobber, "388 20 709 10 766 20 557 1 337 4");
                AddRecipeData(data, BaitTackle.UnbreakableTreasureHunter, "336 8 337 4");
                AddRecipeData(data, BaitTackle.UnbreakableBarbedHook, "334 2 335 2 336 2 337 5");
                AddRecipeData(data, BaitTackle.UnbreakableDressedSpinner, "335 4 428 2 66 1 62 1 60 1 70 1 68 1 64 1 337 5");
            }
        }

        private string AddRecipeData(IDictionary<string, string> data, BaitTackle baitTackle, string recipe)
        {
            return data[baitTackle.GetDescription()] =  GetRecipeString(recipe, baitTackle);
        }

        private string GetRecipeString(string recipe, BaitTackle baitTackle)
        {
            var recipeString = $"{recipe}/Home/{(int)baitTackle} 1/false/null";
            if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
            {
                recipeString += "/" + I18N.Get($"{baitTackle.ToString()}.Name");
            }
            return recipeString;
        }

        private int GetNpcFriendship(string name)
        {
            if (Game1.player.friendshipData.ContainsKey(name))
            {
                return Game1.player.friendshipData[name].Points;
            }
            else
            {
                return 0;
            }
        }
    }
}
