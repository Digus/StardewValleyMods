using System;
using System.Collections.Generic;
using CustomElementHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Tools;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;
using ButcherMod.animals;

namespace ButcherMod
{
    public class MeatCleaver : Tool, ISaveElement
    {

        private FarmAnimal _animal;
        private FarmAnimal _tempAnimal;

        public new static int initialParentTileIndex = 504;
        public new static int indexOfMenuItemView = 530;

        private string _sufix = "";

        public MeatCleaver() : base("Meat Cleaver", 0, initialParentTileIndex, indexOfMenuItemView, false, 0)
        {
            if (DataLoader.ModConfig.Softmode)
            {
                _sufix = ".Soft";
            }
        }


        protected override string loadDisplayName()
        {
            return DataLoader.i18n.Get("Tool.MeatCleaver.Name"+ _sufix);
        }

        protected override string loadDescription()
        {
            return DataLoader.i18n.Get("Tool.MeatCleaver.Description"+ _sufix);
        }

        public override bool canBeTrashed()
        {
            return true;
        }

        public override bool beginUsing(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            x = (int) who.GetToolLocation(false).X;
            y = (int) who.GetToolLocation(false).Y;
            Rectangle rectangle = new Rectangle(x - Game1.tileSize / 2, y - Game1.tileSize / 2, Game1.tileSize,
                Game1.tileSize);

            if (!DataLoader.ModConfig.DisableMeat)
            {
                if (location is Farm)
                {
                    foreach (FarmAnimal farmAnimal in (location as Farm).animals.Values)
                    {
                        if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                        {
                            if (farmAnimal == this._tempAnimal)
                            {
                                this._animal = farmAnimal;
                                break;
                            }
                            else
                            {
                                this._tempAnimal = farmAnimal;
                                Microsoft.Xna.Framework.Audio.Cue hurtSound;
                                if (!DataLoader.ModConfig.Softmode)
                                {
                                    hurtSound = Game1.soundBank.GetCue(farmAnimal.sound);
                                    hurtSound.SetVariable("Pitch", 1800);
                                }
                                else
                                {
                                    hurtSound = Game1.soundBank.GetCue("toolCharge");
                                    hurtSound.SetVariable("Pitch", 5000f);
                                }

                                hurtSound.Play();
                                break;
                            }
                        }
                    }
                }
                else if (location is AnimalHouse)
                {
                    foreach (FarmAnimal farmAnimal in (location as AnimalHouse).animals.Values)
                    {
                        if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                        {
                            if (farmAnimal == this._tempAnimal)
                            {
                                this._animal = farmAnimal;
                                break;
                            }
                            else
                            {
                                this._tempAnimal = farmAnimal;
                                Microsoft.Xna.Framework.Audio.Cue hurtSound;
                                if (!DataLoader.ModConfig.Softmode)
                                {
                                    hurtSound = Game1.soundBank.GetCue(farmAnimal.sound);
                                    hurtSound.SetVariable("Pitch", 1800);
                                }
                                else
                                {
                                    hurtSound = Game1.soundBank.GetCue("toolCharge");
                                    hurtSound.SetVariable("Pitch", 5000f);
                                }

                                hurtSound.Play();
                                break;
                            }
                        }
                    }
                }                
            }

            this.Update(who.facingDirection, 0, who);
            if (this._tempAnimal != null && this._tempAnimal.age < (int)this._tempAnimal.ageWhenMature)
            {
                string dialogue = DataLoader.i18n.Get("Tool.MeatCleaver.TooYoung"+_sufix, new { animalName = this._tempAnimal.displayName });
                DelayedAction.showDialogueAfterDelay(dialogue, 150);
                this._tempAnimal = null;
            }
            if (who.IsMainPlayer)
            {
                Game1.releaseUseToolButton();
                return true;
            }
            return true;
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            base.DoFunction(location, x, y, power, who);
            who.Stamina -= ((float)4f - (float)who.FarmingLevel * 0.2f);
            if (this._animal != null && this._animal.type == "Dinosaur")
            {
                return;
            }

            if (this._animal != null && this._animal.age >= (int) this._animal.ageWhenMature)
            {
                (this._animal.home.indoors as AnimalHouse).animalsThatLiveHere.Remove(this._animal.myID);
                this._animal.health = -1;
                int numClouds = this._animal.frontBackSourceRect.Width / 2;
                int cloudSprite = !DataLoader.ModConfig.Softmode ? 5 : 10;
                for (int i = 0; i < numClouds; i++)
                {
                    int nonRedness = Game1.random.Next(0, 80);
                    Color cloudColor = new Color(255, 255 - nonRedness, 255 - nonRedness); ;
                    
                    Game1.currentLocation.temporarySprites.Add
                    (
                        new TemporaryAnimatedSprite
                        (
                            cloudSprite
                            ,this._animal.position +new Vector2(Game1.random.Next(-Game1.tileSize / 2, this._animal.frontBackSourceRect.Width * 3)
                            ,Game1.random.Next(-Game1.tileSize / 2, this._animal.frontBackSourceRect.Height * 3))
                            ,cloudColor
                            , 8
                            , false,
                            Game1.random.NextDouble() < .5 ? 50 : Game1.random.Next(30, 200), 0, Game1.tileSize
                            , -1
                            ,Game1.tileSize, Game1.random.NextDouble() < .5 ? 0 : Game1.random.Next(0, 600)
                        )
                        {
                            scale = Game1.random.Next(2, 5) * .25f,
                            alpha = Game1.random.Next(2, 5) * .25f,
                            motion = new Vector2(0, (float) -Game1.random.NextDouble())
                        }
                    );
                }
                Color animalColor;
                float alfaFade;
                if (!DataLoader.ModConfig.Softmode)
                {
                    animalColor = Color.LightPink;
                    alfaFade = .025f;
                }
                else
                {
                    animalColor = Color.White;
                    alfaFade = .050f;
                }
                Game1.currentLocation.temporarySprites.Add
                (
                    new TemporaryAnimatedSprite
                    (
                        this._animal.sprite.Texture
                        ,this._animal.sprite.SourceRect
                        , this._animal.position
                        , this._animal.FacingDirection == Game1.left
                        , alfaFade
                        , animalColor
                    )
                    {
                        scale = 4f
                    }
                );
                if (!DataLoader.ModConfig.Softmode)
                {
                    Game1.playSound("killAnimal");
                } else
                {
                    Microsoft.Xna.Framework.Audio.Cue warptSound = Game1.soundBank.GetCue("wand");
                    warptSound.SetVariable("Pitch", 1800);
                    warptSound.Play();
                }
                

                this.CreateMeat();
                who.gainExperience(0, 5);
                this._animal = (FarmAnimal)null;
                this._tempAnimal = (FarmAnimal)null;
            }
        }

        private void CreateMeat()
        {                        
            int debrisType = this._animal.meatIndex;

            Animal animal;
            Animal? foundAnimal = AnimalExtension.GetAnimalFromType(this._animal.type);
            if (foundAnimal == null || foundAnimal == Animal.Dinosaur)
            {
                return;
            }
            else
            {
                animal = (Animal)foundAnimal;
            }

            AnimalItem animalItem = DataLoader.AnimalData.getAnimalItem(animal);
            int meatPrice =  DataLoader.MeatData.getMeatItem(animal.GetMeat()).Price;
            int minimumNumberOfMeat = animalItem.MinimalNumberOfMeat;
            int maxNumberOfMeat = animalItem.MaximumNumberOfMeat;
            int numberOfMeat = minimumNumberOfMeat;

            numberOfMeat += (int) ((_animal.getSellPrice() / ((double)_animal.price) - 0.3) * (maxNumberOfMeat - minimumNumberOfMeat));

            Random random = new Random((int)_animal.myID * 10000 + (int)Game1.stats.DaysPlayed);
            int[] quality = {0,0,0,0,0}; 
            for (int i = 0; i < numberOfMeat; i++)
            {
                var produceQuality = ProduceQuality(random);
                quality[produceQuality]++;
            }

            var tempTotal = meatPrice * quality[0] + meatPrice * quality[1] * 1.25 + meatPrice * quality[2] * 1.5 + meatPrice * quality[4] * 2;
            while (tempTotal < _animal.getSellPrice() && quality[4] != numberOfMeat)
            {
                if (numberOfMeat < maxNumberOfMeat)
                {
                    numberOfMeat++;
                    quality[0]++;
                    tempTotal += meatPrice;
                }
                else if (quality[0] > 0)
                {
                    quality[0]--;
                    quality[1]++;
                    tempTotal += meatPrice * 0.25;
                }
                else if ((quality[1] > 0))
                {
                    quality[1]--;
                    quality[2]++;
                    tempTotal += meatPrice * 0.25;
                }
                else if ((quality[2] > 0))
                {
                    quality[2]--;
                    quality[4]++;
                    tempTotal += meatPrice * 0.50;
                }
            } 

            for (; numberOfMeat > 0; --numberOfMeat)
            {
                Object newItem = new Object(Vector2.Zero, debrisType, 1);
                newItem.quality = quality[4] > 0 ? 4 : quality[2] > 0 ? 2 : quality[1] > 0 ? 1 :  0;
                quality[newItem.quality]--;

                this.ThrowItem(newItem);
            }

            if ((animal == Animal.Sheep || animal == Animal.Rabbit))
            {
                WoolAnimalItem woolAnimalItem = (WoolAnimalItem)animalItem;
                int numberOfWools = this._animal.currentProduce > 0 ? 1 : 0;
                numberOfWools += (int)(woolAnimalItem.MinimumNumberOfExtraWool + (_animal.getSellPrice() / ((double)_animal.price) - 0.3) * (woolAnimalItem.MaximumNumberOfExtraWool - woolAnimalItem.MinimumNumberOfExtraWool));
                
                for (; numberOfWools > 0; --numberOfWools)
                {
                    Object newItem = new Object(Vector2.Zero, this._animal.defaultProduceIndex, 1);
                    newItem.quality = this.ProduceQuality(random);
                    this.ThrowItem(newItem);
                }
            }

            if (animal == Animal.Duck)
            {
                FeatherAnimalItem featherAnimalItem = (FeatherAnimalItem)animalItem;
                int numberOfFeather = (int)(featherAnimalItem.MinimumNumberOfFeatherChances + (_animal.getSellPrice() / ((double)_animal.price) - 0.3) * (featherAnimalItem.MaximumNumberOfFeatherChances - featherAnimalItem.MinimumNumberOfFeatherChances));
                float num1 = (int)this._animal.happiness > 200 ? (float)this._animal.happiness * 1.5f : ((int)this._animal.happiness <= 100 ? (float)((int)this._animal.happiness - 100) : 0.0f);
                for (; numberOfFeather > 0; --numberOfFeather)
                {
                    if (random.NextDouble() < (double) this._animal.happiness / 150.0)
                    {
                        if (random.NextDouble() < ((double) this._animal.friendshipTowardFarmer + (double) num1) / 5000.0 + Game1.dailyLuck + (double) Game1.player.LuckLevel * 0.01)
                        {
                            Object newItem = new Object(Vector2.Zero, this._animal.deluxeProduceIndex, 1);
                            newItem.quality = this.ProduceQuality(random);
                            this.ThrowItem(newItem);
                        }
                    }
                }
            } 

            if (animal == Animal.Rabbit)
            {
                FeetAnimalItem feetAnimalItem = (FeetAnimalItem)animalItem;
                int numberOfFeet = (int)(feetAnimalItem.MinimumNumberOfFeetChances + (_animal.getSellPrice() / ((double)_animal.price) - 0.3) * (feetAnimalItem.MaximumNumberOfFeetChances - feetAnimalItem.MinimumNumberOfFeetChances));
                float num1 = (int)this._animal.happiness > 200 ? (float)this._animal.happiness * 1.5f : ((int)this._animal.happiness <= 100 ? (float)((int)this._animal.happiness - 100) : 0.0f);
                for (; numberOfFeet > 0; --numberOfFeet)
                {
                    if (random.NextDouble() < (double) this._animal.happiness / 150.0)
                    {
                        if (random.NextDouble() < ((double) this._animal.friendshipTowardFarmer + (double) num1) / 5000.0 + Game1.dailyLuck + (double) Game1.player.LuckLevel * 0.01)
                        {
                            Object newItem = new Object(Vector2.Zero, this._animal.deluxeProduceIndex, 1);
                            newItem.quality = this.ProduceQuality(random);
                            this.ThrowItem(newItem);
                        }
                    }
                }
            }
        }

        private void ThrowItem(Object newItem)
        {
            GameLocation location = Game1.currentLocation;
            int xTile = this._animal.getTileX() - 1;
            int yTile = this._animal.getTileY() - 1;
            Vector2 debrisOrigin = new Vector2((float)(xTile * Game1.tileSize + Game1.tileSize),
                (float)(yTile * Game1.tileSize + Game1.tileSize));
            switch (Game1.random.Next(4))
            {
                case 0:
                    location.debris.Add(new Debris((Item) newItem, debrisOrigin,
                        debrisOrigin + new Vector2((float) -Game1.tileSize, 0.0f)));
                    break;
                case 1:
                    location.debris.Add(new Debris((Item) newItem, debrisOrigin,
                        debrisOrigin + new Vector2((float) Game1.tileSize, 0.0f)));
                    break;
                case 2:
                    location.debris.Add(new Debris((Item) newItem, debrisOrigin,
                        debrisOrigin + new Vector2(0.0f, (float) Game1.tileSize)));
                    break;
                case 3:
                    location.debris.Add(new Debris((Item) newItem, debrisOrigin,
                        debrisOrigin + new Vector2(0.0f, (float) -Game1.tileSize)));
                    break;
            }
        }

        private int ProduceQuality(Random random)
        {
            double chance = (double) _animal.friendshipTowardFarmer / 1000.0 - (1.0 - (double) _animal.happiness / 225.0);
            if (!_animal.isCoopDweller() && Game1.getFarmer(_animal.ownerID).professions.Contains(3) ||
                _animal.isCoopDweller() && Game1.getFarmer(_animal.ownerID).professions.Contains(2))
                chance += 0.33;
            var produceQuality = chance < 0.95 || random.NextDouble() >= chance / 2.0
                ? (random.NextDouble() >= chance / 2.0 ? (random.NextDouble() >= chance ? 0 : 1) : 2)
                : 4;
            return produceQuality;
        }

        

        public object getReplacement()
        {
            Object replacement = new Object(169,1);
            return replacement;
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> savedata = new Dictionary<string, string>();
            savedata.Add("name", name);
            return savedata;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            this.name = additionalSaveData["name"];
        }
    }
}
