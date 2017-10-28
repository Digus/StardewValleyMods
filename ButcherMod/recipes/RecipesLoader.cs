using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailFrameworkMod;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;

namespace ButcherMod.recipes
{
    public class RecipesLoader : IAssetEditor
    {
        public MeatFridayChannel MeatFridayChannel { get; }

        public RecipesLoader()
        {
            MeatFridayChannel = new MeatFridayChannel();
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\CookingRecipes");
        }

        public void Edit<T>(IAssetData asset)
        {
            var data = asset.AsDictionary<string, string>().Data;
            data["Meatloaf"] = "639 2 256 1 248 1 -5 1 399 1/1 10/652/default";
            data["Orange Chicken"] = "641 1 635 1/1 10/653/default";
            data["Monte Cristo"] = "216 1 640 1 424 1 -5 1/1 10/654/default";
            data["Bacon Cheeseburger"] = "216 1 639 1 424 1 666 1/1 10/655/default";
            data["Roast Duck"] = "642 1/1 10/656/default";
            data["Rabbit au Vin"] = "643 1 348 1 404 1 399 1/1 10/657/default";
            data["Steak Fajitas"] = "639 2 247 1 260 2 399 2 229 1/1 10/658/default";
            data["Glazed Ham"] = "640 1 340 1 724 1 245 1/1 10/659/default";
            data["Summer Sausage"] = "639 2 666 1 248 1/1 10/660/default";
            data["Sweet and Sour Pork"] = "640 1 419 1 245 1 247 1/1 10/661/default";
            data["Rabbit Stew"] = "643 1 192 1 256 1 20 1 78 1/1 10/662/default";
            data["Winter Duck"] = "642 1 637 1 250 1 416 1/1 10/663/default";
            data["Steak with Mushrooms"] = "644 1 404 1 257 1 281 1 432 1/1 10/664/default";
            data["Cowboy Dinner"] = "644 1 207 1 194 1 270 1 426 1/1 10/665/default";
            data["Bacon"] = "640 1/1 10/666 4/default";
        }

        public void LoadMails()
        {
            string meatloafText = "@," +
                                  "^remember to eat healthy, or you won't have enough energy to work hard! I'm including one of my favorite recipes. Make sure to use ripe tomatoes! " +
                                  "^   -Lewis" +
                                  "^^P.S. Don't tell Marnie!";
            MailDao.SaveLetter(new Letter("meatloafRecipe", meatloafText, "Meatloaf", (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Lewis") >= 9 * 250 && GetNpcFriendship("Marnie") >= 7 * 250));

            string baconCheeseburgerText = "Dear @," +
                                           "^have you tried my burgers at the last fair? This recipe is even better. I only share this with my best friends! " +
                                           "^   -Gus";
            MailDao.SaveLetter(new Letter("baconCheeseburgerRecipe", baconCheeseburgerText, "Bacon Cheeseburger", (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Gus") >= 9 * 250 && SDate.Now() > new SDate(16, "fall", 1)));

            string sweetAndSourPorkText = "Dear @," +
                                          "^this recipe of mine won 1st place in a cooking competition! I think it is perfect for when I have lots of work to do in my garden. Thanks for being a friend to me and Kent! " +
                                          "^   -Jodi";
            MailDao.SaveLetter(new Letter("sweetAndSourPorkRecipe", sweetAndSourPorkText, "Sweet and Sour Pork", (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Jodi") >= 9 * 250 && GetNpcFriendship("Kent") >= 9 * 250));

            string glazedHamText = "Well, I know a couple of recipes." +
                                   "^Since you have been such a good client and friend, I thought I'd send you a secret one from my family... maybe it'll help you mine more ore or something." +
                                   "^Take care. " +
                                   "^   -Clint";
            Func<Letter, bool> glazedHamCondition = (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe)
                                         && GetNpcFriendship("Clint") >= 9
                                         && Game1.stats.GeodesCracked > 60
                                         && Game1.player.getToolFromName("Pickaxe").upgradeLevel > 2
                                         && Game1.player.getToolFromName("Axe").upgradeLevel > 0
                                         && Game1.player.getToolFromName("Watering Can").upgradeLevel > 0
                                         && Game1.player.getToolFromName("Hoe").upgradeLevel > 0;
            MailDao.SaveLetter(new Letter("glazedHamRecipe", glazedHamText, "Glazed Ham", glazedHamCondition));

            string greatingsMale = "Mr. @,";
            string greatingsFamele = "Ms. @,";
            string cowboyDinnerBaseText = "^thank you again for all your contributions. " +
                                          "^You might not know, but I have not always been a museum curator. Before discovering my true vocation, I worked as a cowboy. " +
                                          "^This recipe I'm sending you is a tradition where I came from. It's not much, but consider it a personal thank you from me." +
                                          "^   -Gunther";
            MailDao.SaveLetter(new Letter("cowboyDinnerkRecipe", greatingsMale + cowboyDinnerBaseText + "¦" + greatingsFamele + cowboyDinnerBaseText, "Cowboy Dinner", (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && (Game1.getLocationFromName("ArchaeologyHouse") as LibraryMuseum)?.museumPieces.Count >= 70));

            string rabbitStewText = "@," +
                                    "^how are you doing? You should be careful when exploring the mines. I've enclosed some instructions on how to make a meal I eat before going there. " +
                                    "^I'm one of the few people in the valley who can catch a wild rabbit. You can find plenty of cave carrots in the mines. Leeks are every where during spring. You can get potatoes and tomatoes from..." +
                                    "^I'm sure you will figure it out." +
                                    "^   -Linus";
            MailDao.SaveLetter(new Letter("rabbitStewRecipe", rabbitStewText, "Rabbit Stew", (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Linus") >= 9 * 250 && (Game1.stats.TimesUnconscious >= 1 || Game1.player.deepestMineLevel >= 100)));

            string monteCristoText = "Dear @," +
                                     "^I see you like to forage as much as I do, so let me share a secret with you. I don't like sandwichs very much, but there is one exception. When I go for a long hiking at Cindersap Forest, I always take this one with me. It helps me focus on finding the best quality stuff. " +
                                     "^I hope you like it too." +
                                     "^   -Leah";
            MailDao.SaveLetter(new Letter("monteCristoRecipe", monteCristoText, "Monte Cristo", (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Leah") >= 8 * 250 && Game1.stats.ItemsForaged >= 1200));

            string steakWithMushroomsText = "@," +
                                            "^I heard you've been killing lots of monsters. " +
                                            "^My coach used to prepare a special meal for us before an important game. It was supposed to make us faster and stronger. " +
                                            "^So I asked him for the recipe to give you. " +
                                            "^It's also really tasty." +
                                            "^   -Alex";
            MailDao.SaveLetter(new Letter("steakWithMushroomsRecipe", steakWithMushroomsText, "Steak with Mushrooms", (letter) => !Game1.player.cookingRecipes.ContainsKey(letter.Recipe) && GetNpcFriendship("Alex") >= 8 * 250 && Game1.stats.MonstersKilled >= 1000));
        }

        private int GetNpcFriendship(string name)
        {
            if (Game1.player.friendships.ContainsKey(name))
            {
                return Game1.player.friendships[name][0];
            }
            else
            {
                return 0;
            }
        }
    }
}
