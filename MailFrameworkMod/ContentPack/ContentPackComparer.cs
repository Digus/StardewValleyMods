using System.Collections.Generic;
using StardewModdingAPI;

namespace MailFrameworkMod.ContentPack
{
    internal class ContentPackComparer : IEqualityComparer<IContentPack>
    {
        public bool Equals(IContentPack x, IContentPack y)
        {
            return y != null && x != null && x.Manifest.UniqueID.Equals(y.Manifest.UniqueID);
        }

        public int GetHashCode(IContentPack obj)
        {
            return obj.Manifest.UniqueID.GetHashCode();
        }
    }
}