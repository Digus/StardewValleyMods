using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AnimalHusbandryMod.common
{
    public class EventsOverrides
    {
        private static Texture2D spring_outdoorsTileSheet = DataLoader.Helper.Content.Load<Texture2D>("maps/spring_outdoorsTileSheet", ContentSource.GameContent);
        private static Texture2D summer_outdoorsTileSheet = DataLoader.Helper.Content.Load<Texture2D>("maps/summer_outdoorsTileSheet", ContentSource.GameContent);
        private static Texture2D fall_outdoorsTileSheet = DataLoader.Helper.Content.Load<Texture2D>("maps/fall_outdoorsTileSheet", ContentSource.GameContent);
        private static Texture2D winter_outdoorsTileSheet = DataLoader.Helper.Content.Load<Texture2D>("maps/winter_outdoorsTileSheet", ContentSource.GameContent);

        public static void addSpecificTemporarySprite(ref string key, ref GameLocation location)
        {
            if (key == "animalCompetition")
            {
                Texture2D outdoors = null;
                switch (SDate.Now().Season)
                {
                    case "spring":
                        outdoors = spring_outdoorsTileSheet;
                        break;
                    case "summer":
                        outdoors = summer_outdoorsTileSheet;
                        break;
                    case "fall":
                        outdoors = fall_outdoorsTileSheet;
                        break;
                    case "winter":
                        outdoors = winter_outdoorsTileSheet;
                        break;
                }

                location.TemporarySprites.Add(new TemporaryAnimatedSprite(DataLoader.LooseSprites,
                    new Rectangle(84, 0, 98, 79), 9999f, 1, 999,
                    new Vector2(26f, 59f) * (float) Game1.tileSize, false, false,
                    (float)(59 * Game1.tileSize) / 10000f, 0.0f, Color.White,
                    (float) Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false));

                //Outdoors
                Rectangle singleFeed = new Rectangle(304, 144, 16, 32);
                Rectangle doubleFeed = new Rectangle(320,128, 32, 32);
                Rectangle water = new Rectangle(288, 112, 32, 32);
                Rectangle create = new Rectangle(288, 144, 16, 32);
                //LooseSprites
                Rectangle TopLeft = new Rectangle(0, 44, 16, 16);
                Rectangle TopCenter = new Rectangle(16, 44, 16, 16);
                Rectangle TopRight = new Rectangle(32, 44, 16, 16);
                Rectangle CenterLeft = new Rectangle(0, 60, 16, 16);
                Rectangle CenterCenter = new Rectangle(16, 60, 16, 16);
                Rectangle CenterRight = new Rectangle(32, 60, 16, 16);
                Rectangle BottonLeft = new Rectangle(0, 76, 16, 16);
                Rectangle BottonCenter = new Rectangle(16, 76, 16, 16);
                Rectangle BottonRight = new Rectangle(32, 76, 16, 16);

                Rectangle LeftUp = new Rectangle(48, 44, 16, 16);
                Rectangle RightUp = new Rectangle(64, 44, 16, 16);
                Rectangle LeftDown = new Rectangle(48, 60, 16, 16);
                Rectangle RightDown = new Rectangle(64, 60, 16, 16);

                addTemporarySprite(location, outdoors, doubleFeed, 24, 62);
                addTemporarySprite(location, outdoors, water,32,62);
                addTemporarySprite(location, outdoors, singleFeed, 34, 62);
                addTemporarySprite(location, outdoors, create, 23, 62);

                addTemporarySprite(location, DataLoader.LooseSprites, TopLeft, 26, 64);
                addTemporarySprite(location, DataLoader.LooseSprites, TopCenter, 27, 64);
                addTemporarySprite(location, DataLoader.LooseSprites, TopCenter, 28, 64);
                addTemporarySprite(location, DataLoader.LooseSprites, TopCenter, 29, 64);
                addTemporarySprite(location, DataLoader.LooseSprites, TopCenter, 30, 64);
                addTemporarySprite(location, DataLoader.LooseSprites, TopRight, 31, 64);

                addTemporarySprite(location, DataLoader.LooseSprites, TopLeft, 25, 65);
                addTemporarySprite(location, DataLoader.LooseSprites, LeftUp, 26, 65);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterCenter, 27, 65);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterCenter, 28, 65);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterCenter, 29, 65);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterCenter, 30, 65);
                addTemporarySprite(location, DataLoader.LooseSprites, RightUp, 31, 65);
                addTemporarySprite(location, DataLoader.LooseSprites, TopRight, 32, 65);

                addTemporarySprite(location, DataLoader.LooseSprites, CenterLeft, 25, 66);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterCenter, 26, 66);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterCenter, 27, 66);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterCenter, 28, 66);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterCenter, 29, 66);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterCenter, 30, 66);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterCenter, 31, 66);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterRight, 32, 66);

                addTemporarySprite(location, DataLoader.LooseSprites, BottonLeft, 25, 67);
                addTemporarySprite(location, DataLoader.LooseSprites, LeftDown, 26, 67);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterCenter, 27, 67);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterCenter, 28, 67);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterCenter, 29, 67);
                addTemporarySprite(location, DataLoader.LooseSprites, CenterCenter, 30, 67);
                addTemporarySprite(location, DataLoader.LooseSprites, RightDown, 31, 67);
                addTemporarySprite(location, DataLoader.LooseSprites, BottonRight, 32, 67);

                addTemporarySprite(location, DataLoader.LooseSprites, BottonLeft, 26, 68);
                addTemporarySprite(location, DataLoader.LooseSprites, BottonCenter, 27, 68);
                addTemporarySprite(location, DataLoader.LooseSprites, BottonCenter, 28, 68);
                addTemporarySprite(location, DataLoader.LooseSprites, BottonCenter, 29, 68);
                addTemporarySprite(location, DataLoader.LooseSprites, BottonCenter, 30, 68);
                addTemporarySprite(location, DataLoader.LooseSprites, BottonRight, 31, 68);
            }
        }

        private static void addTemporarySprite(GameLocation location, Texture2D texture, Rectangle sourceRectangle , float x, float y)
        {
            location.TemporarySprites.Add(new TemporaryAnimatedSprite(texture,
                sourceRectangle, 9999f, 1, 999
                , new Vector2(x, y) * (float)Game1.tileSize, false, false,
                (float)(y * Game1.tileSize) / 10000f, 0.0f, Color.White,
                (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, false));
        }
    }
}
