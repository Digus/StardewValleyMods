using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;

namespace WaterRetainingFieldMod.Integrations
{
    public interface IMailFrameworkModApi
    {
        public void RegisterContentPack(IContentPack contentPack);

        public void RegisterLetter(ILetter iLetter, Func<ILetter, bool> condition, Action<ILetter> callback = null, Func<ILetter, List<Item>> dynamicItems = null);
    }
}
