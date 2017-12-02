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

namespace ButcherMod.tools
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

            bool MeatCleaverCondition(Letter l)
            {
                bool hasAnimal = Game1.locations.Exists((location) =>
                {
                    if (location is Farm farm)
                    {
                        return farm.buildings.Exists((b => (b.indoors as AnimalHouse)?.animalsThatLiveHere.Count > 0));
                    }
                    return false;
                });
                bool hasMeatCleaver = Game1.player.items.Exists(i => i is MeatCleaver);
                hasMeatCleaver |= Game1.locations.Exists((location) =>
                {
                    if (location.objects.Values.ToList()
                        .Exists((o) =>
                        {
                            if (o is Chest chest)
                            {
                                return chest.items.Exists((ci) => ci is MeatCleaver);
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
                                            return chest.items.Exists((ci) => ci is MeatCleaver);
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
                return hasAnimal && !hasMeatCleaver;
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
                bool hasInseminationSyringe = Game1.player.items.Exists(i => i is InseminationSyringe);
                hasInseminationSyringe |= Game1.locations.Exists((location) =>
                {
                    if (location.objects.Values.ToList()
                        .Exists((o) =>
                        {
                            if (o is Chest chest)
                            {
                                return chest.items.Exists((ci) => ci is InseminationSyringe);
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
                                            return chest.items.Exists((ci) => ci is InseminationSyringe);
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
                return hasAnimalInValidBuildings && !hasInseminationSyringe;
            }

            MailDao.SaveLetter(new Letter("meatCleaver", meatCleaverText, new List<Item> { new MeatCleaver() }, MeatCleaverCondition));

            MailDao.SaveLetter(new Letter("inseminationSyringe", DataLoader.i18n.Get("Tool.InseminationSyringe.Letter"), new List<Item> { new InseminationSyringe() }, InseminationSyringeCondition));
        }
    }
}
