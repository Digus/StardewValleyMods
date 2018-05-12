using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.meats
{
    public class MeatPriceOverrides
    {
        public static bool sellToStorePrice(StardewValley.Object __instance, ref int __result)
        {
            if (Game1.player.professions.Contains(0) && __instance.Category == -14)
            {
                float num = (float)(int)((double)__instance.Price * (1.0 + (double)__instance.Quality * 0.25));
                num *= 1.2f;
                __result = (int)num;
                return false;                
            }
            return true;            
        }
    }
}
