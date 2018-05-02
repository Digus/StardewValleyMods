using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;
using PyTK.CustomElementHandler;
using StardewValley.Characters;
using StardewValley.Locations;

namespace AnimalHusbandryMod.tools
{
    public class FeedingBasket : MilkPail, ISaveElement
    {
        private FarmAnimal _animal;
        private Pet _pet;

        public static int InitialParentTileIndex = 519;
        public static int IndexOfMenuItemView = 519;
        public static int AttachmentMenuTile = 69;

        public FeedingBasket() : base()
        {
            this.name = "Feeding Basket";
            this.initialParentTileIndex = InitialParentTileIndex;
            this.indexOfMenuItemView = IndexOfMenuItemView;
            this.stackable = false;
            this.currentParentTileIndex = initialParentTileIndex;
            this.numAttachmentSlots = 1;
            this.attachments = new StardewValley.Object[numAttachmentSlots];
            this.category = -99;
        }

        protected override string loadDisplayName()
        {
            return DataLoader.i18n.Get("Tool.FeedingBasket.Name");
        }

        protected override string loadDescription()
        {
            return DataLoader.i18n.Get("Tool.FeedingBasket.Description");
        }

        public override bool canBeTrashed()
        {
            return true;
        }

        public override bool beginUsing(GameLocation location, int x, int y, StardewValley.Farmer who)
        {
            x = (int)who.GetToolLocation(false).X;
            y = (int)who.GetToolLocation(false).Y;
            Rectangle rectangle = new Rectangle(x - Game1.tileSize / 2, y - Game1.tileSize / 2, Game1.tileSize, Game1.tileSize);

            if (!DataLoader.ModConfig.DisableTreats)
            {
                if (location is Farm)
                {
                    foreach (FarmAnimal farmAnimal in (location as Farm).animals.Values)
                    {
                        if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                        {
                            this._animal = farmAnimal;
                            break;
                        }
                    }
                    if (this._animal == null)
                    {
                        foreach (Pet pet in location.characters.FindAll(i => i is Pet))
                        {
                            if (pet.GetBoundingBox().Intersects(rectangle))
                            {
                                this._pet = pet;
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
                            this._animal = farmAnimal;
                            break;
                        }
                    }
                }
                else if (location is FarmHouse)
                {
                    foreach (Pet pet in location.characters.FindAll(i => i is Pet))
                    {
                        if (pet.GetBoundingBox().Intersects(rectangle))
                        {
                            this._pet = pet;
                            break;
                        }
                    }
                }
            }

            if (this._animal != null)
            {
                string dialogue = "";
                if (this.attachments[0] == null)
                {
                    Game1.showRedMessage(DataLoader.i18n.Get("Tool.FeedingBasket.Empty"));
                    this._animal = null;
                }
                else if (!TreatsController.CanReceiveTreat(this._animal))
                {
                    dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.NotLikeTreat", new { itemName = this.attachments[0].DisplayName });
                }
                else if (!TreatsController.IsLikedTreat(this._animal, this.attachments[0].parentSheetIndex) && !TreatsController.IsLikedTreat(this._animal, this.attachments[0].Category))
                {
                    dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.NotLikeTreat", new  { itemName = this.attachments[0].DisplayName});
                }
                else if (this.attachments[0].category == StardewValley.Object.MilkCategory && !this._animal.isBaby())
                {
                    dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.OnlyBabiesCanEatMilk");
                }
                else if (!TreatsController.IsReadyForTreat(this._animal))
                {
                    if (TreatsController.GetTreatItem(this._animal)?.MinimumDaysBetweenTreats == 1)
                    {
                        dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.AlreadyAteTreatToday", new { animalName = this._pet.displayName });
                    }
                    else
                    {
                        int daysUntilNextTreat = TreatsController.DaysUntilNextTreat(this._animal);
                        if (daysUntilNextTreat > 1)
                        {
                            dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.WantsTreatInDays", new { animalName = this._animal.displayName ,  numberOfDays = daysUntilNextTreat });
                        }
                        else if (daysUntilNextTreat == 1)
                        {
                            dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.WantsTreatTomorrow", new { animalName = this._animal.displayName });
                        }
                    }
                }
                else
                {
                    this._animal.pauseTimer = 1000;               
                }


                if (dialogue.Length > 0)
                {
                    DelayedAction.showDialogueAfterDelay(dialogue, 150);
                    this._animal = null;
                }
            }
            if (this._pet != null)
            {
                string dialogue = "";
                if (this.attachments[0] == null)
                {
                    Game1.showRedMessage(DataLoader.i18n.Get("Tool.FeedingBasket.Empty"));
                    this._pet = null;
                }
                else if (!TreatsController.IsLikedTreatPet(this.attachments[0].parentSheetIndex) && !TreatsController.IsLikedTreatPet(this.attachments[0].Category))
                {
                    dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.NotLikeTreat", new { itemName = this.attachments[0].DisplayName });
                }
                else if (!TreatsController.IsReadyForTreatPet())
                {
                    int daysUntilNextTreat = TreatsController.DaysUntilNextTreatPet();

                    if (DataLoader.AnimalData.Pet.MinimumDaysBetweenTreats == 1)
                    {
                        dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.AlreadyAteTreatToday", new { animalName = this._pet.displayName });
                    }
                    else if (daysUntilNextTreat > 1)
                    {
                        dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.WantsTreatInDays", new { animalName = this._pet.displayName, numberOfDays = daysUntilNextTreat });
                    }
                    else if (daysUntilNextTreat == 1)
                    {
                        dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.WantsTreatTomorrow", new { animalName = this._pet.displayName });
                    }
                }
                else
                {
                    _pet.Halt();
                    _pet.CurrentBehavior = 0;
                    _pet.Halt();
                    _pet.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>(){ new FarmerSprite.AnimationFrame(18, 200) });

                }


                if (dialogue.Length > 0)
                {
                    DelayedAction.showDialogueAfterDelay(dialogue, 150);
                    this._pet = null;
                }
            }

            
            who.Halt();
            int currentFrame = who.FarmerSprite.currentFrame;
            if (this._animal != null || this._pet != null)
            {
                switch (who.FacingDirection)
                {
                    case 0:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(62, 900, false, false, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                    case 1:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(58, 900, false, false, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                    case 2:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(54, 900, false, false, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                    case 3:
                        who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(58, 900, false, true, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
                        break;
                }
            } else
            {                
                who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(currentFrame, 0, false, who.FacingDirection == 3, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
            }
            who.FarmerSprite.oldFrame = currentFrame;
            who.UsingTool = true;
            who.CanMove = false;

            if (this._animal != null || this._pet != null)
            {
                Rectangle boundingBox;
                if (this._animal != null)
                {
                    boundingBox = this._animal.GetBoundingBox();
                }
                else
                {
                    boundingBox = this._pet.GetBoundingBox();
                }

                double numX = boundingBox.Center.X;
                double numY = boundingBox.Center.Y;

                Vector2 vectorBasket = new Vector2((float) numX - 32, (float) numY);
                Vector2 vectorFood = new Vector2((float) numX - 24, (float) numY - 10);
                var foodScale = Game1.pixelZoom * 0.75f;

                TemporaryAnimatedSprite basketSprite = new TemporaryAnimatedSprite(Game1.toolSpriteSheet,
                    Game1.getSourceRectForStandardTileSheet(Game1.toolSpriteSheet, this.currentParentTileIndex, 16, 16),
                    750.0f, 1, 1, vectorBasket, false, false, ((float) boundingBox.Bottom + 0.1f) / 10000f, 0.0f,
                    Color.White, Game1.pixelZoom, 0.0f, 0.0f, 0.0f) {delayBeforeAnimationStart = 100};
                who.currentLocation.temporarySprites.Add(basketSprite);
                TemporaryAnimatedSprite foodSprite = new TemporaryAnimatedSprite(Game1.objectSpriteSheet,
                    Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,this.attachments[0].parentSheetIndex,
                    16, 16), 500.0f, 1, 1, vectorFood, false, false,((float) boundingBox.Bottom + 0.2f) / 10000f, 0.0f,
                    Color.White, foodScale, 0.0f, 0.0f, 0.0f) {delayBeforeAnimationStart = 100};
                who.currentLocation.temporarySprites.Add(foodSprite);

                for (int index = 0; index < 8; ++index)
                {
                    Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                        this.attachments[0].parentSheetIndex, 16, 16);
                    standardTileSheet.X += 8;
                    standardTileSheet.Y += 8;

                    standardTileSheet.Width = Game1.pixelZoom;
                    standardTileSheet.Height = Game1.pixelZoom;
                    TemporaryAnimatedSprite temporaryAnimatedSprite2 =
                        new TemporaryAnimatedSprite(Game1.objectSpriteSheet, standardTileSheet, 400f, 1, 0,
                            vectorFood + new Vector2(12, 12), false, false,
                            ((float) boundingBox.Bottom + 0.2f) / 10000f, 0.0f, Color.White, (float) foodScale, 0.0f,
                            0.0f, 0.0f, false)
                        {
                            motion = new Vector2((float) Game1.random.Next(-30, 31) / 10f,(float) Game1.random.Next(-6, -3)),
                            acceleration = new Vector2(0.0f, 0.5f),
                            delayBeforeAnimationStart = 600
                        };
                    who.currentLocation.temporarySprites.Add(temporaryAnimatedSprite2);
                }

                if (this._animal != null)
                {
                    Game1.delayedActions.Add(new DelayedAction(300, new DelayedAction.delayedBehavior(() => {
                        AnimalHusbandryModEntery.ModHelper.Reflection.GetField<bool>(this._animal, "isEating").SetValue(true);
                        this._animal.sprite.loop = false;
                    })));
                }
                else if (this._pet != null)
                {
                    
                    _pet.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(18, 300),
                        new FarmerSprite.AnimationFrame(17, 100),
                        new FarmerSprite.AnimationFrame(16, 100),
                        new FarmerSprite.AnimationFrame(0, 100),
                        new FarmerSprite.AnimationFrame(16, 100),
                        new FarmerSprite.AnimationFrame(17, 100),
                        new FarmerSprite.AnimationFrame(18, 300)
                    });
                    _pet.sprite.loop = false;
                }
                DelayedAction.playSoundAfterDelay("eat", 600);
            }

            return true;
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            this.currentParentTileIndex = InitialParentTileIndex;
            this.indexOfMenuItemView = IndexOfMenuItemView;

            if (this._animal != null)
            {
                this._animal.doEmote(20, true);

                if (!DataLoader.ModConfig.DisableFriendshipInscreseWithTreats)
                {
                    double professionAjust = 1.0;
                    if (!_animal.isCoopDweller() && who.professions.Contains(3) || _animal.isCoopDweller() && who.professions.Contains(2))
                    {
                        professionAjust += DataLoader.ModConfig.PercentualAjustOnFriendshipInscreaseFromProfessions;
                    }
                    this._animal.friendshipTowardFarmer = Math.Min(1000, this._animal.friendshipTowardFarmer + (int)Math.Ceiling(professionAjust * this.attachments[0].price * (1.0 + this.attachments[0].quality * 0.25) / (this._animal.price / 1000.0)));
                }
                if (!DataLoader.ModConfig.DisableMoodInscreseWithTreats)
                {
                    this._animal.happiness = byte.MaxValue;
                }

                if (DataLoader.ModConfig.EnableTreatsCountAsAnimalFeed)
                {
                    this._animal.fullness = byte.MaxValue;
                }

                TreatsController.FeedAnimalTreat(this._animal, this.attachments[0]);

                if (this.attachments[0].Category == StardewValley.Object.MilkCategory)
                {
                    this._animal.age = Math.Min(this._animal.ageWhenMature - 1, this._animal.age + 1);
                }

                --this.attachments[0].Stack;
                if (this.attachments[0].Stack <= 0)
                {
                    Game1.showGlobalMessage(DataLoader.i18n.Get("Tool.FeedingBasket.ItemConsumed", new { itemName = this.attachments[0].DisplayName }));
                    this.attachments[0] = (StardewValley.Object)null;
                }
            }
            else if (this._pet != null)
            {
                this._pet.doEmote(20, true);

                if (!DataLoader.ModConfig.DisableFriendshipInscreseWithTreats)
                {
                    this._pet.friendshipTowardFarmer = Math.Min(Pet.maxFriendship, this._pet.friendshipTowardFarmer + 6);
                }
                TreatsController.FeedPetTreat(this.attachments[0]);

                --this.attachments[0].Stack;
                if (this.attachments[0].Stack <= 0)
                {
                    Game1.showGlobalMessage(DataLoader.i18n.Get("Tool.FeedingBasket.ItemConsumed", new { itemName = this.attachments[0].DisplayName }));
                    this.attachments[0] = (StardewValley.Object)null;
                }
            }
            this._animal = null;
            this._pet = null;

            if (Game1.activeClickableMenu == null)
            {
                who.CanMove = true;
            }
            else
            {
                who.Halt();
            }
            who.usingTool = false;
            who.canReleaseTool = true;
        }

        public override bool canThisBeAttached(StardewValley.Object o)
        {
            if (o == null 
                || o.category == StardewValley.Object.VegetableCategory 
                || o.category == StardewValley.Object.FruitsCategory 
                || TreatsController.IsLikedTreat(o.parentSheetIndex)
                || TreatsController.IsLikedTreat(o.Category)
                )
            {
                return true;
            }
            return false;
        }

        public override StardewValley.Object attach(StardewValley.Object o)
        {
            if (o != null)
            {
                StardewValley.Object @object = this.attachments[0];
                if (@object != null && @object.canStackWith((Item)o))
                {
                    @object.Stack = o.addToStack(@object.Stack);
                    if (@object.Stack <= 0)
                        @object = (StardewValley.Object)null;
                }
                this.attachments[0] = o;
                Game1.playSound("button1");
                return @object;
            }
            else
            {
                if (this.attachments[0] != null)
                {
                    StardewValley.Object attachment = this.attachments[0];
                    this.attachments[0] = (StardewValley.Object)null;
                    Game1.playSound("dwop");
                    return attachment;
                }
            }
            return (StardewValley.Object)null;
        }

        public override void drawAttachments(SpriteBatch b, int x, int y)
        {

            if (attachments[0] != null)
            {
                b.Draw(Game1.menuTexture, new Vector2((float)x, (float)y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
                attachments[0].drawInMenu(b, new Vector2((float)x, (float)y), 1f);
            }
            else
            {
                b.Draw(Game1.menuTexture, new Vector2((float)x, (float)y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, AttachmentMenuTile, -1, -1)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
            }
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> savedata = new Dictionary<string, string>();
            savedata.Add("name", name);
            return savedata;
        }

        public dynamic getReplacement()
        {
            FishingRod replacement = new FishingRod(2);
            replacement.upgradeLevel = -1;
            replacement.attachments = this.attachments;
            return replacement;
        }


        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            this.attachments = (replacement as Tool).attachments;

            this.name = additionalSaveData["name"];
        }
    }
}
