using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace CropTransplantMod
{
    internal class ObjectOverrides
    {
        internal static Object RegularPotObject =  null;
        internal static HeldIndoorPot CurrentHeldIndoorPot = null;
        internal static bool shakeFlag = false;

        public static bool drawWithOffset(ref HoeDirt __instance, Vector2 tileLocation, Vector2 offset)
        {
            return true;
        }

        public static bool PressUseToolButton( ref bool __result)
        {
            if (Game1.fadeToBlack)
                return false;
            Game1.player.toolPower = 0;
            Game1.player.toolHold = 0;
            if (Game1.player.CurrentTool == null && Game1.player.ActiveObject == null)
            {
                Vector2 key = key = Game1.currentCursorTile;
                if (!Game1.currentLocation.Objects.ContainsKey(key) ||
                    !(Game1.currentLocation.Objects[key] is IndoorPot)
                    || !Utility.tileWithinRadiusOfPlayer((int)key.X, (int)key.Y, 1, Game1.player))
                {
                    key = Game1.player.GetToolLocation(false) / 64f;
                    key.X = (float)(int)key.X;
                    key.Y = (float)(int)key.Y;
                }
                
                if (Game1.currentLocation.Objects.ContainsKey(key))
                {
                    Object @object = Game1.currentLocation.Objects[key];
                    if (@object is IndoorPot pot)
                    {
                        pot.performRemoveAction(pot.TileLocation, Game1.currentLocation);
                        Game1.currentLocation.Objects.Remove(pot.TileLocation);
                        if (pot.hoeDirt.Value.crop != null )
                        {
                            CurrentHeldIndoorPot = new HeldIndoorPot(pot.TileLocation);
                            RegularPotObject = new Object(Vector2.Zero, 62, false);
                            Game1.player.ActiveObject = CurrentHeldIndoorPot;
                            GameEvents.UpdateTick += tickUpdate;
                            CurrentHeldIndoorPot.hoeDirt.Value.crop = pot.hoeDirt.Value.crop;
                            CurrentHeldIndoorPot.hoeDirt.Value.fertilizer.Value = pot.hoeDirt.Value.fertilizer.Value;
                            shakeCrop(CurrentHeldIndoorPot.hoeDirt.Value, pot.TileLocation);
                            Game1.player.Stamina -= ((float)4f - (float)Game1.player.FarmingLevel * 0.2f);
                        }
                        else
                        {
                            Game1.player.ActiveObject = new Object(Vector2.Zero, 62, false);
                        }
                        
                        __result = true;
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool PlayerCanPlaceItemHere(ref GameLocation location, Item item, ref int x, ref int y, ref Farmer f, ref bool __result)
        {
            if (item != null && item is Object object1 && object1.ParentSheetIndex == 62 && object1.bigCraftable.Value && !(object1 is HeldIndoorPot))
            {
                if ((Game1.eventUp || f.bathingClothes.Value) || !Utility.withinRadiusOfPlayer(x, y, 1, f) && (!Utility.withinRadiusOfPlayer(x, y, 2, f) || !Game1.isAnyGamePadButtonBeingPressed() || (double)Game1.mouseCursorTransparency != 0.0))
                    return true;

                Vector2 tileLocation = new Vector2((float)(x / 64), (float)(y / 64));
                if (location.isTileHoeDirt(tileLocation) && !location.objects.ContainsKey(tileLocation))
                {
                    HoeDirt hoeDirt = location.terrainFeatures[tileLocation] as HoeDirt;
                    if (hoeDirt.crop != null)
                    {
                        __result = true;
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool CanGrabSomethingFromHere(int x, int y, ref Farmer who, ref bool __result)
        {
            if (Game1.currentLocation == null)
                return false;
            Vector2 index = new Vector2((float)(x / 64), (float)(y / 64));
            if (who.ActiveObject is IndoorPot || (who.ActiveObject is Object object1 && object1.ParentSheetIndex == 62 && object1.bigCraftable.Value))
            {
                __result = false;
                return false;
            } else if (who.CurrentTool == null && who.ActiveObject == null && Game1.currentLocation.Objects.ContainsKey(index) && Game1.currentLocation.Objects[index] is IndoorPot pot)
            {
                Game1.mouseCursorTransparency = !Utility.tileWithinRadiusOfPlayer((int)index.X, (int)index.Y, 1, who) ? 0.5f : 1f;
                Game1.mouseCursor = 2;
                __result = false;
                return false;
            }
            return true;
        }

        public static bool PerformUseAction(ref bool __result)
        {
            if (Game1.player.ActiveObject is IndoorPot || (Game1.player.ActiveObject is Object object1 && object1.ParentSheetIndex == 62 && object1.bigCraftable.Value))
            {
                __result = false;
                return false;
            }
            return true;
        }

        public static bool TryToPlaceItem(ref GameLocation location, Item item, ref int x, ref int y)
        {
            if (Utility.withinRadiusOfPlayer(x, y, 1, Game1.player) && item is Object object1 && object1.ParentSheetIndex == 62 && object1.bigCraftable.Value)
            {
                Vector2 tileLocation = new Vector2((float) (x / 64), (float) (y / 64));
                if (location.isTileHoeDirt(tileLocation))
                {
                    if (!location.objects.ContainsKey(tileLocation))
                    {
                        HoeDirt hoeDirt = location.terrainFeatures[tileLocation] as HoeDirt;
                        if (hoeDirt.crop != null)
                        {
                            if (!(Game1.player.ActiveObject is HeldIndoorPot heldPot) ||
                                heldPot.hoeDirt.Value.crop == null)
                            {
                                CurrentHeldIndoorPot = new HeldIndoorPot(tileLocation);
                                RegularPotObject = Game1.player.ActiveObject;
                                Game1.player.ActiveObject = CurrentHeldIndoorPot;
                                GameEvents.UpdateTick += tickUpdate;
                                CurrentHeldIndoorPot.hoeDirt.Value.crop = hoeDirt.crop;
                                CurrentHeldIndoorPot.hoeDirt.Value.fertilizer.Value = hoeDirt.fertilizer.Value;
                                shakeCrop(CurrentHeldIndoorPot.hoeDirt.Value, tileLocation);
                                hoeDirt.crop = null;
                                hoeDirt.fertilizer.Value = 0;
                                Game1.player.Stamina -= ((float) 4f - (float) Game1.player.FarmingLevel * 0.2f);
                                location.playSound("dirtyHit");
                                return false;
                            }
                        }
                        else
                        {
                            if (Game1.player.ActiveObject is HeldIndoorPot heldPot &&
                                heldPot.hoeDirt.Value.crop != null
                                && !location.farmers.Any(f=>f.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64))))
                            {
                                hoeDirt.crop = heldPot.hoeDirt.Value.crop;
                                shakeCrop(hoeDirt, tileLocation);
                                hoeDirt.fertilizer.Value = heldPot.hoeDirt.Value.fertilizer.Value;
                                GameEvents.UpdateTick -= tickUpdate;
                                Game1.player.ActiveObject = RegularPotObject;
                                location.playSound("dirtyHit");
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        internal static void shakeCrop(HoeDirt hoeDirt, Vector2? tileLocation = null)
        {
            var maxShake = hoeDirt.getMaxShake();
            if (hoeDirt.crop != null && hoeDirt.crop.currentPhase.Value != 0 && (double) maxShake == 0.0)
            {
                Grass.grassSound = Game1.soundBank.GetCue("grassyStep");
                Grass.grassSound.Play();

                var farmer = Game1.player;
                int speedOfCollision = 2;
                
                hoeDirt.shake(
                    (float)(0.392699092626572 / (double)((5 + farmer.addedSpeed) / speedOfCollision) -(speedOfCollision > 2 ? (double)hoeDirt.crop.currentPhase.Value * 3.14159274101257 / 64.0: 0.0))
                    , (float)Math.PI / 80f / (float)((5 + farmer.addedSpeed) / speedOfCollision)
                    ,tileLocation.HasValue ? (double)farmer.lastPosition.X > (double)tileLocation.Value.X * 64.0 + 32.0 : farmer.FacingDirection == 1 ? true : farmer.FacingDirection == 3 ? false : shakeFlag = !shakeFlag);
            }
        }

        private static void tickUpdate(object sender, EventArgs e)
        {
            if (Game1.player.ActiveObject is HeldIndoorPot heldPot)
            {
                if (heldPot.hoeDirt.Value.crop != null)
                {
                    heldPot.hoeDirt.Value.tickUpdate(Game1.currentGameTime, heldPot.TileLocation, Game1.currentLocation);
                    if (Game1.player.isMoving())
                    {
                        shakeCrop(heldPot.hoeDirt.Value);
                    }
                }
            }
            else
            {
                if (CurrentHeldIndoorPot != null && Game1.player.getIndexOfInventoryItem(CurrentHeldIndoorPot) != -1)
                {
                    var f = Game1.player;
                    Multiplayer multiplayer = DataLoader.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                    multiplayer.broadcastSprites(Game1.player.currentLocation, new TemporaryAnimatedSprite[1]
                    {
                        new TemporaryAnimatedSprite("TileSheets\\animations",new Rectangle(0, 50 * 64, 64, 64), 100f,8,0, f.Position + new Vector2(0.0f, -120f), false, false, (float)((double)f.getStandingY() / 10000.0 - 1.0 / 1000.0), 0.0f, Color.ForestGreen, 1f, 0.0f, 0.0f, 0.0f, false)
                    });
                    Grass.grassSound = Game1.soundBank.GetCue("grassyStep");
                    Grass.grassSound.Play();
                }
                int heldIndorPotInveotryPosition = Game1.player.getIndexOfInventoryItem(CurrentHeldIndoorPot);
                Game1.player.removeItemFromInventory(CurrentHeldIndoorPot);
                Game1.player.addItemToInventory(RegularPotObject, heldIndorPotInveotryPosition);
                CurrentHeldIndoorPot = null;
                GameEvents.UpdateTick -= tickUpdate;
            }
        }

        private static void showAnimation()
        {
            Game1.player.canMove = false;
            switch (Game1.player.facingDirection)
            {
                case 0:
                    Game1.player.FarmerSprite.animateOnce(
                    new FarmerSprite.AnimationFrame[6]
                    {
                        new FarmerSprite.AnimationFrame(62, 0, false, false,
                            (AnimatedSprite.endOfAnimationBehavior) null, false),
                        new FarmerSprite.AnimationFrame(62, 100, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(63, 100, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(64, 100, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(65, 100, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(65, 0, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), false)
                    });
                    break;
                case 1:
                    Game1.player.FarmerSprite.animateOnce(
                        new FarmerSprite.AnimationFrame[6]
                    {
                        new FarmerSprite.AnimationFrame(58, 0, false, false,
                            (AnimatedSprite.endOfAnimationBehavior) null, false),
                        new FarmerSprite.AnimationFrame(58, 100, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(59, 100, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(60, 100, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(61, 100, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(61, 0, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), false)
                    });
                    break;
                case 2:
                    Game1.player.FarmerSprite.animateOnce(
                        new FarmerSprite.AnimationFrame[6]
                    {
                        new FarmerSprite.AnimationFrame(54, 0, false, false,
                            (AnimatedSprite.endOfAnimationBehavior) null, false),
                        new FarmerSprite.AnimationFrame(54, 100, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(55, 100, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(56, 100, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(57, 100, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(57, 0, false, false,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), false)
                    });
                    break;
                case 3:
                    Game1.player.FarmerSprite.animateOnce(
                        new FarmerSprite.AnimationFrame[6]
                    {
                        new FarmerSprite.AnimationFrame(58, 0, false, true,
                            (AnimatedSprite.endOfAnimationBehavior) null, false),
                        new FarmerSprite.AnimationFrame(58, 100, false, true,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(59, 100, false, true,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(60, 100, false, true,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(61, 200, false, true,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.showItemIntake), false),
                        new FarmerSprite.AnimationFrame(61, 0, false, true,
                            new AnimatedSprite.endOfAnimationBehavior(Farmer.canMoveNow), false)
                    });
                    break;
            }
            Game1.player.FarmerSprite.PauseForSingleAnimation = true;
        }
    }
}