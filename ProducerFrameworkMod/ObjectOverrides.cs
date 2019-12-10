using System;
using System.Collections.Generic;
using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using ProducerFrameworkMod.ContentPack;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Network;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod
{

    internal class ObjectOverrides
    {
        [HarmonyPriority(800)]
        internal static bool PerformObjectDropInAction(Object __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
        {
            if (__instance.isTemporarilyInvisible || !(dropInItem is Object))
                return false;
            Object input = (Object) dropInItem;

            if (ProducerController.GetProducerItem(__instance.name, input) is ProducerRule producerRule)
            {
                if (__instance.heldObject.Value != null && !__instance.name.Equals("Recycling Machine") && !__instance.name.Equals("Crystalarium") || (bool)((NetFieldBase<bool, NetBool>)input.bigCraftable))
                {
                    return true;
                }
                if ((bool)((NetFieldBase<bool, NetBool>)__instance.bigCraftable) && !probe && (__instance.heldObject.Value == null))
                    __instance.scale.X = 5f;
                
                if (who.IsLocalPlayer && producerRule.FuelIndex.HasValue && !who.hasItemInInventory(producerRule.FuelIndex.Value, producerRule.FuelStack))
                {
                    if (!probe)
                    {
                        if (producerRule.FuelIndex.Value >= 0)
                        {

                            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
                            var objectName = objects[producerRule.FuelIndex.Value].Split('/')[4];
                            Game1.showRedMessage(DataLoader.Helper.Translation.Get("Message.Requirement.Amount", new { amount = producerRule.FuelStack, objectName }));
                        }
                        else
                        {
                            Game1.showRedMessage(DataLoader.Helper.Translation.Get("Message.Requirement.Amount", new { amount = producerRule.FuelStack, objectName = producerRule.FuelIndex.Value }));
                        }
                    }
                    return false;
                }

                if ((int)((NetFieldBase<int, NetInt>)input.stack) < producerRule.InputStack)
                {
                    if (!probe && who.IsLocalPlayer)
                    {
                        Game1.showRedMessage(DataLoader.Helper.Translation.Get("Message.Requirement.Amount", new { amount = producerRule.InputStack, objectName = Lexicon.makePlural(input.DisplayName, producerRule.InputStack == 1 ) }));
                    }
                    return false;
                }

                string outputName = null;
                if (producerRule.PreserveType.HasValue)
                {
                    outputName = GetPreserveName(producerRule.PreserveType.Value, input.Name);
                }
                else if (producerRule.OutputName != null)
                {
                    if (!producerRule.CompoundOutputName.HasValue)
                    {
                        outputName = producerRule.OutputName;
                    }
                    else
                    {
                        outputName = producerRule.CompoundOutputName == AffixType.Prefix
                            ? producerRule.OutputName + input.DisplayName
                            : input.DisplayName + producerRule.OutputName;
                    }
                }

                Object output = new Object(Vector2.Zero, producerRule.OutputIndex, outputName, false, true, false, false);

                if (producerRule.InputPriceBased)
                {
                    output.Price = (int) (producerRule.OutputPriceIncrement + input.Price * producerRule.OutputPriceMultiplier);
                }

                output.Quality = producerRule.KeepInputQuality ? input.Quality : producerRule.OutputQuality ?? 0;
                output.Stack = producerRule.OutputStack;

                __instance.heldObject.Value = output;

                if (!probe)
                {
                    if (outputName != null)
                    {
                        __instance.heldObject.Value.name = outputName;
                    }
                    __instance.heldObject.Value.preserve.Value = producerRule.PreserveType;
                    if (producerRule.CompoundOutputName.HasValue || producerRule.PreserveType.HasValue)
                    {
                        __instance.heldObject.Value.preservedParentSheetIndex.Value = input.parentSheetIndex;
                    }
                    string loadingDisplayName =__instance.DisplayName;

                    producerRule.Sounds.ForEach(s=>
                    {
                        try
                        {
                            who.currentLocation.playSound(s, NetAudio.SoundContext.Default);
                        }
                        catch (Exception e)
                        {
                            ProducerFrameworkModEntry.ModMonitor.Log($"Error trying to play sound '{s}'.");
                        }
                    });
                    __instance.minutesUntilReady.Value = producerRule.MinutesUntilReady;

                    if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig)
                    {
                        __instance.showNextIndex.Value = producerConfig.AlternateFrameProducing;
                    }

                    Multiplayer multiplayer = DataLoader.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

                    switch (producerRule.PlacingAnimation)
                    {
                        case PlacingAnimation.Bubbles:
                            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[1]
                            {
                                new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, __instance.tileLocation.Value * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) __instance.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, producerRule.PlacingAnimationColor * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                                {
                                    alphaFade = 0.005f
                                }
                            });
                            break;
                        case PlacingAnimation.Fire:
                            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[1]
                            {
                                new TemporaryAnimatedSprite(30, __instance.tileLocation.Value * 64f + new Vector2(0.0f, -16f), producerRule.PlacingAnimationColor, 4, false, 50f, 10, 64, (float) (((double) __instance.tileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), -1, 0)
                                {
                                    alphaFade = 0.005f
                                }
                            });
                            break;
                    }

                    __instance.initializeLightSource((Vector2)((NetFieldBase<Vector2, NetVector2>)__instance.tileLocation), false);

                    if (producerRule.FuelIndex.HasValue)
                    {
                        RemoveItemsFromInventory(who, producerRule.FuelIndex.Value, producerRule.FuelStack);
                    }

                    input.Stack -= producerRule.InputStack;
                    __result = input.Stack <= 0;
                }
                else
                {
                    __result = true;
                }
                return false;
            }

            return true;
        }

        internal static void checkForAction(Object __instance, bool justCheckingForActivity, bool __result)
        {
            if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig && __instance.heldObject.Value == null && __instance.MinutesUntilReady <= 0)
            {
                __instance.showNextIndex.Value = false;
            }
        }

        internal static void minutesElapsed(Object __instance)
        {
            if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig && __instance.heldObject.Value != null && __instance.MinutesUntilReady <= 0)
            {
                __instance.showNextIndex.Value = producerConfig.AlternateFrameWhenReady;
            }
        }

        internal static bool LoadDisplayName(Object __instance, ref string __result)
        {
            if (__instance.preserve.Value == null && __instance.preservedParentSheetIndex.Value > 0 && __instance.ParentSheetIndex != 463 && __instance.ParentSheetIndex != 464)
            {
                IDictionary<int, string> objects = Game1.objectInformation;
                objects.TryGetValue(__instance.preservedParentSheetIndex.Value, out var preservedData);
                objects.TryGetValue(__instance.ParentSheetIndex, out var instanceData);
                if (!string.IsNullOrEmpty(preservedData) && !string.IsNullOrEmpty(instanceData))
                {
                    string preservedName = preservedData.Split('/')[0];
                    string preservedDisplayName = preservedData.Split('/')[4];

                    string instanceName = instanceData.Split('/')[0];
                    string instanceDisplayName = instanceData.Split('/')[4];

                    __result = __instance.Name
                        .Replace(preservedName, preservedDisplayName)
                        .Replace(instanceName, instanceDisplayName);
                    
                    return false;
                }
            }
            return true;
        }

        private static string GetPreserveName(Object.PreserveType preserveType, string preserveParentName)
        {
            switch (preserveType)
            {
                case Object.PreserveType.Wine:
                    return $"{preserveParentName} Wine";
                case Object.PreserveType.Jelly:
                    return $"{preserveParentName} Jelly";
                case Object.PreserveType.Pickle:
                    return $"Pickled {preserveParentName}";
                case Object.PreserveType.Juice:
                    return $"{preserveParentName} Juice";
                case Object.PreserveType.Roe:
                    return $"{preserveParentName} Roe";
                case Object.PreserveType.AgedRoe:
                    return $"Aged {preserveParentName}";
            }
            return null;
        }

        private static bool RemoveItemsFromInventory(Farmer farmer, int index, int stack)
        {
            if (farmer.hasItemInInventory(index, stack, 0))
            {
                for (int index1 = 0; index1 < farmer.items.Count; ++index1)
                {
                    if (farmer.items[index1] != null && farmer.items[index1] is Object object1 && (object1.ParentSheetIndex == index || object1.Category == index))
                    {
                        if (farmer.items[index1].Stack > stack)
                        {
                            farmer.items[index1].Stack -= stack;
                            return true;
                        }
                        stack -= farmer.items[index1].Stack;
                        farmer.items[index1] = (Item)null;
                    }
                    if (stack <= 0)
                        return true;
                }
            }
            return false;
        }
    }
}