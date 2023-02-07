using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace MailFrameworkMod.Api
{
    public interface IMailFrameworkModApi
    {
        public void RegisterContentPack(IContentPack contentPack);

        public void RegisterLetter(ILetter iLetter, Func<ILetter, bool> condition, Action<ILetter> callback = null, Func<ILetter, List<Item>> dynamicItems = null);

        public ILetter GetLetter(string id);

        public string GetMailDataString(string id);
    }
}
