using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace CropTransplantMod
{
    public class HeldIndoorPot : IndoorPot
    {
        public HeldIndoorPot():base()
        {
        }

        public HeldIndoorPot(Vector2 tileLocation) : base(tileLocation)
        {
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            bool result = base.placementAction(location, x, y, who);
            if (this.hoeDirt.Value.crop != null)
            {
                Vector2 index1 = new Vector2((float)(x / 64), (float)(y / 64));
                if(location.objects[index1] is IndoorPot pot)
                {
                    pot.hoeDirt.Value.crop = this.hoeDirt.Value.crop;
                    pot.hoeDirt.Value.fertilizer.Value = this.hoeDirt.Value.fertilizer.Value;
                    ObjectOverrides.shakeCrop(pot.hoeDirt.Value, index1);
                }
            }
            return result;
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            Vector2 vector2 = this.getScale() * 4f;
            Vector2 local = objectPosition;
            var x = local.X / 64;
            var y = (local.Y + 64) / 64;
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet, objectPosition, new Rectangle?(Object.getSourceRectForBigCraftable(f.ActiveObject.ParentSheetIndex)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f));
            if (this.hoeDirt.Value.fertilizer.Value != 0)
            {
                int num = 0;
                switch (this.hoeDirt.Value.fertilizer.Value)
                {
                    case 369:
                        num = 1;
                        break;
                    case 370:
                        num = 2;
                        break;
                    case 371:
                        num = 3;
                        break;
                    case 465:
                        num = 4;
                        break;
                    case 466:
                        num = 5;
                        break;
                }
                spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)((double)objectPosition.X + 4.0), (float)(objectPosition.Y + 52)), new Rectangle?(new Rectangle(173 + num / 2 * 16, 466 + num % 2 * 16, 13, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f)+0.0001f);
            }
            if (this.hoeDirt.Value.crop != null)
                DrawWithOffset(this.hoeDirt.Value.crop, spriteBatch, objectPosition, this.hoeDirt.Value.state.Value != 1 || this.hoeDirt.Value.crop.currentPhase.Value != 0 || this.hoeDirt.Value.crop.raisedSeeds.Value ? Color.White : new Color(180, 100, 200) * 1f, this.hoeDirt.Value.getShakeRotation(), new Vector2(32f, 72),f);
            if (this.heldObject.Value == null)
                return;
            this.heldObject.Value.draw(spriteBatch, (int)x * 64, (int)(y * 64 - 48), (float)((objectPosition.Y + 0.660000026226044) * 64.0 / 10000.0 + (double)x * 9.99999974737875E-06), 1f);
        }

        private void DrawWithOffset(Crop crop, SpriteBatch b, Vector2 tileLocation, Color toTint, float rotation, Vector2 offset, Farmer f)
        {
            if (crop.forageCrop.Value)
            {
                b.Draw(Game1.mouseCursors,  offset + tileLocation, new Rectangle?(new Rectangle((int)((double)tileLocation.X * 51.0 + (double)tileLocation.Y * 77.0) % 3 * 16, 128 + crop.whichForageCrop.Value * 16, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0002f);
            }
            else
            {
                Rectangle? sourceRect = null;
                if (crop.currentPhase.Value > 0)
                {
                    sourceRect = GetSourceRect(crop, (int) tileLocation.X * 7 + (int) tileLocation.Y * 11);
                }
                else
                {
                    sourceRect = GetSourceRect(crop, (int)base.tileLocation.X * 7 + (int)base.tileLocation.Y * 11);
                }
                b.Draw(Game1.cropSpriteSheet,  offset + tileLocation, sourceRect, toTint, rotation, new Vector2(8f, 24f), 4f, crop.flip.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0002f);
                if (crop.tintColor.Value.Equals((object)Color.White) || crop.currentPhase.Value != crop.phaseDays.Count - 1 || crop.dead.Value)
                    return;
                b.Draw(Game1.cropSpriteSheet, offset + tileLocation, new Rectangle?(new Rectangle((crop.fullyGrown.Value ? (crop.dayOfCurrentPhase.Value <= 0 ? 6 : 7) : crop.currentPhase.Value + 1 + 1) * 16 + (crop.rowInSpriteSheet.Value % 2 != 0 ? 128 : 0), crop.rowInSpriteSheet.Value / 2 * 16 * 2, 16, 32)), crop.tintColor.Value, rotation, new Vector2(8f, 24f), 4f, crop.flip.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0.0f, (float)(f.getStandingY() + 2) / 10000f) + 0.0003f);
            }
        }

        private Rectangle GetSourceRect(Crop crop, int number)
        {
            if (crop.dead.Value)
                return new Rectangle(192 + number % 4 * 16, 384, 16, 32);
            return new Rectangle(Math.Min(240, (crop.fullyGrown.Value ? (crop.dayOfCurrentPhase.Value <= 0 ? 6 : 7) : (int)(crop.phaseToShow.Value != -1 ? crop.phaseToShow.Value : crop.currentPhase.Value) + ((int)(crop.phaseToShow.Value != -1 ? crop.phaseToShow.Value : crop.currentPhase.Value) != 0 || number % 2 != 0 ? 0 : -1) + 1) * 16 + (crop.rowInSpriteSheet.Value % 2 != 0 ? 128 : 0)), crop.rowInSpriteSheet.Value / 2 * 16 * 2, 16, 32);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            
        }
    }
}
