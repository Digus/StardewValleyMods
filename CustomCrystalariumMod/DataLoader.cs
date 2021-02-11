using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailFrameworkMod;
using StardewModdingAPI;
using StardewValley;
using ModConfig = CustomCrystalariumMod.ModConfig;

namespace CustomCrystalariumMod
{
    internal class DataLoader
    {
        public static IModHelper Helper;
        public static ITranslationHelper I18N;
        public static ModConfig ModConfig;
        internal static Dictionary<int,int> CrystalariumDataId = new Dictionary<int, int>();
        internal static Dictionary<string, CustomCloner> ClonerData =  new Dictionary<string, CustomCloner>();

        public const string ClonersDataJson = "ClonersData.json";

        public DataLoader(IModHelper helper)
        {
            Helper = helper;
            I18N = helper.Translation;
            ModConfig = helper.ReadConfig<ModConfig>();

            Dictionary<object, int>  CrystalariumData = DataLoader.Helper.Data.ReadJsonFile<Dictionary<object, int>>("data\\CrystalariumData.json") ?? new Dictionary<object, int>(){{74, 20160}};
            DataLoader.Helper.Data.WriteJsonFile("data\\CrystalariumData.json", CrystalariumData);

            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
            CrystalariumData.ToList().ForEach(d =>
            {
                int? id = GetId(d.Key, objects);
                if (id.HasValue && !CrystalariumDataId.ContainsKey(id.Value)) CrystalariumDataId[id.Value] = d.Value;                
            });

            DataLoader.LoadContentPacksCommand();

            if (!ModConfig.DisableLetter)
            {
                MailDao.SaveLetter
                (
                    new Letter
                    (
                        "CustomCrystalarium"
                        , I18N.Get("CustomCrystalarium.Letter")
                        , (l) => !Game1.player.mailReceived.Contains(l.Id)
                        , (l) => Game1.player.mailReceived.Add(l.Id)
                    )
                    {
                        Title = I18N.Get("CustomCrystalarium.Letter.Title")
                    }
                );
            }
        }

        public static void LoadContentPacksCommand(string command = null, string[] args = null)
        {
            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);            

            foreach (IContentPack contentPack in Helper.ContentPacks.GetOwned())
            {
                if (File.Exists(Path.Combine(contentPack.DirectoryPath, ClonersDataJson)))
                {
                    CustomCrystalariumModEntry.ModMonitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                    List<CustomCloner> clonerData = contentPack.ReadJsonFile<List<CustomCloner>>(ClonersDataJson);
                    foreach (var cloner in clonerData)
                    {
                        if (String.IsNullOrEmpty(cloner.Name))
                        {
                            CustomCrystalariumModEntry.ModMonitor.Log($"The cloner name property can't be null or empty. This cloner will be ignored.", LogLevel.Warn);
                        }
                        if (cloner.Name == "Crystalarium")
                        {
                            cloner.CloningData.ToList().ForEach(d =>
                            {
                                int? id = GetId(d.Key, objects);
                                if (id.HasValue && !CrystalariumDataId.ContainsKey(id.Value))
                                {
                                    CrystalariumDataId[id.Value] = d.Value;
                                    CustomCrystalariumModEntry.ModMonitor.Log($"Adding crystalarium data for item '{d.Key}' from mod '{contentPack.Manifest.UniqueID}'.", LogLevel.Trace);
                                }
                            });
                        }
                        else
                        {
                            cloner.ModUniqueID = contentPack.Manifest.UniqueID;
                            if (ClonerData.ContainsKey(cloner.Name))
                            {
                                if (ClonerData[cloner.Name].ModUniqueID != cloner.ModUniqueID)
                                {
                                    CustomCrystalariumModEntry.ModMonitor.Log($"Both mods '{ClonerData[cloner.Name].ModUniqueID}' and '{cloner.ModUniqueID}' have data for  '{cloner.Name}'. You should report the problem to these mod's authors. Data from mod '{ClonerData[cloner.Name].ModUniqueID}' will be used.", LogLevel.Warn);
                                    continue;
                                }
                            }
                            cloner.CloningData.ToList().ForEach(d => 
                            {
                                int? id = GetId(d.Key, objects);
                                if (id.HasValue) cloner.CloningDataId[id.Value] = d.Value;
                            });
                            ClonerData[cloner.Name] = cloner;
                        }
                    }
                }
                else
                {
                    CustomCrystalariumModEntry.ModMonitor.Log($"Ignoring content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}\nIt does not have an CaskData.json file.", LogLevel.Warn);
                }
            }
        }

        private static int? GetId(object identifier, Dictionary<int, string> objects)
        {
            if (Int32.TryParse(identifier.ToString(), out int id))
            {
                return id;
            }
            else
            {
                KeyValuePair<int, string> pair = objects.FirstOrDefault(o => o.Value.StartsWith(identifier + "/"));
                if (pair.Value != null)
                {
                    return pair.Key;
                }
            }
            return null;
        }
    }
}
