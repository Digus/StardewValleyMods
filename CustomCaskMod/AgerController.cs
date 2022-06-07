using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using SObject = StardewValley.Object;

namespace CustomCaskMod
{
    public class AgerController
    {
        private static readonly Dictionary<string, CustomAger> AgerData = new Dictionary<string, CustomAger>();

        public static CustomAger GetAger(string name)
        {
            AgerData.TryGetValue(name, out CustomAger result);
            return result;
        }

        public static bool HasAger(string name)
        {
            return AgerData.ContainsKey(name);
        }

        public static void SetAger(CustomAger ager)
        {
            AgerData[ager.Name] = ager;
        }

        public static float? GetAgingMultiplierForItem(CustomAger customAger, Item ageable)
        {
            if (customAger.AgingDataId.ContainsKey(ageable.ParentSheetIndex))
            {
                return customAger.AgingDataId[ageable.ParentSheetIndex];
            }
            else if (customAger.AgingDataId.ContainsKey(ageable.Category))
            {
                return customAger.AgingDataId[ageable.Category];
            }
            return null;
        }

        public static void ClearAgers()
        {
            AgerData.Clear();
        }
    }
}
