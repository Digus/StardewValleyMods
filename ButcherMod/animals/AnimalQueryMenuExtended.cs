﻿using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.tools;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace AnimalHusbandryMod.animals
{
    public class AnimalQueryMenuExtended : AnimalQueryMenu
    {
        private FarmAnimal _farmAnimal;
        private String _parentName;
        private TextBox _textBox;

        private bool confirmingMeat = false;

        private ClickableTextureComponent pregnantStatus = null;
        private ClickableTextureComponent treatStatus = null;
        private ClickableTextureComponent meatButton = null;

        private IReflectedField<bool> _movingAnimal;
        private IReflectedField<bool> _confirmingSell;
        private IReflectedField<double> _lovelLevel;
        private IReflectedField<string> _hoverText;

        public AnimalQueryMenuExtended(FarmAnimal farmAnimal) : base(farmAnimal)
        {
            _farmAnimal = farmAnimal;
            if (!DataLoader.ModConfig.DisablePregnancy)
            {
                if (PregnancyController.IsAnimalPregnant(this._farmAnimal.myID))
                {
                    this.allowReproductionButton = null;
                    pregnantStatus = new ClickableTextureComponent(
                        new Microsoft.Xna.Framework.Rectangle(
                            this.xPositionOnScreen + AnimalQueryMenu.width + Game1.pixelZoom * 3,
                            this.yPositionOnScreen + AnimalQueryMenu.height - Game1.tileSize * 2 -
                            IClickableMenu.borderWidth + Game1.pixelZoom, Game1.pixelZoom * 11, Game1.pixelZoom * 11),
                        DataLoader.LooseSprites, new Microsoft.Xna.Framework.Rectangle(34, 29, 11, 11), 4f, false);
                }
            }

            if (TreatsController.IsReadyForTreat(farmAnimal))
            {
                treatStatus = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + AnimalQueryMenu.width + Game1.tileSize + 4, this.yPositionOnScreen + AnimalQueryMenu.height - Game1.tileSize * 2 - IClickableMenu.borderWidth, Game1.tileSize, Game1.tileSize), DataLoader.ToolsSprites, new Microsoft.Xna.Framework.Rectangle(240, 0, 16, 16), 4f, false);
            }
            else
            {
                treatStatus = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + AnimalQueryMenu.width + Game1.tileSize + 4, this.yPositionOnScreen + AnimalQueryMenu.height - Game1.tileSize * 2 - IClickableMenu.borderWidth, Game1.tileSize, Game1.tileSize), DataLoader.LooseSprites, new Microsoft.Xna.Framework.Rectangle(16, 28, 16, 16), 4f, false);
            }

            if (!DataLoader.ModConfig.DisableMeat)
            {
                if (!this._farmAnimal.isBaby())
                {
                    meatButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + AnimalQueryMenu.width + Game1.tileSize + 4, this.yPositionOnScreen + AnimalQueryMenu.height - Game1.tileSize * 3 - IClickableMenu.borderWidth, Game1.tileSize, Game1.tileSize), DataLoader.LooseSprites, new Microsoft.Xna.Framework.Rectangle(0, 28, 16, 16), 4f, false);
                }
            }

            _parentName = DataLoader.Helper.Reflection.GetField<String>(this, "parentName").GetValue();
            _textBox = DataLoader.Helper.Reflection.GetField<TextBox>(this, "textBox").GetValue();
            _movingAnimal = DataLoader.Helper.Reflection.GetField<bool>(this, "movingAnimal");
            _confirmingSell = DataLoader.Helper.Reflection.GetField<bool>(this, "confirmingSell");
            _lovelLevel = DataLoader.Helper.Reflection.GetField<double>(this, "loveLevel");
            _hoverText = DataLoader.Helper.Reflection.GetField<string>(this, "hoverText");
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (Game1.globalFade)
                return;
            if (_movingAnimal.GetValue())
            {
                Building buildingAt = (Game1.getLocationFromName("Farm") as Farm).getBuildingAt(new Vector2((float)((x + Game1.viewport.X) / Game1.tileSize), (float)((y + Game1.viewport.Y) / Game1.tileSize)));
                if 
                (
                    buildingAt != null                     
                    && buildingAt.buildingType.Contains(this._farmAnimal.buildingTypeILiveIn)
                    && ! (buildingAt.indoors as AnimalHouse).isFull()
                    && ! buildingAt.Equals((object)this._farmAnimal.home)
                    && PregnancyController.IsAnimalPregnant(this._farmAnimal.myID)
                    && PregnancyController.CheckBuildingLimit((buildingAt.indoors as AnimalHouse))
                )
                {
                    if (this.okButton != null && this.okButton.containsPoint(x, y))
                    {
                        Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.prepareForReturnFromPlacement), 0.02f);
                        Game1.playSound("smallSelect");
                    }
                    Game1.showRedMessage(DataLoader.i18n.Get("Menu.AnimalQueryMenu.PregnancyBuildingLimit", new { buildingType = this._farmAnimal.displayHouse }));
                    return;
                }
            }
            else if (this.confirmingMeat)
            {
                if (this.yesButton.containsPoint(x, y))
                {
                    (this._farmAnimal.home.indoors as AnimalHouse).animalsThatLiveHere.Remove(this._farmAnimal.myID);
                    this._farmAnimal.health = -1;
                    int num1 = this._farmAnimal.frontBackSourceRect.Width / 2;
                    for (int index = 0; index < num1; ++index)
                    {
                        int num2 = Game1.random.Next(25, 200);
                        Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(5, this._farmAnimal.position + new Vector2((float)Game1.random.Next(-Game1.tileSize / 2, this._farmAnimal.frontBackSourceRect.Width * 3), (float)Game1.random.Next(-Game1.tileSize / 2, this._farmAnimal.frontBackSourceRect.Height * 3)), new Color((int)byte.MaxValue - num2, (int)byte.MaxValue, (int)byte.MaxValue - num2), 8, false, Game1.random.NextDouble() < 0.5 ? 50f : (float)Game1.random.Next(30, 200), 0, Game1.tileSize, -1f, Game1.tileSize, Game1.random.NextDouble() < 0.5 ? 0 : Game1.random.Next(0, 600))
                        {
                            scale = (float)Game1.random.Next(2, 5) * 0.25f,
                            alpha = (float)Game1.random.Next(2, 5) * 0.25f,
                            motion = new Vector2(0.0f, (float)-Game1.random.NextDouble())
                        });
                    }
                    Game1.playSound("newRecipe");
                    Game1.playSound("money");
                    Game1.exitActiveMenu();
                    Game1.player.Stamina -= ((float)4f - (float)Game1.player.FarmingLevel * 0.2f);
                    Game1.player.Stamina -= ((float)4f - (float)Game1.player.FarmingLevel * 0.2f);
                    Game1.player.gainExperience(0, 5);
                    MeatController.AddItemsToInventoryByMenuIfNecessary(MeatController.CreateMeat(this._farmAnimal));
                }
                else
                {
                    if (!this.noButton.containsPoint(x, y))
                        return;
                    this.confirmingMeat = false;
                    Game1.playSound("smallSelect");
                    if (!Game1.options.SnappyMenus)
                        return;
                    this.currentlySnappedComponent = this.getComponentWithID(103);
                    this.snapCursorToCurrentSnappedComponent();
                }
                return;
            }
            else if (!_confirmingSell.GetValue())
            {
                if (this.meatButton?.containsPoint(x, y)??false)
                {
                    this.confirmingMeat = true;
                    ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(Game1.viewport.Width / 2 - Game1.tileSize - 4, Game1.viewport.Height / 2 - Game1.tileSize / 2, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
                    textureComponent1.myID = 111;
                    textureComponent1.rightNeighborID = 105;
                    this.yesButton = textureComponent1;
                    ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(Game1.viewport.Width / 2 + 4, Game1.viewport.Height / 2 - Game1.tileSize / 2, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47, -1, -1), 1f, false);
                    textureComponent2.myID = 105;
                    textureComponent2.leftNeighborID = 111;
                    this.noButton = textureComponent2;
                    Game1.playSound("smallSelect");
                    if (!Game1.options.SnappyMenus)
                        return;
                    this.populateClickableComponentList();
                    this.currentlySnappedComponent = (ClickableComponent)this.noButton;
                    this.snapCursorToCurrentSnappedComponent();
                }
            }

            base.receiveLeftClick(x, y, playSound);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (_movingAnimal.GetValue())
            {
                Vector2 tile = new Vector2((float)((x + Game1.viewport.X) / Game1.tileSize), (float)((y + Game1.viewport.Y) / Game1.tileSize));
                Farm locationFromName = Game1.getLocationFromName("Farm") as Farm;
                Building buildingAt = locationFromName.getBuildingAt(tile);
                if (buildingAt != null)
                    if( buildingAt.color.Equals(Color.LightGreen * 0.8f)
                    && PregnancyController.IsAnimalPregnant(this._farmAnimal.myID)
                    && PregnancyController.CheckBuildingLimit((buildingAt.indoors as AnimalHouse)))
                {
                    buildingAt.color = Color.Red * 0.8f;
                }
            }
            else
            {
                if (this.meatButton != null)
                {
                    if (this.meatButton.containsPoint(x, y))
                    {
                        this.meatButton.scale = Math.Min(4.1f, this.meatButton.scale + 0.05f);
                        _hoverText.SetValue(DataLoader.i18n.Get("Menu.AnimalQueryMenu.ExchangeAnimalForMeat"));
                    }
                    else
                        this.meatButton.scale = Math.Max(4f, this.sellButton.scale - 0.05f);
                }
                if (this.pregnantStatus != null)
                {
                    if (this.pregnantStatus.containsPoint(x, y))
                    {
                        int daysUtilBirth = PregnancyController.GetPregnancyItem(this._farmAnimal.myID).DaysUntilBirth;
                        if (daysUtilBirth > 1)
                        {
                            _hoverText.SetValue(DataLoader.i18n.Get("Menu.AnimalQueryMenu.DaysUntilBirth", new { numberOfDays = daysUtilBirth }));
                        }
                        else
                        {
                            _hoverText.SetValue(DataLoader.i18n.Get("Menu.AnimalQueryMenu.ReadyForBirth"));
                        }
                        
                    }
                }
                if (this.treatStatus != null)
                {
                    if (this.treatStatus.containsPoint(x, y))
                    {
                        int daysUntilNextTreat = TreatsController.DaysUntilNextTreat(this._farmAnimal);
                        if (daysUntilNextTreat > 1)
                        {
                            _hoverText.SetValue(DataLoader.i18n.Get("Menu.AnimalQueryMenu.WantsTreatInDays", new { numberOfDays = daysUntilNextTreat }));
                        }
                        else if (daysUntilNextTreat == 1)
                        {
                            _hoverText.SetValue(DataLoader.i18n.Get("Menu.AnimalQueryMenu.WantsTreatTomorrow"));
                        }
                        else
                        {
                            _hoverText.SetValue(DataLoader.i18n.Get("Menu.AnimalQueryMenu.WantsTreat"));
                        }

                    }
                }
            }
        }

        public override void draw(SpriteBatch b)
        {
            bool movingAnimal = _movingAnimal.GetValue();
            bool confirmingSell = _confirmingSell.GetValue();
            double loveLevel = _lovelLevel.GetValue();
            string hoverText = _hoverText.GetValue();

            if (!movingAnimal && !Game1.globalFade)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen + Game1.tileSize * 2, AnimalQueryMenu.width, AnimalQueryMenu.height - Game1.tileSize * 2, false, true, (string)null, false);
                if ((int)this._farmAnimal.harvestType != 2)
                    this._textBox.Draw(b);
                int num1 = (this._farmAnimal.age + 1) / 28 + 1;
                string text1;
                if (num1 > 1)
                    text1 = Game1.content.LoadString("Strings\\UI:AnimalQuery_AgeN", (object)num1);
                else
                    text1 = Game1.content.LoadString("Strings\\UI:AnimalQuery_Age1");
                if (this._farmAnimal.age < (int)this._farmAnimal.ageWhenMature)
                    text1 += Game1.content.LoadString("Strings\\UI:AnimalQuery_AgeBaby");
                Utility.drawTextWithShadow(b, text1, Game1.smallFont, new Vector2((float)(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize / 2), (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4 + Game1.tileSize * 2)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                int num2 = 0;
                if (this._parentName != null)
                {
                    num2 = Game1.tileSize / 3;
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AnimalQuery_Parent", (object)this._parentName), Game1.smallFont, new Vector2((float)(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize / 2), (float)(Game1.tileSize / 2 + this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4 + Game1.tileSize * 2)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                }
                int num3 = loveLevel * 1000.0 % 200.0 >= 100.0 ? (int)(loveLevel * 1000.0 / 200.0) : -100;
                for (int index = 0; index < 5; ++index)
                {
                    b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 / 2 + 8 * Game1.pixelZoom * index), (float)(num2 + this.yPositionOnScreen - Game1.tileSize / 2 + Game1.tileSize * 5)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(211 + (loveLevel * 1000.0 <= (double)((index + 1) * 195) ? 7 : 0), 428, 7, 6)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.89f);
                    if (num3 == index)
                        b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 / 2 + 8 * Game1.pixelZoom * index), (float)(num2 + this.yPositionOnScreen - Game1.tileSize / 2 + Game1.tileSize * 5)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(211, 428, 4, 6)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.891f);
                }
                Utility.drawTextWithShadow(b, Game1.parseText(this._farmAnimal.getMoodMessage(), Game1.smallFont, AnimalQueryMenu.width - IClickableMenu.spaceToClearSideBorder * 2 - Game1.tileSize), Game1.smallFont, new Vector2((float)(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize / 2), (float)(num2 + this.yPositionOnScreen + Game1.tileSize * 6 - Game1.tileSize + Game1.pixelZoom)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                this.okButton.draw(b);
                this.sellButton.draw(b);
                this.moveHomeButton.draw(b);
                if (this.allowReproductionButton != null)
                    this.allowReproductionButton.draw(b);
                // START pregancyStatus treatStatus meatButton
                if (this.pregnantStatus != null)
                    this.pregnantStatus.draw(b);
                if (this.treatStatus != null)
                    this.treatStatus.draw(b);
                if (this.meatButton != null)
                    this.meatButton.draw(b);
                // END PregancyStatus
                // ADDED || confirmingMeat
                if (confirmingSell || confirmingMeat)
                {
                    b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                    Game1.drawDialogueBox(Game1.viewport.Width / 2 - Game1.tileSize * 5 / 2, Game1.viewport.Height / 2 - Game1.tileSize * 3, Game1.tileSize * 5, Game1.tileSize * 4, false, true, (string)null, false);
                    string text2 = Game1.content.LoadString("Strings\\UI:AnimalQuery_ConfirmSell");
                    b.DrawString(Game1.dialogueFont, text2, new Vector2((float)(Game1.viewport.Width / 2) - Game1.dialogueFont.MeasureString(text2).X / 2f, (float)(Game1.viewport.Height / 2 - Game1.tileSize * 3 / 2 + 8)), Game1.textColor);
                    this.yesButton.draw(b);
                    this.noButton.draw(b);
                }
                else if (hoverText != null && hoverText.Length > 0)
                    IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
            }
            else if (!Game1.globalFade)
            {
                string text = Game1.content.LoadString("Strings\\UI:AnimalQuery_ChooseBuilding", (object)this._farmAnimal.displayHouse, (object)this._farmAnimal.displayType);
                Game1.drawDialogueBox(Game1.tileSize / 2, -Game1.tileSize, (int)Game1.dialogueFont.MeasureString(text).X + IClickableMenu.borderWidth * 2 + Game1.tileSize / 4, Game1.tileSize * 2 + IClickableMenu.borderWidth * 2, false, true, (string)null, false);
                b.DrawString(Game1.dialogueFont, text, new Vector2((float)(Game1.tileSize / 2 + IClickableMenu.spaceToClearSideBorder * 2 + 8), (float)(Game1.tileSize / 2 + Game1.pixelZoom * 3)), Game1.textColor);
                this.okButton.draw(b);
            }
            this.drawMouse(b);
        }
    }
}