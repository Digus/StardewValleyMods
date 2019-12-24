using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using ProducerFrameworkMod.ContentPack;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Network;
using Enumerable = System.Linq.Enumerable;
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
                if (__instance.heldObject.Value != null && !__instance.name.Equals("Crystalarium") || (bool)((NetFieldBase<bool, NetBool>)input.bigCraftable))
                {
                    return true;
                }
                if (IsInputExcluded(input, producerRule))
                {
                    return true;
                }

                if ((bool)((NetFieldBase<bool, NetBool>)__instance.bigCraftable) && !probe && (__instance.heldObject.Value == null))
                {
                    __instance.scale.X = 5f;
                }

                bool shouldDisplayMessages = !probe && who.IsLocalPlayer;

                if (IsStackLessThanRequired(input, producerRule.InputStack, shouldDisplayMessages))
                {
                    return false;
                }

                foreach (var fuel in producerRule.FuelList)
                {
                    if (IsFuelStackLessThanRequired(fuel, shouldDisplayMessages, who))
                    {
                        return false;
                    }
                }

                Vector2 tileLocation = __instance.tileLocation.Value;
                Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed * 10000000 + Game1.timeOfDay *10000 + (int)tileLocation.X * 200 + (int)tileLocation.Y);
                OutputConfig outputConfig = ChooseOutput(producerRule.OutputConfigs, random);

                string outputName = null;

                Object output = new Object(Vector2.Zero, outputConfig.OutputIndex, (string) null, false, true, false, false);

                if (outputConfig.InputPriceBased)
                {
                    output.Price = (int)(outputConfig.OutputPriceIncrement + input.Price * outputConfig.OutputPriceMultiplier);
                }

                output.Quality = outputConfig.KeepInputQuality ? input.Quality : outputConfig.OutputQuality;

                output.Stack = GetOutputStack(outputConfig, input, random);

                __instance.heldObject.Value = output;

                if (!probe)
                {
                    bool inputUsed = false;
                    if (outputConfig.PreserveType.HasValue)
                    {
                        outputName = ObjectUtils.GetPreserveName(outputConfig.PreserveType.Value, input.Name);
                        __instance.heldObject.Value.preserve.Value = outputConfig.PreserveType;
                        inputUsed = true;
                    }
                    else if (outputConfig.OutputName != null)
                    {
                        outputName = outputConfig.OutputName
                            .Replace("{inputName}", input.Name)
                            .Replace("{outputName}", output.Name)
                            .Replace("{farmerName}", who.Name)
                            .Replace("{farmName}", who.farmName.Value);
                        inputUsed = outputConfig.OutputName.Contains("{inputName}");
                    }
                    if (outputName != null)
                    {
                        __instance.heldObject.Value.Name = outputName;
                    }
                    if (inputUsed)
                    {
                        __instance.heldObject.Value.preservedParentSheetIndex.Value = input.parentSheetIndex;
                    }

                    //Called just to load the display name.
                    string loadingDisplayName = output.DisplayName;

                    GameLocation currentLocation = who.currentLocation;
                    PlaySound(producerRule.Sounds, currentLocation);

                    __instance.minutesUntilReady.Value = producerRule.MinutesUntilReady;

                    if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig)
                    {
                        __instance.showNextIndex.Value = producerConfig.AlternateFrameProducing;
                    }

                    if (producerRule.PlacingAnimation.HasValue)
                    {
                        AnimationController.DisplayAnimation(producerRule.PlacingAnimation.Value, producerRule.PlacingAnimationColor, currentLocation, tileLocation);
                    }

                    __instance.initializeLightSource(tileLocation, false);

                    foreach (var fuel in producerRule.FuelList)
                    {
                        RemoveItemsFromInventory(who, fuel.Item1, fuel.Item2);
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

        private static void PlaySound(List<string> soundList, GameLocation currentLocation)
        {
            soundList.ForEach(s =>
            {
                try
                {
                    currentLocation.playSound(s, NetAudio.SoundContext.Default);
                }
                catch (Exception e)
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Error trying to play sound '{s}'.");
                }
            });
        }

        private static bool IsInputExcluded(Object input, ProducerRule producerRule)
        {
            return producerRule.ExcludeIdentifiers != null && (producerRule.ExcludeIdentifiers.Contains(input.ParentSheetIndex.ToString())
                                                               || producerRule.ExcludeIdentifiers.Contains(input.Name)
                                                               || producerRule.ExcludeIdentifiers.Contains(input.Category.ToString())
                                                               || producerRule.ExcludeIdentifiers.Intersect(input.GetContextTags()).Any());
        }

        private static bool IsStackLessThanRequired(Object object1, int requiredStack, bool shouldDisplayMessages)
        {
            if (object1.Stack < requiredStack)
            {
                if (shouldDisplayMessages)
                {
                    Game1.showRedMessage(DataLoader.Helper.Translation.Get(
                        "Message.Requirement.Amount"
                        , new { amount = requiredStack, objectName = Lexicon.makePlural(object1.DisplayName, requiredStack == 1)}
                    ));
                }
                return true;
            }
            return false;
        }

        private static bool IsFuelStackLessThanRequired(Tuple<int, int> fuel, bool shouldDisplayMessages, Farmer who)
        {
            if (!who.hasItemInInventory(fuel.Item1, fuel.Item2))
            {
                if (shouldDisplayMessages)
                {
                    if (fuel.Item1 >= 0)
                    {
                        Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
                        var objectName = Lexicon.makePlural(ObjectUtils.GetObjectParameter(objects[fuel.Item1], (int)ObjectParameter.DisplayName), fuel.Item2 == 1);
                        Game1.showRedMessage(DataLoader.Helper.Translation.Get("Message.Requirement.Amount", new { amount = fuel.Item2, objectName }));
                    }
                    else
                    {
                        var objectName = ObjectUtils.GetCategoryName(fuel.Item1);
                        Game1.showRedMessage(DataLoader.Helper.Translation.Get("Message.Requirement.Amount", new { amount = fuel.Item2, objectName }));
                    }
                }
                return true;
            }
            return false;
        }

        private static int GetOutputStack(OutputConfig outputConfig, Object input, Random random)
        {
            double chance = random.NextDouble();
            StackConfig stackConfig;
            if (input.Quality == 4 && chance < outputConfig.IridiumQualityInput.Probability)
            {
                stackConfig = outputConfig.IridiumQualityInput;
            }
            else if (input.Quality == 2 && chance < outputConfig.GoldQualityInput.Probability)
            {
                stackConfig = outputConfig.GoldQualityInput;
            }
            else if (input.Quality == 1 && chance < outputConfig.SilverQualityInput.Probability)
            {
                stackConfig = outputConfig.SilverQualityInput;
            }
            else
            {
                stackConfig = new StackConfig(outputConfig.OutputStack, outputConfig.OutputMaxStack);
            }
            return random.Next(stackConfig.OutputStack, Math.Max(stackConfig.OutputStack, stackConfig.OutputMaxStack));
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

        private static OutputConfig ChooseOutput(List<OutputConfig> producerRuleOutputConfig, Random random)
        {
            List<OutputConfig> outputConfigs = producerRuleOutputConfig.FindAll(o => o.OutputProbability > 0);
            Double chance = random.NextDouble();
            Double probabilies = 0;
            foreach (OutputConfig outputConfig in outputConfigs)
            {
                probabilies += outputConfig.OutputProbability;
                if (chance - probabilies < 0)
                {
                    return outputConfig;
                }
            }
            outputConfigs = producerRuleOutputConfig.FindAll(o => o.OutputProbability <= 0);
            double increment = (1 - probabilies) / outputConfigs.Count;
            foreach (OutputConfig outputConfig in outputConfigs)
            {
                probabilies += increment;
                if (chance - probabilies < 0)
                {
                    return outputConfig;
                }
            }
            return producerRuleOutputConfig.FirstOrDefault();
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
            if (NameUtils.HasCustomNameForIndex(__instance.ParentSheetIndex) && !__instance.preserve.Value.HasValue && __instance.ParentSheetIndex != 463 && __instance.ParentSheetIndex != 464)
            {
                IDictionary<int, string> objects = Game1.objectInformation;
                string translation = NameUtils.GetCustomNameFromIndex(__instance.ParentSheetIndex);
                
                if (objects.TryGetValue(__instance.ParentSheetIndex, out var instanceData) && ObjectUtils.GetObjectParameter(instanceData, (int)ObjectParameter.Name) != __instance.Name)
                {
                    if (translation.Contains("{outputName}"))
                    {
                        translation = translation.Replace("{outputName}",ObjectUtils.GetObjectParameter(instanceData, (int) ObjectParameter.DisplayName));
                    }
                }
                else
                {
                    __result = __instance.Name;
                    return false;
                }
                
                if (translation.Contains("{inputName}"))
                {
                    if (objects.TryGetValue(__instance.preservedParentSheetIndex.Value, out var preservedData))
                    {
                        translation = translation.Replace("{inputName}", ObjectUtils.GetObjectParameter(preservedData,(int)ObjectParameter.DisplayName));
                    }
                    else
                    {
                        __result = __instance.Name;
                        return false;
                    }
                }
                if (translation.Contains("{farmName}"))
                {
                    translation = translation.Replace("{farmName}", Game1.player.farmName.Value);
                }
                if (translation.Contains("{farmerName}"))
                {
                    string farmerName = Game1.getAllFarmers().FirstOrDefault(f => __instance.Name.Contains(f.name))?.Name ?? Game1.player.Name;
                    translation = translation.Replace("{farmerName}", farmerName);
                }
                __result = translation;
                return false;
            }
            return true;
        }
    }
}