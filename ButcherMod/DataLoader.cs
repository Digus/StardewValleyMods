using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ButcherMod.recipes;
using ButcherMod.tools;
using MailFrameworkMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace ButcherMod
{
    public class DataLoader : IAssetEditor
    {
        private readonly IModHelper _helper;

        public MeatCleaverLoader MeatCleaverLoader { get; }

        public RecipesLoader RecipeLoader { get; }

        public DataLoader(IModHelper helper)
        {
            this._helper = helper;

            MeatCleaverLoader = new MeatCleaverLoader(_helper.Content.Load<Texture2D>("tools/MeatCleaver.png"));
            RecipeLoader = new RecipesLoader();

            var editors = this._helper.Content.AssetEditors;
            editors.Add(this);
            editors.Add(MeatCleaverLoader);
            editors.Add(RecipeLoader);

            //this._helper.Content.InvalidateCache("Data\\CookingRecipes.xnb");

            MeatCleaverLoader.LoadMail();
            RecipeLoader.LoadMails();
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\ObjectInformation") || asset.AssetNameEquals("Data\\Bundles");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data\\ObjectInformation"))
            {
                var data = asset.AsDictionary<int, string>().Data;
                //MEAT
                data[639] = "Beef/100/15/Basic -14/Beef/Meat from a cow. Fatty and slightly sweet.";
                data[640] = "Pork/1250/30/Basic -14/Pork/Meat from a pig. Very nutritious.";
                data[641] = "Chicken/250/15/Basic -14/Chicken/The meat of a chicken. It's mild.";
                data[642] = "Duck/800/20/Basic -14/Duck/The meat of a duck. It's darker and richer than chicken meat.";
                data[643] = "Rabbit/2500/20/Basic -14/Rabbit/The meat of a rabbit. It's very lean.";
                data[644] = "Mutton/650/20/Basic -14/Mutton/The meat from sheep or goat.";
                //COOKING
                data[652] = "Meatloaf/370/90/Cooking - 7/Meatloaf/A dense meat casserole with a sweet tomato glaze./food/0 0 0 0 1 0 0 0 0 1 1/1440";
                data[653] = "Orange Chicken/250/65/Cooking - 7/Orange Chicken/It has a sweet, tangy sauce./food/0 0 2 0 2 0 0 0 0 0 0/600";
                data[654] = "Monte Cristo/620/120/Cooking - 7/Monte Cristo/It has a nice thick batter and is absolutely delicious./food/0 0 0 0 3 6 0 0 64 2 0/780";
                data[655] = "Bacon Cheeseburger/660/130/Cooking - 7/Bacon Cheeseburger/The best kind of burger./food/0 0 0 0 0 0 0 100 0 0 0/960";
                data[656] = "Roast Duck/410/100/Cooking - 7/Roast Duck/Simplicity at its best./food/0 0 0 0 0 0 0 0 0 1 6/780";
                data[657] = "Rabbit au Vin/570/110/Cooking - 7/Rabbit au Vin/A strong and sophisticated meal that will make you feel you can take on anything./food/0 0 0 0 4 0 0 0 64 2 4 4/1440";
                data[658] = "Steak Fajitas/415/100/Cooking - 7/Steak Fajitas/Spicy got a new level./food/0 0 0 0 0 0 0 0 0 2 0/300";
                data[659] = "Glazed Ham/550/105/Cooking - 7/Glazed Ham/The ham yields a moist, succulent, sweet taste./food/0 0 6 0 3 0 0 0 64 1 0/960";
                data[660] = "Summer Sausage/360/90/Cooking - 7/Summer Sausage/Lean beef, bacon and garlic make for a distinctive tangy flavor./food/0 0 0 0 0 0 0 0 0 0 2 2/780";
                data[661] = "Sweet and Sour Pork/450/105/Cooking - 7/Sweet and Sour Pork/A juicy pork with a nice crust, seasoned with the perfect balance of vinegar and sugar./food/6 0 0 0 3 0 0 0 32 2 0/780";
                data[662] = "Rabbit Stew/360/90/Cooking - 7/Rabbit Stew/A rustic and very hearty rabbit recipe that will warm you up to withstand anything./food/0 0 4 0 4 0 0 0 64 2 4/1440";
                data[663] = "Winter Duck/360/90/Cooking - 7/Winter Duck/This slightly sweet duck is perfect for festivities./food/0 0 0 0 6 0 0 0 0 0 0/1440";
                data[664] = "Steak with Mushrooms/510/105/Cooking - 7/Steak with Mushrooms/You will feel the strengh of the earth go through your body./food/0 0 0 0 0 0 0 0 0 2 3 6/1440";
                data[665] = "Cowboy Dinner/305/80/Cooking - 7/Cowboy Dinner/Meal of a champion farmer./food/4 0 4 0 3 4 0 50 0 1 0/960";
                data[666] = "Bacon/300/75/Cooking - 7/Bacon/It's bacon!/food/0 0 0 0 0 0 0 50 0 0 0/780";
            }
            else if (asset.AssetNameEquals("Data\\Bundles"))
            {
                var data = asset.AsDictionary<string, string>().Data;
                string value = data["Pantry/4"];
                if (!value.Contains("644 1 0") && value.Contains("/4/5"))
                {
                    value = value.Insert(value.LastIndexOf("/4/5"), " 644 1 0");
                }
                
                data["Pantry/4"] = value;
            }
        }
    }
}
