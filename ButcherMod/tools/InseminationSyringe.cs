using ButcherMod.animals;
using CustomElementHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.tools
{
    public class InseminationSyringe : MilkPail, ISaveElement
    {
        private FarmAnimal _animal;

        public static int InitialParentTileIndex = 518;
        public static int IndexOfMenuItemView = 518;
        public static int AttachmentMenuTile = 68;

        public InseminationSyringe() : base()
        {
            this.name = "Insemination Syringe";
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
            return DataLoader.i18n.Get("Tool.InseminationSyringe.Name");
        }

        protected override string loadDescription()
        {
            return DataLoader.i18n.Get("Tool.InseminationSyringe.Description");
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

            if (!DataLoader.ModConfig.DisablePregnancy)
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
            }

            if (this._animal != null)
            {
                string dialogue = "";
                if (this.attachments[0] == null)
                {
                    Game1.showRedMessage(DataLoader.i18n.Get("Tool.InseminationSyringe.Empty"));
                    this._animal = null;
                }
                else if (IsEggAnimal(this._animal))
                {
                    dialogue = DataLoader.i18n.Get("Tool.InseminationSyringe.EggAnimal", new { animalName = this._animal.displayName });
                }
                else if (this._animal.isBaby())
                {
                    dialogue = DataLoader.i18n.Get("Tool.InseminationSyringe.TooYoung", new { animalName = this._animal.displayName });
                }                
                else if (PregnancyController.IsAnimalPregnant(this._animal.myID))
                {
                    int daysUtillBirth = PregnancyController.GetPregnancyItem(this._animal.myID).DaysUntilBirth;
                    if (daysUtillBirth > 1)
                    {
                        dialogue = DataLoader.i18n.Get("Tool.InseminationSyringe.AlreadyPregnant", new { animalName = this._animal.displayName, numberOfDays = daysUtillBirth });
                    } else
                    {
                        dialogue = DataLoader.i18n.Get("Tool.InseminationSyringe.ReadyForBirth", new { animalName = this._animal.displayName});
                    }                    
                }
                else if (!CheckCorrectProduct(this._animal, this.attachments[0]))
                {
                    var data = DataLoader.Helper.Content.Load<Dictionary<int, string>>(@"Data\ObjectInformation.xnb", ContentSource.GameContent);
                    string produceName = data[this._animal.defaultProduceIndex].Split('/')[4];
                    dialogue = DataLoader.i18n.Get("Tool.InseminationSyringe.CorrectItem", new { itemName = produceName });
                }
                else if (PregnancyController.CheckBuildingLimit(this._animal))
                {
                    dialogue = DataLoader.i18n.Get("Tool.InseminationSyringe.BuildingLimit", new { buildingType = this._animal.displayHouse });
                }                
                else
                {
                    this._animal.doEmote(16, true);
                    Cue hurtSound = Game1.soundBank.GetCue(this._animal.sound);  
                    hurtSound.Play();
                    DelayedAction.playSoundAfterDelay("fishingRodBend", 300);
                    DelayedAction.playSoundAfterDelay("fishingRodBend", 1200);
                    this._animal.pauseTimer = 1500;
                }                
                if (dialogue.Length > 0)
                {
                    DelayedAction.showDialogueAfterDelay(dialogue, 150);
                    this._animal = null;
                }
            }

            who.Halt();
            int currentFrame = who.FarmerSprite.currentFrame;
            who.FarmerSprite.animateOnce(287 + who.FacingDirection, 50f, 4);
            who.FarmerSprite.oldFrame = currentFrame;
            who.UsingTool = true;
            who.CanMove = false;            

            return true;
        }

        public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            this.currentParentTileIndex = InitialParentTileIndex;
            this.indexOfMenuItemView = IndexOfMenuItemView;

            if (this._animal != null)
            {
                Animal? foundAnimal = AnimalExtension.GetAnimalFromType(this._animal.type);
                who.Stamina -= ((float) 4f - (float)who.FarmingLevel * 0.2f);
                int daysUtillBirth = (DataLoader.AnimalData.getAnimalItem((Animal)foundAnimal) as ImpregnatableAnimalItem).MinimumDaysUtillBirth;
                PregnancyController.AddPregancy(new PregnancyItem(this._animal.myID, daysUtillBirth, this._animal.allowReproduction));
                this._animal.allowReproduction = false;
                --this.attachments[0].Stack;
                if (this.attachments[0].Stack <= 0)
                {
                    Game1.showGlobalMessage(DataLoader.i18n.Get("Tool.InseminationSyringe.ItemConsumed", new { itemName = this.attachments[0].DisplayName }));
                    this.attachments[0] = (StardewValley.Object)null;
                }
                this._animal = (FarmAnimal)null;
            }
            
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

        public bool CheckCorrectProduct(FarmAnimal animal, StardewValley.Object @object)
        {
            switch (AnimalExtension.GetAnimalFromType(animal.type))
            {
                case Animal.Cow:
                case Animal.Goat:                    
                        return animal.defaultProduceIndex == @object.ParentSheetIndex || animal.deluxeProduceIndex == @object.ParentSheetIndex;                    
                default:                    
                        return animal.defaultProduceIndex == @object.ParentSheetIndex;
            }
        }

        public bool IsEggAnimal(FarmAnimal animal)
        {
            switch (AnimalExtension.GetAnimalFromType(animal.type))
            {
                case Animal.Duck:
                case Animal.Chicken:
                case Animal.Dinosaur:
                    return true;
                default:
                    return false;
            }
        }

        public override bool canThisBeAttached(StardewValley.Object o)
        {
            if (o == null || o.category == - 6 || o.parentSheetIndex == 430 || o.parentSheetIndex == 440)
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
            FishingRod replacement = new FishingRod(1);
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
