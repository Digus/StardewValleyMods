using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProducerFrameworkMod.ContentPack;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod.Api
{
    internal class ProducerFrameworkModApi : IProducerFrameworkModApi
    {
        public List<Dictionary<string, object>> GetRecipes()
        {
            List<ProducerRule> producerRules = ProducerController.GetProducerRules();
            return GetRecipes(producerRules);
        }

        public List<Dictionary<string, object>> GetRecipes(string producerName)
        {
            List<ProducerRule> producerRules = ProducerController.GetProducerRules(producerName);
            return GetRecipes(producerRules);
        }

        private static List<Dictionary<string, object>> GetRecipes(List<ProducerRule> producerRules)
        {
            List<Dictionary<string, object>> returnValue = new List<Dictionary<string, object>>();
            foreach (ProducerRule producerRule in producerRules)
            {
                Dictionary<string, object> ruleMap = new Dictionary<string, object>();
                List<Dictionary<string, object>> ingredients = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>()
                    {
                        {"ID", producerRule.InputKey},
                        {"Count", producerRule.InputStack}
                    }
                };
                producerRule.FuelList.ForEach(f =>
                    ingredients.Add(new Dictionary<string, object>() {{"ID", f.Item1}, {"Count", f.Item2}}));
                ruleMap["Ingredients"] = ingredients;

                List<Dictionary<string, object>> exceptIngredients = new List<Dictionary<string, object>>();
                producerRule.ExcludeIdentifiers.ForEach(
                    i => exceptIngredients.Add(new Dictionary<string, object>() {{"ID", i}}));
                ruleMap["ExceptIngredients"] = exceptIngredients;

                double probabilities = 0;
                foreach (OutputConfig outputConfig in producerRule.OutputConfigs)
                {
                    Dictionary<string, object> outputRuleMap = new Dictionary<string, object>(ruleMap);
                    outputRuleMap["Output"] = outputConfig.OutputIndex;
                    outputRuleMap["MinOutput"] = new int[]
                    {
                        outputConfig.OutputStack, outputConfig.SilverQualityInput.OutputStack,
                        outputConfig.GoldQualityInput.OutputStack, outputConfig.IridiumQualityInput.OutputStack
                    }.Min();
                    outputRuleMap["MaxOutput"] = new int[]
                    {
                        outputConfig.OutputMaxStack, outputConfig.SilverQualityInput.OutputMaxStack,
                        outputConfig.GoldQualityInput.OutputMaxStack, outputConfig.IridiumQualityInput.OutputMaxStack
                    }.Max();
                    outputRuleMap["OutputChance"] = outputConfig.OutputProbability;
                    probabilities += outputConfig.OutputProbability;

                    //PFM properties.
                    outputRuleMap["OutputIdentifier"] = outputConfig.OutputIdentifier;
                    outputRuleMap["KeepInputQuality"] = outputConfig.KeepInputQuality;
                    outputRuleMap["OutputQuality"] = outputConfig.OutputQuality;
                    outputRuleMap["InputPriceBased"] = outputConfig.InputPriceBased;
                    outputRuleMap["OutputPriceIncrement"] = outputConfig.OutputPriceIncrement;
                    outputRuleMap["OutputPriceMultiplier"] = outputConfig.OutputPriceMultiplier;
                    outputRuleMap["OutputName"] = outputConfig.OutputName;
                    outputRuleMap["OutputTranslationKey"] = outputConfig.OutputTranslationKey;
                    outputRuleMap["PreserveType"] = outputConfig.PreserveType;

                    returnValue.Add(outputRuleMap);
                }

                var outputConfigs = returnValue.FindAll(r => (double) r["OutputChance"] <= 0);
                double increment = (1 - probabilities) / outputConfigs.Count;
                returnValue.FindAll(r => (double) r["OutputChance"] <= 0).ForEach(r => r["OutputChance"] = increment);
            }

            return returnValue;
        }

        public List<Dictionary<string, object>> GetRecipes(Object producerObject)
        {
            return GetRecipes(producerObject.Name);
        }

        public List<ProducerRule> GetProducerRules(string producerName)
        {
            return ProducerController.GetProducerRules(producerName);
        }

        public List<ProducerRule> GetProducerRules(Object producerObject)
        {
            return GetProducerRules(producerObject.Name);
        }
    }
}
