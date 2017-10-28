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
    public class MeatCleaverLoader : IAssetEditor
    {
        private readonly Texture2D _meatCleaverSpriteSheet;

        public MeatCleaverLoader(Texture2D meatCleaverSpriteSheet)
        {
            _meatCleaverSpriteSheet = meatCleaverSpriteSheet;
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("TileSheets\\tools");
        }

        public void Edit<T>(IAssetData asset)
        {
            Texture2D toolSpriteSheet = asset.AsImage().Data;
            int originalWidth = toolSpriteSheet.Width;
            int originalHeight = toolSpriteSheet.Height;
            Color[] data1 = new Color[originalWidth * originalHeight];
            toolSpriteSheet.GetData<Color>(data1);
            Texture2D meatCleaverSpriteSheet = _meatCleaverSpriteSheet;
            int meatCleaverWidth = meatCleaverSpriteSheet.Width;
            int meatCleaverlHeight = meatCleaverSpriteSheet.Height;
            Color[] data2 = new Color[meatCleaverWidth * meatCleaverlHeight];
            meatCleaverSpriteSheet.GetData<Color>(data2);
            Texture2D newSpriteSheet = new Texture2D(Game1.game1.GraphicsDevice, originalWidth, originalHeight + meatCleaverlHeight, false, SurfaceFormat.Color);

            var data3 = new Color[data1.Length + data2.Length];
            data1.CopyTo(data3, 0);
            data2.CopyTo(data3, data1.Length);

            newSpriteSheet.SetData(data3);

            asset.ReplaceWith(newSpriteSheet);

            var newToolInitialParentIdex = (originalWidth / 16) * (originalHeight / 16);
            MeatCleaver.initialParentTileIndex = newToolInitialParentIdex;
            MeatCleaver.indexOfMenuItemView = newToolInitialParentIdex + 26;
        }

        public void LoadMail()
        {
            string meatCleaverText = "Dear Farmer @," +
                       "^^    We know you are the proud owner of a farm animal. Congratulations!" +
                       "^    Here in our world, we love animals very deeply, but unfortunately they can't breed. So, we are always looking to get more from other worlds." +
                       "^    We know you humans love meat, and you might one day want to kill your animals for their meat... DONT'T DO THAT!" +
                       "^^    So here is the deal. To our luck, meat here grows on tree. So just send your animals to us and we will send you some meat in exchange. We promise to take really good care of them." +
                       "^    We have sent you a magic wand, just use it on the animal you want meat from to make the exchange. The more loved it is, the more meat we will give you. We might even add some extra items depending on which animal you send us." +
                       "^    -Your interdimensional friend" +
                       "^^P.S. We know the wand looks like a Meat Cleaver, we had to do it like that to please the bloodlust of some humans.";

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

            MailDao.SaveLetter(new Letter("meatCleaver", meatCleaverText, new List<Item> { new MeatCleaver() }, MeatCleaverCondition));
        }
    }
}
