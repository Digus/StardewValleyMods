using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CropTransplantMod.integrations;
using MailFrameworkMod;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace CropTransplantMod
{
    public class DataLoader
    {
        public static IModHelper Helper;
        public static ITranslationHelper I18N;
        public static ModConfig ModConfig;

        public DataLoader(IModHelper helper, IManifest manifest)
        {
            Helper = helper;
            I18N = helper.Translation;
            ModConfig = helper.ReadConfig<ModConfig>();

            MailDao.SaveLetter
            (
                new Letter
                (
                    "CropTransplantLetter"
                    , I18N.Get("CropTransplant.Letter")
                    , (l) => !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("CropTransplantPotLetter") && Game1.player.craftingRecipes.ContainsKey("Garden Pot") && !ModConfig.GetGardenPotEarlier
                    , (l) => Game1.player.mailReceived.Add(l.Id)
                )
            );

            MailDao.SaveLetter
            (
                new Letter
                (
                    "CropTransplantPotLetter"
                    , I18N.Get("CropTransplantPot.Letter")
                    , new List<Item>() { new Object(Vector2.Zero, 62, false)}
                    , (l) => !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("CropTransplantLetter") && GetNpcFriendship("Evelyn") >= 2 * 250 && ModConfig.GetGardenPotEarlier
                    , (l) => Game1.player.mailReceived.Add(l.Id)
                )
            );

            CreateConfigMenu(manifest);
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

        private void CreateConfigMenu(IManifest manifest)
        {
            GenericModConfigMenuApi api = Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                api.RegisterModConfig(manifest, () => DataLoader.ModConfig = new ModConfig(), () => Helper.WriteConfig(DataLoader.ModConfig));

                api.RegisterSimpleOption(manifest, "Get Garden Pot Earlier", "Evelyn will send you a Garden Pot once you reach 2 hearts level of friendship with her. You will not learn the recipe though, for that you should get the greenhouse as normal.", () => DataLoader.ModConfig.GetGardenPotEarlier, (bool val) => DataLoader.ModConfig.GetGardenPotEarlier = val);
                
                api.RegisterSimpleOption(manifest, "Soil Under Trees", "A soil tile will be shown under trees when planted on stone floor, wood and other 'not plantable' soil. Some tiles aren't correct labeled by the game, so the soil tile may appears on places you would think it's not needed", () => DataLoader.ModConfig.EnableSoilTileUnderTrees, (bool val) => DataLoader.ModConfig.EnableSoilTileUnderTrees = val);

                api.RegisterLabel(manifest, "Crop Properties:", "Properties for crops transplant.");

                api.RegisterSimpleOption(manifest, "Place Outside The Farm", "You'll be able to place Garden Pot holding a crop on outdoor areas out of the farm. You can also put crops on hoed tiles out of the farm.", () => DataLoader.ModConfig.EnablePlacementOfCropsOutsideOutOfTheFarm, (bool val) => DataLoader.ModConfig.EnablePlacementOfCropsOutsideOutOfTheFarm = val);

                api.RegisterSimpleOption(manifest,"Transplant Energy Cost", "The cost of energy for lifting a crop from the ground. The energy cost decrease as you level up the farming skill. Level 10 will cost 50% less energy than the base cost.", () => DataLoader.ModConfig.CropTransplantEnergyCost, (float val) => DataLoader.ModConfig.CropTransplantEnergyCost = val);

                api.RegisterLabel(manifest, "Tree Properties:", "Properties for regular tree transplant.");

                api.RegisterClampedOption(manifest, "Max Stage", "The max stage a tree can be lifted. 0 to disable the lifting of any tree. 5 will enable all stages.", () => DataLoader.ModConfig.TreeTransplantMaxStage, (int val) => DataLoader.ModConfig.TreeTransplantMaxStage = val, 0, 5);

                api.RegisterSimpleOption(manifest, "Place On Any Tile", "﻿You'll be able to place trees on any unoccupied tile type.", () => DataLoader.ModConfig.EnablePlacementOfTreesOnAnyTileType, (bool val) => DataLoader.ModConfig.EnablePlacementOfTreesOnAnyTileType = val);

                api.RegisterSimpleOption(manifest, "Stage 1 Energy Cost", "The cost of energy for lifting a tree stage 1(seed) from the ground. Check the wiki for the stage images.", () => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[0], (float val) => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[0] = val);

                api.RegisterSimpleOption(manifest, "Stage 2 Energy Cost", "The cost of energy for lifting a tree stage 2(sprout) from the ground. Check the wiki for the stage images.", () => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[1], (float val) => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[1] = val);

                api.RegisterSimpleOption(manifest, "Stage 3 Energy Cost", "The cost of energy for lifting a tree stage 3(sapling) from the ground. Check the wiki for the stage images.", () => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[2], (float val) => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[2] = val);

                api.RegisterSimpleOption(manifest, "Stage 4 Energy Cost", "The cost of energy for lifting a tree stage 4(bush) from the ground. Check the wiki for the stage images.", () => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[3], (float val) => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[3] = val);

                api.RegisterSimpleOption(manifest, "Stage 5 Energy Cost", "The cost of energy for lifting a tree stage 5(tree) from the ground. Check the wiki for the stage images.", () => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[4], (float val) => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[4] = val);

                api.RegisterLabel(manifest, "Fruit Tree Properties:", "Properties for fruit tree transplant.");

                api.RegisterClampedOption(manifest, "Max Stage", "The max stage a fruit tree can be lifted. 0 to disable the lifting of any fruit tree. 5 will enable all stages.", () => DataLoader.ModConfig.TreeTransplantMaxStage, (int val) => DataLoader.ModConfig.FruitTreeTransplantMaxStage = val, 0, 5);

                api.RegisterSimpleOption(manifest, "Place Outside The Farm", "﻿You'll be able to place fruit trees out of the farm.", () => DataLoader.ModConfig.EnablePlacementOfFruitTreesOutOfTheFarm, (bool val) => DataLoader.ModConfig.EnablePlacementOfFruitTreesOutOfTheFarm = val);

                api.RegisterSimpleOption(manifest, "Place On Any Tile", "You'll be able to place fruit trees on any unoccupied tile type.", () => DataLoader.ModConfig.EnablePlacementOfFruitTreesOnAnyTileType, (bool val) => DataLoader.ModConfig.EnablePlacementOfFruitTreesOnAnyTileType = val);

                api.RegisterSimpleOption(manifest, "Place Next To Another Tree", "You'll be able to place fruit trees next to other trees. They will still not mature if to close to other stuff.", () => DataLoader.ModConfig.EnablePlacementOfFruitTreesNextToAnotherTree, (bool val) => DataLoader.ModConfig.EnablePlacementOfFruitTreesNextToAnotherTree = val);

                api.RegisterSimpleOption(manifest, "Place Blocked Growth", "You'll be able to place immature fruit trees where they will not mature.", () => DataLoader.ModConfig.EnablePlacementOfFruitTreesBlockedGrowth, (bool val) => DataLoader.ModConfig.EnablePlacementOfFruitTreesBlockedGrowth = val);

                api.RegisterSimpleOption(manifest, "Stage 1 Energy Cost", "The cost of energy for lifting a fruit tree stage 1(sapling) from the ground. Check the wiki for the stage images.", () => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[0], (float val) => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[0] = val);

                api.RegisterSimpleOption(manifest, "Stage 2 Energy Cost", "The cost of energy for lifting a fruit tree stage 2(small bush) from the ground. Check the wiki for the stage images.", () => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[1], (float val) => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[1] = val);

                api.RegisterSimpleOption(manifest, "Stage 3 Energy Cost", "The cost of energy for lifting a fruit tree stage 3(large bush) from the ground. Check the wiki for the stage images.", () => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[2], (float val) => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[2] = val);

                api.RegisterSimpleOption(manifest, "Stage 4 Energy Cost", "The cost of energy for lifting a fruit tree stage 4(small tree) from the ground. Check the wiki for the stage images.", () => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[3], (float val) => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[3] = val);

                api.RegisterSimpleOption(manifest, "Stage 5 Energy Cost", "The cost of energy for lifting a fruit tree stage 5(tree) from the ground. Check the wiki for the stage images.", () => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[4], (float val) => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[4] = val);

                api.RegisterLabel(manifest, "Tea Bush Properties:", "Properties for tea bush transplant.");

                api.RegisterSimpleOption(manifest, "Place Outside The Farm", "You'll be able to place tea bush out of the farm.", () => DataLoader.ModConfig.EnableToPlantTeaBushesOutOfTheFarm, (bool val) => DataLoader.ModConfig.EnableToPlantTeaBushesOutOfTheFarm = val);

                api.RegisterSimpleOption(manifest, "Place On Any Tile", "You'll be able to place tea bush on any unoccupied tile type.", () => DataLoader.ModConfig.EnableToPlantTeaBushesOnAnyTileType, (bool val) => DataLoader.ModConfig.EnableToPlantTeaBushesOnAnyTileType = val);
            }
        }
    }
}
