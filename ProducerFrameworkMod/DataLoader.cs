using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProducerFrameworkMod.ContentPack;
using StardewModdingAPI;

namespace ProducerFrameworkMod
{
    internal class DataLoader
    {
        public const string ProducersJson = "Producers.json";
        public static IModHelper Helper;

        public DataLoader(IModHelper helper)
        {
            Helper = helper;
        }

        public static void LoadContentPacks(object sender, EventArgs e)
        {
            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                if (File.Exists(Path.Combine(contentPack.DirectoryPath, ProducersJson)))
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                    List<ProducerItem> producerItems = contentPack.ReadJsonFile<List<ProducerItem>>(ProducersJson);
                    ProducerController.AddProducerItems(producerItems);
                }
                else
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Ignoring content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have an {ProducersJson} file.", LogLevel.Warn);
                }
            }
        }
    }
}
