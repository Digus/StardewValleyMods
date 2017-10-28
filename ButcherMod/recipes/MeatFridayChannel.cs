using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomTV;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;

namespace ButcherMod.recipes
{
    public class MeatFridayChannel
    {
        public const string ChannelDisplayName = "The Queen Of Sauce - Meat Friday";
        public const string ReRunDisplaySuffix = " (Re-run)";

        private TemporaryAnimatedSprite _queenSprite;

        private readonly Dictionary<int, string> _recipes;

        public MeatFridayChannel()
        {
            this._recipes = new Dictionary<int, string>();
            _recipes.Add(2, "Roast Duck/Roast Duck! This is an easy recipe to learn how to work with meat. You will need a piece of duck meat and the seasonings of you choice. I suggest salt and black pepper mixed on melted butter. Some people says eating this meal makes your body as tough as the crisp skin of the duck. So, let's get started...");
            _recipes.Add(3, "Bacon/Bacon! Everyone knows a recipes that uses bacon, but not everyone knows how to get a piece of pork and turn into this delicious meat, that goes well with almost anything. That's what I'm teaching you today, now listen closely...");
            _recipes.Add(4, "Summer Sausage/Summer Sausage! Do you know why it get its name from the season? Well, using lactic acid fermentation, curing, and smoking, sausage makers were able to make a product that was not only delicious, but could be kept without refrigeration... even in the summer! For my sausage I like to use beef, bacon and for a spice flavour, garlic.");
            _recipes.Add(5, "Orange Chicken/Orange Chicken! Fall is here and you were wandering what to do with all this orange you got from the summer. Worry no more, today's dish is as tasty as it's easy to make. Start squishing while you are listening to me...");
            _recipes.Add(6, "Steak Fajitas/Steak Fajitas! Are you starting to feel rusty with the cold whether coming? After eating my Fajitas you will be so hot you'll think you can run from Zuzu to Stardew in minutes. Get some beef, spring onions and hot peppers. Now double it all up! Let's fry it on heated oil... mmmm... My eyes are already watering...");
            _recipes.Add(7, "Rabbit au Vin/Rabbit au Vin! Probably the fanciest recipe I have ever cooked. If impress some one is what you are looking for, I guarantee you will succeed! It looks fabulous, smells divine and has the taste of heaven. Now, start taking notes, you'll gonna need it...");
            _recipes.Add(8, "Winter Duck/Winter Duck! I saved this one for the last Meat Friday of the year because it goes great with the festivities. Tales of good fortune always surrounded pomegranate, and this is something welcomed when a new year is about to start. But it's not easy to cook snow yam, so here are my tips...");
        }

        public void CheckChannelDay()
        {
            CustomTVMod.removeChannel("MeatFriday");

            if(SDate.Now().DayOfWeek == DayOfWeek.Friday)
            {
                int recipe = GetRecipeNumber();
                if (recipe >= 2)
                {
                    Boolean rerun = Game1.stats.DaysPlayed % 2 == 0U;

                    string name = ChannelDisplayName;
                    if (rerun)
                    {
                        name += ReRunDisplaySuffix;
                    }
                    CustomTVMod.addChannel("MeatFriday", name, ShowQueenAnnouncement);
                }
            }
        }

        private static int GetRecipeNumber()
        {
            return (int)(Game1.stats.DaysPlayed % 112U / 14) + 1;
        }

        private void ShowQueenAnnouncement(TV tv, TemporaryAnimatedSprite sprite, StardewValley.Farmer farmer, string answer)
        {
            _queenSprite = new TemporaryAnimatedSprite(Game1.mouseCursors, new Rectangle(602, 361, 42, 28), 150f, 2, 999999, tv.getScreenPosition(), false, false, (float)((double)(tv.boundingBox.Bottom - 1) / 10000.0 + 9.99999974737875E-06), 0.0f, Color.White, tv.getScreenSizeModifier(), 0.0f, 0.0f, 0.0f, false);
            string announcementText = @"Greetings! It is I, the queen of sauce... here to teach you a new meat recipe from my secret cookbook. This friday's dish...";
            CustomTVMod.showProgram(_queenSprite, announcementText, ShowRecipePresentation);
        }

        private void ShowRecipePresentation()
        {
            string text = _recipes[GetRecipeNumber()].Split('/')[1];
            CustomTVMod.showProgram(_queenSprite, text, AddRecipe);
        }

        private void AddRecipe()
        {
            string recipeName = _recipes[GetRecipeNumber()].Split('/')[0];
            string addRecipeText;
            if (!Game1.player.cookingRecipes.ContainsKey(recipeName))
            {
                addRecipeText = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13153", (object) recipeName);
                Game1.player.cookingRecipes.Add(recipeName, 0);
            }
            else
            {
                addRecipeText = Game1.content.LoadString("Strings\\StringsFromCSFiles:TV.cs.13151", (object) recipeName);
            }
            CustomTVMod.showProgram(_queenSprite, addRecipeText, CustomTVMod.endProgram);
        }
    }
}
