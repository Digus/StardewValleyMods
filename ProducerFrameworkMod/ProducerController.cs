using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProducerFrameworkMod.ContentPack;
using StardewModdingAPI;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod
{
    internal class ProducerController
    {
        private static readonly Dictionary<Tuple<string, object>, ProducerRule> RulesRepository = new Dictionary<Tuple<string, object>, ProducerRule>();
        private static readonly Dictionary<string, ProducerConfig> ConfigRepository = new Dictionary<string, ProducerConfig>()
        {
            {
                "Furnace", new ProducerConfig("Furnace",true)
            }
        };

        public static void AddProducerItems(List<ProducerRule> producerRules)
        {
            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
            foreach (var producerRule in producerRules)
            {
                object inputIdentifier;
                if (!Int32.TryParse(producerRule.InputIdentifier, out int intInputIdentifier))
                {
                    KeyValuePair<int, string> pair = objects.FirstOrDefault(o => o.Value.StartsWith(producerRule.InputIdentifier + "/"));
                    if (pair.Value != null)
                    {
                        inputIdentifier = pair.Key;
                    }
                    else
                    {
                        inputIdentifier = producerRule.InputIdentifier;
                    }
                }
                else
                {
                    inputIdentifier = intInputIdentifier;
                }
                if (!Int32.TryParse(producerRule.OutputIdentifier, out int outputIndex))
                {
                    KeyValuePair<int, string> pair = objects.FirstOrDefault(o => o.Value.StartsWith(producerRule.OutputIdentifier + "/"));
                    if (pair.Value != null)
                    {
                        outputIndex = pair.Key;
                    }
                    else
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log($"Output not found for producer '{producerRule.ProducerName}' and input '{producerRule.InputIdentifier}'.",LogLevel.Warn);
                        continue;
                    }
                }
                producerRule.OutputIndex = outputIndex;

                if (producerRule.FuelIdentifier != null)
                {
                    if (!Int32.TryParse(producerRule.FuelIdentifier, out int fuelIndex))
                    {
                        KeyValuePair<int, string> pair = objects.FirstOrDefault(o => o.Value.StartsWith(producerRule.FuelIdentifier + "/"));
                        if (pair.Value != null)
                        {
                            fuelIndex = pair.Key;
                        }
                        else
                        {
                            ProducerFrameworkModEntry.ModMonitor.Log($"Fuel not found for producer '{producerRule.ProducerName}' and input '{producerRule.InputIdentifier}'.", LogLevel.Warn);
                            continue;
                        }
                    }
                    producerRule.FuelIndex = fuelIndex;
                }

                if (producerRule.PlacingAnimationColorName != null)
                {
                    try
                    {
                        producerRule.PlacingAnimationColor = DataLoader.Helper.Reflection.GetProperty<Color>(typeof(Color), producerRule.PlacingAnimationColorName).GetValue();
                    }
                    catch (Exception)
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log($"Color '{producerRule.PlacingAnimationColorName}' not valid, will use the default color '{producerRule.PlacingAnimationColor}'.");
                    }
                }

                RulesRepository[new Tuple<string, object>(producerRule.ProducerName, inputIdentifier)] = producerRule;
            }
        }

        public static void AddProducersConfig(List<ProducerConfig> producersConfig)
        {
            producersConfig.ForEach(c => ConfigRepository[c.ProducerName] = c);
        }

        public static ProducerRule GetProducerItem(string producerName, Object input)
        {
            ProducerRule value;
            RulesRepository.TryGetValue(new Tuple<string, object>(producerName, input.ParentSheetIndex), out value);
            if (value == null)
            {
                RulesRepository.TryGetValue(new Tuple<string, object>(producerName, input.Category), out value);
            }

            if (value == null)
            {
                foreach (string tag in input.GetContextTagList())
                {
                    if (RulesRepository.TryGetValue(new Tuple<string, object>(producerName, tag), out value))
                    {
                        break;
                    }
                }
            }
            return value;
        }

        public static ProducerConfig GetProducerConfig(string producerName)
        {
            ConfigRepository.TryGetValue(producerName, out ProducerConfig producerConfig);
            return producerConfig;
        }
    }
}
