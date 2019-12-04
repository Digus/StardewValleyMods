using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProducerFrameworkMod.ContentPack;
using StardewModdingAPI;

namespace ProducerFrameworkMod
{
    internal class ProducerController
    {
        private static readonly Dictionary<Tuple<string, int>, ProducerItem> Repository = new Dictionary<Tuple<string, int>, ProducerItem>();

        public static void AddProducerItems(List<ProducerItem> producerItems)
        {
            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
            foreach (var p in producerItems)
            {
                if (!Int32.TryParse(p.InputIdentifier, out int intInputIdentifier))
                {
                    KeyValuePair<int, string> pair = objects.FirstOrDefault(o => o.Value.StartsWith(p.InputIdentifier + "/"));
                    if (pair.Value != null)
                    {
                        intInputIdentifier = pair.Key;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (!Int32.TryParse(p.OutputIdentifier, out int OutputIndex))
                {
                    KeyValuePair<int, string> pair = objects.FirstOrDefault(o => o.Value.StartsWith(p.OutputIdentifier + "/"));
                    if (pair.Value != null)
                    {
                        OutputIndex = pair.Key;
                    }
                    else
                    {
                        continue;
                    }
                }
                p.OutputIndex = OutputIndex;
                Repository[new Tuple<string, int>(p.ProducerName, intInputIdentifier)] = p;
            }
        }

        public static ProducerItem GetItem(string ProducerName, int InputIndex, int InputCategory)
        {
            ProducerItem value;
            Repository.TryGetValue(new Tuple<string, int>(ProducerName, InputIndex), out value);
            if (value == null)
            {
                Repository.TryGetValue(new Tuple<string, int>(ProducerName, InputCategory), out value);
            }
            return value;
        }
    }
}
