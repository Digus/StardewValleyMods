using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailFrameworkMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using DataLoader = AnimalHusbandryMod.common.DataLoader;

namespace AnimalHusbandryMod.tools
{
    public class ToolsLoader : IAssetEditor
    {
        private readonly Texture2D _toolsSpriteSheet;
        private readonly Texture2D _menuTilesSpriteSheet;

        public ToolsLoader(Texture2D toolsSpriteSheet, Texture2D menuTilesSpriteSheet)
        {
            _toolsSpriteSheet = toolsSpriteSheet;
            _menuTilesSpriteSheet = menuTilesSpriteSheet;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("TileSheets\\tools") || asset.AssetNameEquals("Maps\\MenuTiles");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("TileSheets\\tools"))
            {
                Texture2D toolSpriteSheet = asset.AsImage().Data;
                int originalWidth = toolSpriteSheet.Width;
                int originalHeight = toolSpriteSheet.Height;
                Color[] data1 = new Color[originalWidth * originalHeight];
                toolSpriteSheet.GetData<Color>(data1);
                Texture2D customToolsSpriteSheet = _toolsSpriteSheet;
                int meatCleaverWidth = customToolsSpriteSheet.Width;
                int meatCleaverlHeight = customToolsSpriteSheet.Height;
                Color[] data2 = new Color[meatCleaverWidth * meatCleaverlHeight];
                customToolsSpriteSheet.GetData<Color>(data2);
                Texture2D newSpriteSheet = new Texture2D(Game1.game1.GraphicsDevice, originalWidth, originalHeight + meatCleaverlHeight, false, SurfaceFormat.Color);

                var data3 = new Color[data1.Length + data2.Length];
                data1.CopyTo(data3, 0);
                data2.CopyTo(data3, data1.Length);

                newSpriteSheet.SetData(data3);

                asset.ReplaceWith(newSpriteSheet);

                var newToolInitialParentIdex = (originalWidth / 16) * (originalHeight / 16);

                int offset = 0;
                if (DataLoader.ModConfig.Softmode)
                {
                    offset = 7;
                }

                MeatCleaver.initialParentTileIndex = newToolInitialParentIdex + offset;
                MeatCleaver.indexOfMenuItemView = newToolInitialParentIdex + 26 + offset;
                InseminationSyringe.InitialParentTileIndex = newToolInitialParentIdex + 14;
                InseminationSyringe.IndexOfMenuItemView = newToolInitialParentIdex + 14;
                FeedingBasket.InitialParentTileIndex = newToolInitialParentIdex + 15;
                FeedingBasket.IndexOfMenuItemView = newToolInitialParentIdex + 15;
                LoadMail();
            } else if (asset.AssetNameEquals("Maps\\MenuTiles"))
            {
                Texture2D menuTilesSpriteSheet = asset.AsImage().Data;
                int originalWidth = menuTilesSpriteSheet.Width;
                int originalHeight = menuTilesSpriteSheet.Height;
                Color[] data1 = new Color[originalWidth * originalHeight];
                menuTilesSpriteSheet.GetData<Color>(data1);
                Texture2D customMenuTilesSpriteSheet = _menuTilesSpriteSheet;
                int customMenuTilesWidth = customMenuTilesSpriteSheet.Width;
                int customMenuTileslHeight = customMenuTilesSpriteSheet.Height;
                Color[] data2 = new Color[customMenuTilesWidth * customMenuTileslHeight];
                customMenuTilesSpriteSheet.GetData<Color>(data2);
                Texture2D newSpriteSheet = new Texture2D(Game1.game1.GraphicsDevice, originalWidth, originalHeight + customMenuTileslHeight, false, SurfaceFormat.Color);

                var data3 = new Color[data1.Length + data2.Length];
                data1.CopyTo(data3, 0);
                data2.CopyTo(data3, data1.Length);

                newSpriteSheet.SetData(data3);

                asset.ReplaceWith(newSpriteSheet);

                var newMenuTitlesInitialParentIdex = (originalWidth / 64) * (originalHeight / 64);                

                InseminationSyringe.AttachmentMenuTile = newMenuTitlesInitialParentIdex;
                FeedingBasket.AttachmentMenuTile = newMenuTitlesInitialParentIdex + 1;
            }
        }

        internal void ReplaceOldTools(object sender, EventArgs e)
        {
            List<Item> inventory = Game1.player.items;
            for (int i = 0; i < inventory.Count; i++)
            {
                ReplaceIfOldItem(inventory, i);

            }
            List<GameLocation> locations = Game1.locations;
            for (int i = 0; i < locations.Count; i++)
            {
                var location = locations[i];
                ReplaceInLocationChests(location);

                if (location is BuildableGameLocation bgl)
                {
                    bgl.buildings.ForEach(((b) =>
                    {
                        if (b.indoors is GameLocation gl)
                        {
                            ReplaceInLocationChests(gl);
                        }
                    }));
                }
            }
        }

        private static void ReplaceInLocationChests(GameLocation location)
        {
            var objects = location.objects.Values;
            for (int j = 0; j < objects.Count; j++)
            {
                var o = objects.ToList()[j];
                if (o is Chest chest)
                {
                    List<Item> items = chest.items;
                    for (int k = 0; k < items.Count; k++)
                    {
                        ReplaceIfOldItem(items, k);
                    }
                }
            }
        }

        private static void ReplaceIfOldItem(List<Item> items, int i)
        {
            Item item = items[i];
            if (item != null)
            {
                if (item.Name.Contains("ButcherMod.MeatCleaver"))
                {
                    items[i] = new MeatCleaver();
                    AnimalHusbandryModEntery.monitor.Log($"An older version of the MeatCleaver found. Replacing it with the new one.", LogLevel.Debug);
                }
                else if (item.Name.Contains("ButcherMod.tools.InseminationSyringe"))
                {
                    items[i] = new InseminationSyringe();
                    AnimalHusbandryModEntery.monitor.Log($"An older version of the InseminationSyringe found. Replacing it with the new one.", LogLevel.Debug);
                }
            }
        }

        public void LoadMail()
        {

            string meatCleaverText;
            if (DataLoader.ModConfig.Softmode)
            {
                meatCleaverText = DataLoader.i18n.Get("Tool.MeatCleaver.Letter.Soft");
            }
            else
            {
                meatCleaverText = DataLoader.i18n.Get("Tool.MeatCleaver.Letter");
            }


            bool HasAnimal()
            {
                return Game1.locations.Exists((location) =>
                {
                    if (location is Farm farm)
                    {
                        return farm.buildings.Exists((b => (b.indoors as AnimalHouse)?.animalsThatLiveHere.Count > 0));
                    }
                    return false;
                });
            }

            bool MeatCleaverCondition(Letter l)
            {
                return HasAnimal() && !HasTool(typeof(MeatCleaver));
            }

            List<string> validBuildingsForInsemination = new List<string>(new string[] { "Deluxe Barn", "Big Barn", "Deluxe Coop" });
            bool InseminationSyringeCondition(Letter l)
            {
                bool hasAnimalInValidBuildings = Game1.locations.Exists((location) =>
                {
                    if (location is Farm farm)
                    {
                        return farm.buildings
                        .Exists((b) => 
                        {
                            return (b.indoors as AnimalHouse)?.animalsThatLiveHere.Count > 0 && validBuildingsForInsemination.Contains((b.indoors as AnimalHouse)?.name);
                        });
                    }
                    return false;
                });
               
                return hasAnimalInValidBuildings && !HasTool(typeof(InseminationSyringe));
            }

            bool FeedingBasketCondition(Letter l)
            {

                return !Game1.player.mailReceived.Contains("feedingBasket") && Game1.player.getFriendshipHeartLevelForNPC("Marnie") >= 2 && (Game1.player.hasPet() || HasAnimal());
            }

            bool FeedingBasketRedeliveryCondition(Letter l)
            {
                
                return Game1.player.mailReceived.Contains("feedingBasket") && !HasTool(typeof(FeedingBasket)) && Game1.player.getFriendshipHeartLevelForNPC("Marnie") >= 6;
            }

            if (!DataLoader.ModConfig.DisableMeat)
            {
                MailDao.SaveLetter(new Letter("meatCleaver", meatCleaverText, new List<Item> { new MeatCleaver() }, MeatCleaverCondition));
            }
            
            if (!DataLoader.ModConfig.DisablePregnancy)
            {
                MailDao.SaveLetter(new Letter("inseminationSyringe", DataLoader.i18n.Get("Tool.InseminationSyringe.Letter"), new List<Item> { new InseminationSyringe() }, InseminationSyringeCondition));
            }


            MailDao.SaveLetter(new Letter("participantRibbon", DataLoader.i18n.Get("Tool.ParticipantRibbon.Letter"), new List<Item> { new ParticipantRibbon() }, (l)=> true));


            if (!DataLoader.ModConfig.DisableTreats)
            {
                MailDao.SaveLetter
                (
                    new Letter
                    (
                        "feedingBasket",
                        DataLoader.i18n.Get("Tool.FeedingBasket.Letter"),
                        new List<Item> {new FeedingBasket()},
                        FeedingBasketCondition,
                        (l)=> Game1.player.mailReceived.Add(l.Id)
                    )
                );
                MailDao.SaveLetter
                (
                    new Letter
                    (
                        "feedingBasketRedelivery",
                        DataLoader.i18n.Get("Tool.FeedingBasket.LetterRedelivery"),
                        new List<Item> { new FeedingBasket() },
                        FeedingBasketRedeliveryCondition
                    )
                );
            }
        }

        private bool HasTool(Type toolClass)
        {
            bool hasInInventory = Game1.player.items.Exists(toolClass.IsInstanceOfType);
            return hasInInventory || Game1.locations.Exists((location) =>
            {
                if (location.objects.Values.ToList()
                    .Exists((o) =>
                    {
                        if (o is Chest chest)
                        {
                            return chest.items.Exists(toolClass.IsInstanceOfType);
                        }
                        return false;
                    }))
                {
                    return true;
                }
                if (location is BuildableGameLocation bgl)
                {
                    return bgl.buildings.Exists(((b) =>
                    {
                        if (b.indoors is GameLocation gl)
                        {
                            if (gl.objects.Values.ToList()
                                .Exists((o) =>
                                {
                                    if (o is Chest chest)
                                    {
                                        return chest.items.Exists(toolClass.IsInstanceOfType);
                                    }
                                    return false;
                                }))
                            {
                                return true;
                            }
                        }
                        return false;
                    }));
                }
                return false;
            });
        }
    }
}
