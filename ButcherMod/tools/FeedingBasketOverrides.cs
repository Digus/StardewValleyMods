﻿using System;
using System.Collections.Generic;
using System.Linq;
using AnimalHusbandryMod.animals;
using AnimalHusbandryMod.common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Tools;

namespace AnimalHusbandryMod.tools
{
    public class FeedingBasketOverrides
    {
        internal static string FeedingBasketKey = "DIGUS.ANIMALHUSBANDRYMOD/FeedingBasket";

        internal static readonly Dictionary<string, FarmAnimal> Animals = new Dictionary<string, FarmAnimal>();
        internal static readonly Dictionary<string, Pet> Pets = new Dictionary<string, Pet>();

        public static int InitialParentTileIndex = 519;
        public static int IndexOfMenuItemView = 519;
        public static int AttachmentMenuTile = 73;

        public static bool getOne(MilkPail __instance, ref Item __result)
        {
            if (!IsFeedingBasket(__instance)) return true;

            __result = (Item)ToolsFactory.GetFeedingBasket();
            return false;
        }

        public static bool loadDisplayName(MilkPail __instance, ref string __result)
        {
            if (!IsFeedingBasket(__instance)) return true;

            __result = DataLoader.i18n.Get("Tool.FeedingBasket.Name");
            return false;
        }

        public static bool loadDescription(MilkPail __instance, ref string __result)
        {
            if (!IsFeedingBasket(__instance)) return true;

            __result = DataLoader.i18n.Get("Tool.FeedingBasket.Description");
            return false;
        }

        public static bool canBeTrashed(MilkPail __instance, ref bool __result)
        {
            if (!IsFeedingBasket(__instance)) return true;

            __result = true;
            return false;
        }

        public static bool beginUsing(MilkPail __instance, GameLocation location, int x, int y, StardewValley.Farmer who, ref bool __result)
        {
            if (!IsFeedingBasket(__instance)) return true;

            string feedingBasketId = __instance.modData[FeedingBasketKey];

            x = (int)who.GetToolLocation(false).X;
            y = (int)who.GetToolLocation(false).Y;
            Rectangle rectangle = new Rectangle(x - Game1.tileSize / 2, y - Game1.tileSize / 2, Game1.tileSize, Game1.tileSize);
            // Added this because for some wierd reason the current value appears subtracted by 5 the first time the tool is used.
            __instance.CurrentParentTileIndex = InitialParentTileIndex;

            if (Context.IsMainPlayer && !DataLoader.ModConfig.DisableTreats)
            {
                if (location is Farm farm)
                {
                    foreach (FarmAnimal farmAnimal in farm.animals.Values)
                    {
                        if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                        {
                            Animals[feedingBasketId] = farmAnimal;
                            break;
                        }
                    }
                    if (!Animals.ContainsKey(feedingBasketId) || Animals[feedingBasketId] == null)
                    {
                        foreach (Pet localPet in farm.characters.Where(i => i is Pet))
                        {
                            if (localPet.GetBoundingBox().Intersects(rectangle))
                            {
                                Pets[feedingBasketId] = localPet;
                                break;
                            }
                        }
                    }
                }
                else if (location is AnimalHouse animalHouse)
                {
                    foreach (FarmAnimal farmAnimal in animalHouse.animals.Values)
                    {
                        if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                        {
                            Animals[feedingBasketId] = farmAnimal;
                            break;
                        }
                    }
                }
                else if (location is FarmHouse)
                {
                    foreach (Pet localPet in location.characters.Where(i => i is Pet))
                    {
                        if (localPet.GetBoundingBox().Intersects(rectangle))
                        {
                            Pets[feedingBasketId] = localPet;
                            break;
                        }
                    }
                }
            }

            Animals.TryGetValue(feedingBasketId, out FarmAnimal animal);
            if (animal != null)
            {
                string dialogue = "";
                if (__instance.attachments[0] == null)
                {
                    Game1.showRedMessage(DataLoader.i18n.Get("Tool.FeedingBasket.Empty"));
                    Animals[feedingBasketId] = animal = null;
                }
                else if (!TreatsController.CanReceiveTreat(animal))
                {
                    dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.NotLikeTreat", new { itemName = __instance.attachments[0].DisplayName });
                }
                else if (!TreatsController.IsLikedTreat(animal, __instance.attachments[0].ParentSheetIndex) && !TreatsController.IsLikedTreat(animal, __instance.attachments[0].Category))
                {
                    dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.NotLikeTreat", new { itemName = __instance.attachments[0].DisplayName });
                }
                else if (__instance.attachments[0].Category == StardewValley.Object.MilkCategory && !animal.isBaby())
                {
                    dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.OnlyBabiesCanEatMilk");
                }
                else if (!TreatsController.IsReadyForTreat(animal))
                {
                    if (TreatsController.GetTreatItem(animal)?.MinimumDaysBetweenTreats == 1)
                    {
                        dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.AlreadyAteTreatToday", new { animalName = animal.displayName });
                    }
                    else
                    {
                        int daysUntilNextTreat = TreatsController.DaysUntilNextTreat(animal);
                        if (daysUntilNextTreat > 1)
                        {
                            dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.WantsTreatInDays", new { animalName = animal.displayName, numberOfDays = daysUntilNextTreat });
                        }
                        else if (daysUntilNextTreat == 1)
                        {
                            dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.WantsTreatTomorrow", new { animalName = animal.displayName });
                        }
                    }
                }
                else
                {
                    animal.pauseTimer = 1000;
                }


                if (dialogue.Length > 0)
                {
                    DelayedAction.showDialogueAfterDelay(dialogue, 150);
                    Animals[feedingBasketId] = animal = null;
                }
            }
            Pets.TryGetValue(feedingBasketId, out Pet pet);
            if (pet != null)
            {
                string dialogue = "";
                if (__instance.attachments[0] == null)
                {
                    Game1.showRedMessage(DataLoader.i18n.Get("Tool.FeedingBasket.Empty"));
                    Pets[feedingBasketId] = null;
                }
                else if (!TreatsController.IsLikedTreatPet(__instance.attachments[0].ParentSheetIndex) && !TreatsController.IsLikedTreatPet(__instance.attachments[0].Category))
                {
                    dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.NotLikeTreat", new { itemName = __instance.attachments[0].DisplayName });
                }
                else if (!TreatsController.IsReadyForTreatPet())
                {
                    int daysUntilNextTreat = TreatsController.DaysUntilNextTreatPet();

                    if (DataLoader.AnimalData.Pet.MinimumDaysBetweenTreats == 1)
                    {
                        dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.AlreadyAteTreatToday", new { animalName = pet.displayName });
                    }
                    else if (daysUntilNextTreat > 1)
                    {
                        dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.WantsTreatInDays", new { animalName = pet.displayName, numberOfDays = daysUntilNextTreat });
                    }
                    else if (daysUntilNextTreat == 1)
                    {
                        dialogue = DataLoader.i18n.Get("Tool.FeedingBasket.WantsTreatTomorrow", new { animalName = pet.displayName });
                    }
                }
                else
                {
                    pet.Halt();
                    pet.CurrentBehavior = 0;
                    pet.OnNewBehavior();
                    pet.Halt();
                    pet.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>() { new FarmerSprite.AnimationFrame(18, 200) });
                }

                if (dialogue.Length > 0)
                {
                    DelayedAction.showDialogueAfterDelay(dialogue, 150);
                    Pets[feedingBasketId] = pet = null;
                }
            }

            who.Halt();
            int currentFrame = who.FarmerSprite.currentFrame;
            if (animal != null || pet != null)
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
            }
            else
            {
                who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1] { new FarmerSprite.AnimationFrame(currentFrame, 0, false, who.FacingDirection == 3, new AnimatedSprite.endOfAnimationBehavior(StardewValley.Farmer.useTool), true) });
            }
            who.FarmerSprite.oldFrame = currentFrame;
            who.UsingTool = true;
            who.CanMove = false;

            if (animal != null || pet != null)
            {
                Rectangle boundingBox;
                boundingBox = animal != null ? animal.GetBoundingBox() : pet.GetBoundingBox();

                double numX = boundingBox.Center.X;
                double numY = boundingBox.Center.Y;

                Vector2 vectorBasket = new Vector2((float)numX - 32, (float)numY);
                Vector2 vectorFood = new Vector2((float)numX - 24, (float)numY - 10);
                var foodScale = Game1.pixelZoom * 0.75f;

                Multiplayer multiplayer = DataLoader.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

                TemporaryAnimatedSprite basketSprite = new TemporaryAnimatedSprite(Game1.toolSpriteSheetName,
                    Game1.getSourceRectForStandardTileSheet(Game1.toolSpriteSheet, __instance.CurrentParentTileIndex, 16, 16),
                    750.0f, 1, 1, vectorBasket, false, false, ((float)boundingBox.Bottom + 0.1f) / 10000f, 0.0f,
                    Color.White, Game1.pixelZoom, 0.0f, 0.0f, 0.0f)
                { delayBeforeAnimationStart = 100 };
                multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[1] { basketSprite });
                TemporaryAnimatedSprite foodSprite = new TemporaryAnimatedSprite(Game1.objectSpriteSheetName,
                    Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, __instance.attachments[0].ParentSheetIndex,
                    16, 16), 500.0f, 1, 1, vectorFood, false, false, ((float)boundingBox.Bottom + 0.2f) / 10000f, 0.0f,
                    Color.White, foodScale, 0.0f, 0.0f, 0.0f)
                { delayBeforeAnimationStart = 100 };
                multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[1] { foodSprite });

                for (int index = 0; index < 8; ++index)
                {
                    Rectangle standardTileSheet = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet,
                        __instance.attachments[0].ParentSheetIndex, 16, 16);
                    standardTileSheet.X += 8;
                    standardTileSheet.Y += 8;

                    standardTileSheet.Width = Game1.pixelZoom;
                    standardTileSheet.Height = Game1.pixelZoom;
                    TemporaryAnimatedSprite temporaryAnimatedSprite2 =
                        new TemporaryAnimatedSprite(Game1.objectSpriteSheetName, standardTileSheet, 400f, 1, 0,
                            vectorFood + new Vector2(12, 12), false, false,
                            ((float)boundingBox.Bottom + 0.2f) / 10000f, 0.0f, Color.White, (float)foodScale, 0.0f,
                            0.0f, 0.0f, false)
                        {
                            motion = new Vector2((float)Game1.random.Next(-30, 31) / 10f, (float)Game1.random.Next(-6, -3)),
                            acceleration = new Vector2(0.0f, 0.5f),
                            delayBeforeAnimationStart = 600
                        };
                    multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[1] { temporaryAnimatedSprite2 });
                }

                if (animal != null)
                {
                    FarmAnimal tempAnimal = animal;
                    Game1.delayedActions.Add(new DelayedAction(300, new DelayedAction.delayedBehavior(() => {
                        AnimalHusbandryModEntry.ModHelper.Reflection.GetField<NetBool>(tempAnimal, "isEating").GetValue().Value = true;
                        tempAnimal.Sprite.loop = false;
                    })));
                }
                else if (pet != null)
                {

                    pet.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(18, 300),
                        new FarmerSprite.AnimationFrame(17, 100),
                        new FarmerSprite.AnimationFrame(16, 100),
                        new FarmerSprite.AnimationFrame(0, 100),
                        new FarmerSprite.AnimationFrame(16, 100),
                        new FarmerSprite.AnimationFrame(17, 100),
                        new FarmerSprite.AnimationFrame(18, 300)
                    });
                    pet.Sprite.loop = false;
                }
                DelayedAction.playSoundAfterDelay("eat", 600);
            }
            __result = true;
            return false;
        }

        public static bool DoFunction(MilkPail __instance, GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            if (!IsFeedingBasket(__instance)) return true;

            string feedingBasketId = __instance.modData[FeedingBasketKey];

            AnimalHusbandryModEntry.ModHelper.Reflection.GetField<StardewValley.Farmer>(__instance, "lastUser").SetValue(who);
            Game1.recentMultiplayerRandom = new Random((int)(short)Game1.random.Next((int)short.MinValue, 32768));
            __instance.CurrentParentTileIndex = InitialParentTileIndex;
            __instance.indexOfMenuItemView.Value = IndexOfMenuItemView;

            Animals.TryGetValue(feedingBasketId, out FarmAnimal animal);
            if (animal != null)
            {
                animal.doEmote(20, true);

                if (!DataLoader.ModConfig.DisableFriendshipInscreseWithTreats)
                {
                    double professionAjust = 1.0;
                    if (!animal.isCoopDweller() && who.professions.Contains(3) || animal.isCoopDweller() && who.professions.Contains(2))
                    {
                        professionAjust += DataLoader.ModConfig.PercentualAjustOnFriendshipInscreaseFromProfessions;
                    }
                    animal.friendshipTowardFarmer.Value = Math.Min(1000, animal.friendshipTowardFarmer.Value + (int)Math.Ceiling(professionAjust * __instance.attachments[0].Price * (1.0 + __instance.attachments[0].Quality * 0.25) / (animal.price.Value / 1000.0)));
                }
                if (!DataLoader.ModConfig.DisableMoodInscreseWithTreats)
                {
                    animal.happiness.Value = byte.MaxValue;
                }

                if (DataLoader.ModConfig.EnableTreatsCountAsAnimalFeed)
                {
                    animal.fullness.Value = byte.MaxValue;
                }

                TreatsController.FeedAnimalTreat(animal, __instance.attachments[0]);

                if (__instance.attachments[0].Category == StardewValley.Object.MilkCategory)
                {
                    animal.age.Value = Math.Min(animal.ageWhenMature.Value - 1, animal.age.Value + 1);
                }

                --__instance.attachments[0].Stack;
                if (__instance.attachments[0].Stack <= 0)
                {
                    Game1.showGlobalMessage(DataLoader.i18n.Get("Tool.FeedingBasket.ItemConsumed", new { itemName = __instance.attachments[0].DisplayName }));
                    __instance.attachments[0] = (StardewValley.Object)null;
                }
            }
            if (Pets.TryGetValue(feedingBasketId, out Pet pet) && pet != null)
            {
                pet.doEmote(20, true);

                if (!DataLoader.ModConfig.DisableFriendshipInscreseWithTreats)
                {
                    pet.friendshipTowardFarmer.Value = Math.Min(Pet.maxFriendship, pet.friendshipTowardFarmer.Value + 6);
                }
                TreatsController.FeedPetTreat(__instance.attachments[0]);

                --__instance.attachments[0].Stack;
                if (__instance.attachments[0].Stack <= 0)
                {
                    Game1.showGlobalMessage(DataLoader.i18n.Get("Tool.FeedingBasket.ItemConsumed", new { itemName = __instance.attachments[0].DisplayName }));
                    __instance.attachments[0] = (StardewValley.Object)null;
                }
            }
            Animals[feedingBasketId] = (FarmAnimal)null;
            Pets[feedingBasketId] = (Pet)null;

            if (Game1.activeClickableMenu == null)
            {
                who.CanMove = true;
                who.completelyStopAnimatingOrDoingAction();
            }
            else
            {
                who.Halt();
            }
            who.UsingTool = false;
            who.canReleaseTool = true;

            DataLoader.Helper.Reflection.GetMethod(__instance, "finish").Invoke();

            return false;
        }

        public static bool canThisBeAttached(MilkPail __instance, StardewValley.Object o, ref bool __result)
        {
            if (!IsFeedingBasket(__instance)) return true;

            if (o == null
                || o.Category == StardewValley.Object.VegetableCategory
                || o.Category == StardewValley.Object.FruitsCategory
                || TreatsController.IsLikedTreat(o.ParentSheetIndex)
                || TreatsController.IsLikedTreat(o.Category)
            )
            {
                __result = true;
                return false;
            }
            __result = false;
            return false;
        }

        public static bool attach(MilkPail __instance, StardewValley.Object o, ref StardewValley.Object __result)
        {
            if (!IsFeedingBasket(__instance)) return true;

            if (o != null)
            {
                StardewValley.Object @object = __instance.attachments[0];
                if (@object != null && @object.canStackWith((Item)o))
                {
                    @object.Stack = o.addToStack(@object);
                    if (@object.Stack <= 0)
                        @object = (StardewValley.Object)null;
                }
                __instance.attachments[0] = o;
                Game1.playSound("button1");
                __result = @object;
                return false;
            }
            else
            {
                if (__instance.attachments[0] != null)
                {
                    StardewValley.Object attachment = __instance.attachments[0];
                    __instance.attachments[0] = (StardewValley.Object)null;
                    Game1.playSound("dwop");
                    __result = attachment;
                    return false;
                }
            }
            __result = (StardewValley.Object)null;
            return false;
        }

        public static bool drawAttachments(MilkPail __instance, SpriteBatch b, int x, int y)
        {
            if (!IsFeedingBasket(__instance)) return true;

            if (__instance.attachments[0] != null)
            {
                b.Draw(Game1.menuTexture, new Vector2((float)x, (float)y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
                __instance.attachments[0].drawInMenu(b, new Vector2((float)x, (float)y), 1f);
            }
            else
            {
                b.Draw(Game1.menuTexture, new Vector2((float)x, (float)y), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, AttachmentMenuTile, -1, -1)), Microsoft.Xna.Framework.Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);
            }
            return false;
        }

        private static bool IsFeedingBasket(MilkPail tool)
        {
            return tool.modData.ContainsKey(FeedingBasketKey);
        }
    }
}
