using System;
using Microsoft.Xna.Framework;
using Netcode;
using ProducerFrameworkMod.ContentPack;
using StardewValley;
using StardewValley.Network;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod
{

    internal class ObjectOverrides
    {
        internal static bool PerformObjectDropInAction(Object __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
        {
            if (__instance.isTemporarilyInvisible || !(dropInItem is Object))
                return false;
            Object object1 = (Object) dropInItem;
            if (__instance.heldObject.Value != null && !__instance.name.Equals("Recycling Machine") && !__instance.name.Equals("Crystalarium") || (bool)((NetFieldBase<bool, NetBool>)object1.bigCraftable))
                return false;
            if ((bool)((NetFieldBase<bool, NetBool>)__instance.bigCraftable) && !probe && (__instance.heldObject.Value == null))
                __instance.scale.X = 5f;

            if (ProducerController.GetItem(__instance.name, object1.ParentSheetIndex, object1.Category) is ProducerItem producerItem)
            {
                Object output = new Object(Vector2.Zero, producerItem.OutputIndex
                    , producerItem.CompoundOutputName == null ? producerItem.OutputName
                    : producerItem.CompoundOutputName == AffixType.Prefix ? producerItem.OutputName + object1.DisplayName
                    : object1.DisplayName + producerItem.OutputName
                    , false, true, false, false);
                

                if (producerItem.InputPriceBased)
                {
                    output.Price = (int) (producerItem.OutputPriceIncrement + object1.Price * producerItem.OutputPriceMultiplier);
                }

                output.Quality = producerItem.OutputQuality??0;
                output.Stack = producerItem.OutputStack;

                __instance.heldObject.Value = output;

                if (!probe)
                {
                    if (producerItem.OutputName != null)
                    {
                        __instance.heldObject.Value.name = producerItem.CompoundOutputName == null
                            ? producerItem.OutputName
                            : producerItem.CompoundOutputName == AffixType.Prefix
                                ? producerItem.OutputName + object1.DisplayName
                                : object1.DisplayName + producerItem.OutputName;
                    }


                    producerItem.Sounds.ForEach(s=> who.currentLocation.playSound(s, NetAudio.SoundContext.Default));
                    __instance.minutesUntilReady.Value = producerItem.MinutesUntilReady;
                    Multiplayer multiplayer = DataLoader.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                    switch (producerItem.PlacingAnimation)
                    {
                        case PlacingAnimation.Boobles:
                            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[1]
                            {
                                new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, __instance.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) __instance.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                                {
                                    alphaFade = 0.005f
                                }
                            });
                            break;
                        case PlacingAnimation.Smoke:
                            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[1]
                            {
                                new TemporaryAnimatedSprite(30, __instance.tileLocation.Value * 64f + new Vector2(0.0f, -16f), Color.White, 4, false, 50f, 10, 64, (float) (((double) __instance.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), -1, 0)
                                {
                                    alphaFade = 0.005f
                                }
                            });
                            break;
                    }
                }
                __result = true;
                return false;
            }

            return true;
        }
    }
}