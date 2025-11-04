using System;
using System.Collections.Generic;
using System.Reflection;
using AnimalHusbandryMod.animals;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using DataLoader = AnimalHusbandryMod.common.DataLoader;

namespace AnimalHusbandryMod.tools
{
    public class MeatCleaverOverrides : ToolOverridesBase
    {
        public const string MeatCleaverItemId = "DIGUS.ANIMALHUSBANDRYMOD.MeatCleaver";
        internal const string MeatCleaverKey = "DIGUS.ANIMALHUSBANDRYMOD/MeatCleaver";

        private static readonly Dictionary<string, FarmAnimal> Animals = new Dictionary<string, FarmAnimal>();
        private static readonly Dictionary<string, FarmAnimal> TempAnimals = new Dictionary<string, FarmAnimal>();

        internal static string Suffix = "";

        public static bool GetOneNew(GenericTool __instance, ref Item __result)
        {
            if (!IsMeatCleaver(__instance)) return true;

            __result = (Item)ToolsFactory.GetMeatCleaver();
            return false;
        }

        public static void loadDisplayName(GenericTool __instance, ref string __result)
        {
            if (!IsMeatCleaver(__instance)) return;

            __result = DataLoader.i18n.Get("Tool.MeatCleaver.Name" + Suffix);
        }

        public static void loadDescription(GenericTool __instance, ref string __result)
        {
            if (!IsMeatCleaver(__instance)) return;

            __result = DataLoader.i18n.Get("Tool.MeatCleaver.Description" + Suffix);
        }

        public static void canBeTrashed(Item __instance, ref bool __result)
        {
            if (!IsMeatCleaver(__instance)) return;

            __result = true;
        }

        public static void CanAddEnchantment(Tool __instance, ref bool __result)
        {
            if (!IsMeatCleaver(__instance)) return;

            __result = false;
        }

        public static bool beginUsing(GenericTool __instance, GameLocation location, StardewValley.Farmer who, ref bool __result)
        {
            if (!IsMeatCleaver(__instance)) return true;
            if (who == null) return true;

            string meatCleaverId = __instance.modData[MeatCleaverKey];

            int x = (int)who.GetToolLocation(false).X;
            int y = (int)who.GetToolLocation(false).Y;
            Rectangle rectangle = new Rectangle(x - Game1.tileSize / 2, y - Game1.tileSize / 2, Game1.tileSize, Game1.tileSize);

            if (!DataLoader.ModConfig.DisableMeat && Game1.player.Equals(who))
            {
                if (location is not null)
                {
                    foreach (FarmAnimal farmAnimal in location.animals.Values)
                    {
                        if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                        {
                            if (TempAnimals.ContainsKey(meatCleaverId) && farmAnimal == TempAnimals[meatCleaverId])
                            {
                                Animals[meatCleaverId] = farmAnimal;
                            }
                            else
                            {
                                TempAnimals[meatCleaverId] = farmAnimal;
                                if (Game1.player.Equals(who))
                                {
                                    ICue hurtSound;
                                    if (!DataLoader.ModConfig.Softmode)
                                    {
                                        if (farmAnimal.GetSoundId() is { } soundId)
                                        {
                                            hurtSound = Game1.soundBank.GetCue(soundId);
                                            hurtSound.SetVariable("Pitch", 1800);
                                            hurtSound.Play();
                                        }
                                    }
                                    else
                                    {
                                        hurtSound = Game1.soundBank.GetCue("toolCharge");
                                        hurtSound.SetVariable("Pitch", 5000f);
                                        hurtSound.Play();
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }

            __instance.Update(who.FacingDirection, 0, who);
            if (TempAnimals.TryGetValue(meatCleaverId, out FarmAnimal tempAnimal) && tempAnimal != null)
            {
                if (tempAnimal.isBaby())
                {
                    if (Game1.player.Equals(who))
                    {
                        string dialogue = DataLoader.i18n.Get("Tool.MeatCleaver.TooYoung" + Suffix, new {animalName = tempAnimal.displayName});
                        DelayedAction.showDialogueAfterDelay(dialogue, 150);
                    }
                    TempAnimals[meatCleaverId] = null;
                } else if (DataLoader.ModConfig.Softermode && MeatController.CreateMeat(tempAnimal).Count == 0)
                {
                    if (Game1.player.Equals(who))
                    {
                        string dialogue = DataLoader.i18n.Get("Tool.MeatCleaver.NoMeat" + Suffix, new {animalName = tempAnimal.displayName});
                        DelayedAction.showDialogueAfterDelay(dialogue, 150);
                    }
                    TempAnimals[meatCleaverId] = null;
                }
            }
            who.EndUsingTool();
            __result = true;
            return false;
        }

        public static void DoFunction(GenericTool __instance, GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            if (!IsMeatCleaver(__instance)) return;

            string meatCleaverId = __instance.modData[MeatCleaverKey];

            if (!__instance.IsEfficient)
            {
                who.Stamina -= ((float)4f - (float)who.FarmingLevel * 0.2f);
            }
            if (!Animals.ContainsKey(meatCleaverId))
            {
                return;
            }
            FarmAnimal farmAnimal = Animals[meatCleaverId];
            if (farmAnimal == null
                || !MeatController.CanGetMeatFrom(Animals[meatCleaverId])
                || farmAnimal.isBaby())
            {
                return;
            }

            farmAnimal.RemoveOrHitAnimal();

            int numClouds = farmAnimal.Sprite.SourceRect.Width / 2;
            int cloudSprite = !DataLoader.ModConfig.Softmode ? 5 : 10;
            for (int i = 0; i < numClouds; i++)
            {
                int nonRedness = Game1.random.Next(0, 80);
                Color cloudColor = new Color(255, 255 - nonRedness, 255 - nonRedness);

                location.temporarySprites.Add(
                    new TemporaryAnimatedSprite
                    (
                        cloudSprite
                        , farmAnimal.Position + new Vector2(Game1.random.Next(-Game1.tileSize / 2, farmAnimal.Sprite.SourceRect.Width * 3), Game1.random.Next(-Game1.tileSize / 2, farmAnimal.Sprite.SourceRect.Height * 3))
                        , cloudColor
                        , 8
                        , false,
                        Game1.random.NextDouble() < .5 ? 50 : Game1.random.Next(30, 200), 0, Game1.tileSize
                        , -1
                        , Game1.tileSize, Game1.random.NextDouble() < .5 ? 0 : Game1.random.Next(0, 600)
                    )
                    {
                        scale = Game1.random.Next(2, 5) * .25f,
                        alpha = Game1.random.Next(2, 5) * .25f,
                        motion = new Vector2(0, (float) -Game1.random.NextDouble())
                    }
                );
            }

            if (!DataLoader.ModConfig.Softermode)
            {
                location.temporarySprites.Add(
                    new TemporaryAnimatedSprite
                    (
                        farmAnimal.Sprite.textureName.Value
                        , farmAnimal.Sprite.SourceRect
                        , farmAnimal.Position
                        , farmAnimal.FacingDirection == Game1.left
                        , DataLoader.ModConfig.Softmode ? 0.050f : 0.025f 
                        , DataLoader.ModConfig.Softmode ? Color.White : Color.LightPink
                    )
                    {
                        scale = 4f
                    }
                );
            }

            if (!DataLoader.ModConfig.Softmode)
            {
                location.playSound("killAnimal");
            }
            else
            {
                ICue warptSound = Game1.soundBank.GetCue("wand");
                warptSound.SetVariable("Pitch", 1800);
                warptSound.Play();
            }

            List<Item> meatToCreate = MeatController.CreateMeat(farmAnimal);
            if (meatToCreate.Count > 0) 
            {
                MeatController.ThrowItem(meatToCreate, farmAnimal);
                if (DataLoader.ModConfig.Softermode)
                {
                    farmAnimal.ReduceFriendshipFromMeat();
                }
                who.gainExperience(0, DataLoader.ModConfig.Softermode ? 1 : 5);
            }
            Animals[meatCleaverId] = (FarmAnimal) null;
            if (!DataLoader.ModConfig.Softermode)
            {
                TempAnimals[meatCleaverId] = (FarmAnimal)null;
            }
            return;
        }

        private static bool IsMeatCleaver(Item tool)
        {
            return tool?.modData?.ContainsKey(MeatCleaverKey) ?? false;
        }
    }
}
