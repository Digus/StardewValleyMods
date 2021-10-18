using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace ProducerFrameworkMod.Utils
{
    internal class DgaUtils
    {
        public static int DgaIndex = 1720;

        public static bool GetOriginalDgaItem(SObject input, out SObject obj)
        {
            obj = input.ParentSheetIndex == DgaIndex
                ? DataLoader.DgaApi.SpawnDGAItem(DataLoader.DgaApi.GetDGAItemId(input)) as SObject
                : null;
            return obj != null;
        }

        public static int? GetObjectDgaFakeIndex(SObject obj)
        {
            if (obj.ParentSheetIndex == DgaIndex)
            {
                return DataLoader.DgaApi.GetDGAFakeIndex(obj);
            }
            return null;
        }
    }
}
