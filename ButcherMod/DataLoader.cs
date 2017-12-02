using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ButcherMod.cooking;
using ButcherMod.meats;
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
using ButcherMod.animals;

namespace ButcherMod
{
    public class DataLoader : IAssetEditor
    {
        public static IModHelper Helper;
        public static ITranslationHelper i18n;
        public static MeatData MeatData;
        public static CookingData CookingData;
        public static AnimalData AnimalData;
        public static AnimalBuildingData AnimalBuildingData;

        public static ModConfig ModConfig;

        public ToolsLoader MeatCleaverLoader { get; }

        public RecipesLoader RecipeLoader { get; }

        public DataLoader(IModHelper helper)
        {
            Helper = helper;
            i18n = Helper.Translation;

            ModConfig = helper.ReadConfig<ModConfig>();

            MeatCleaverLoader = new ToolsLoader(Helper.Content.Load<Texture2D>("tools/Tools.png"), Helper.Content.Load<Texture2D>("tools/MenuTiles.png"));
            RecipeLoader = new RecipesLoader();

            var editors = Helper.Content.AssetEditors;
            editors.Add(this);
            editors.Add(MeatCleaverLoader);
            editors.Add(RecipeLoader);

            AnimalBuildingData = DataLoader.Helper.ReadJsonFile<AnimalBuildingData>("data\\animalBuilding.json") ?? new AnimalBuildingData();
            DataLoader.Helper.WriteJsonFile("data\\animalBuilding.json", AnimalBuildingData);

            AnimalData = DataLoader.Helper.ReadJsonFile<AnimalData>("data\\animals.json") ?? new AnimalData();
            DataLoader.Helper.WriteJsonFile("data\\animals.json", AnimalData);

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
                MeatData = DataLoader.Helper.ReadJsonFile<MeatData>("data\\meats.json") ?? new MeatData();
                DataLoader.Helper.WriteJsonFile("data\\meats.json", MeatData);

                data[(int)Meat.Beef] = Meat.Beef.GetObjectString();
                data[(int)Meat.Pork] = Meat.Pork.GetObjectString();
                data[(int)Meat.Chicken] = Meat.Chicken.GetObjectString();
                data[(int)Meat.Duck] = Meat.Duck.GetObjectString();
                data[(int)Meat.Rabbit] = Meat.Rabbit.GetObjectString();
                data[(int)Meat.Mutton] = Meat.Mutton.GetObjectString();

                //COOKING
                CookingData = Helper.ReadJsonFile<CookingData>("data\\cooking.json") ?? new CookingData();
                Helper.WriteJsonFile("data\\cooking.json", CookingData);

                data[(int)Cooking.Meatloaf] = Cooking.Meatloaf.GetObjectString();
                data[(int)Cooking.OrangeChicken] = Cooking.OrangeChicken.GetObjectString();
                data[(int)Cooking.MonteCristo] = Cooking.MonteCristo.GetObjectString();
                data[(int)Cooking.BaconCheeseburger] = Cooking.BaconCheeseburger.GetObjectString();
                data[(int)Cooking.RoastDuck] = Cooking.RoastDuck.GetObjectString();
                data[(int)Cooking.RabbitAuVin] = Cooking.RabbitAuVin.GetObjectString();
                data[(int)Cooking.SteakFajitas] = Cooking.SteakFajitas.GetObjectString();
                data[(int)Cooking.GlazedHam] = Cooking.GlazedHam.GetObjectString();
                data[(int)Cooking.SummerSausage] = Cooking.SummerSausage.GetObjectString();
                data[(int)Cooking.SweetAndSourPork] = Cooking.SweetAndSourPork.GetObjectString();
                data[(int)Cooking.RabbitStew] = Cooking.RabbitStew.GetObjectString();
                data[(int)Cooking.WinterDuck] = Cooking.WinterDuck.GetObjectString();
                data[(int)Cooking.SteakWithMushrooms] = Cooking.SteakWithMushrooms.GetObjectString();
                data[(int)Cooking.CowboyDinner] = Cooking.CowboyDinner.GetObjectString();
                data[(int)Cooking.Bacon] = Cooking.Bacon.GetObjectString();
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
