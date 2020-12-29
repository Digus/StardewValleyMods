﻿using ProducerFrameworkMod.ContentPack;
using ProducerFrameworkMod.Controllers;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StardewModdingAPI.Events;

namespace ProducerFrameworkMod
{
    internal class DataLoader
    {
        public const string ProducerRulesJson = "ProducerRules.json";
        public const string ProducersConfigJson = "ProducersConfig.json";
        public static IModHelper Helper;

        public DataLoader(IModHelper helper)
        {
            Helper = helper;
        }

        public static void LoadContentPacks(object sender, EventArgs e)
        {
            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                string producersConfigJson = GetActualCaseForFileName(contentPack.DirectoryPath, ProducersConfigJson);
                bool haveProducersConfigFile = producersConfigJson != null;
                ProducerFrameworkModEntry.ModMonitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                if (haveProducersConfigFile)
                {
                    List<ProducerConfig> producersConfigs = contentPack.ReadJsonFile<List<ProducerConfig>>(producersConfigJson);
                    ProducerController.AddProducersConfig(producersConfigs, contentPack.Manifest.UniqueID);
                }
                else
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have an {ProducersConfigJson} file.", LogLevel.Trace);
                }

                if (e is SaveLoadedEventArgs)
                {
                    string producerRulesJson = GetActualCaseForFileName(contentPack.DirectoryPath, ProducerRulesJson);
                    bool haveProducerRulesFile = producerRulesJson != null;
                    if (haveProducerRulesFile)
                    {
                        List<ProducerRule> producerItems = contentPack.ReadJsonFile<List<ProducerRule>>(producerRulesJson);
                        ProducerController.AddProducerItems(producerItems, contentPack.Translation);
                    }
                    else
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log($"Content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have an {ProducerRulesJson} file.", LogLevel.Trace);
                    }

                    if (!haveProducerRulesFile && !haveProducersConfigFile)
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log($"Ignoring content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have any of the required files.", LogLevel.Warn);
                    }
                }
            }
        }

        /// <summary>
        /// Return the file name using the case of an the first file with that name in the directory.
        /// </summary>
        /// <param name="directory">Directory to look for the file</param>
        /// <param name="pattern">The name of the file in any case to look for</param>
        /// <returns></returns>
        private static string GetActualCaseForFileName(string directory, string pattern)
        {
            IEnumerable<string> foundFiles = Directory.EnumerateFiles(directory, pattern);
            string file = foundFiles.FirstOrDefault();
            return file != null ? new FileInfo(file).Name : null;
        }
    }
}
