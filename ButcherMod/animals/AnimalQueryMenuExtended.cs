using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace AnimalHusbandryMod.animals
{
    public class AnimalQueryMenuExtended : AnimalQueryMenu
    {
        private FarmAnimal _farmAnimal;
        private ClickableTextureComponent pregnantStatus = null;

        public AnimalQueryMenuExtended(FarmAnimal farmAnimal) : base(farmAnimal)
        {
            _farmAnimal = farmAnimal;
            if (PregnancyController.IsAnimalPregnant(this._farmAnimal.myID))
            {                
                this.allowReproductionButton = null;
                pregnantStatus = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + AnimalQueryMenu.width + Game1.pixelZoom * 4, this.yPositionOnScreen + AnimalQueryMenu.height - Game1.tileSize * 2 - IClickableMenu.borderWidth + Game1.pixelZoom * 2, Game1.pixelZoom * 9, Game1.pixelZoom * 9), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle( 128, 393, 9, 9), 4f, false);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (DataLoader.Helper.Reflection.GetPrivateValue<bool>(this, "movingAnimal"))
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
                if (this.pregnantStatus != null)
                {
                    if (this.pregnantStatus.containsPoint(x, y))
                    {
                        int daysUtilBirth = PregnancyController.GetPregnancyItem(this._farmAnimal.myID).DaysUntilBirth;
                        if (daysUtilBirth > 1)
                        {
                            DataLoader.Helper.Reflection.GetPrivateField<string>(this, "hoverText").SetValue(DataLoader.i18n.Get("Menu.AnimalQueryMenu.DaysUntilBirth", new { numberOfDays = daysUtilBirth }));
                        }
                        else
                        {
                            DataLoader.Helper.Reflection.GetPrivateField<string>(this, "hoverText").SetValue(DataLoader.i18n.Get("Menu.AnimalQueryMenu.ReadyForBirth"));
                        }
                        
                    }
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (DataLoader.Helper.Reflection.GetPrivateValue<bool>(this, "movingAnimal"))
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
            base.receiveLeftClick(x, y, playSound);
        }

        public override void draw(SpriteBatch b)
        {
            bool isPregnantStatus = false;
            if (!DataLoader.Helper.Reflection.GetPrivateValue<bool>(this, "movingAnimal") && !Game1.globalFade)
            {
                if (pregnantStatus != null)
                {
                    this.allowReproductionButton = pregnantStatus;
                    isPregnantStatus = true;
                }
            }
            base.draw(b);
            if (isPregnantStatus)
            {
                this.allowReproductionButton = null;
            }
        }
    }
}
