using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.meats
{
    [HarmonyPatch]
    public class MeatPriceOverrides
    {
        internal static MethodInfo TargetMethod()
        {
            if (Type.GetType("StardewValley.Object, Stardew Valley") != null)
                return AccessTools.Method(Type.GetType("StardewValley.Object, Stardew Valley"), "sellToStorePrice");
            else
                return AccessTools.Method(Type.GetType("StardewValley.Object, StardewValley"), "sellToStorePrice");
        }
        
        [HarmonyPrefix]
        public static bool sellToStorePrice(StardewValley.Object __instance, ref int __result)
        {
            if (Game1.player.professions.Contains(0) && __instance.category == -14)
            {
                float num = (float)(int)((double)__instance.price * (1.0 + (double)__instance.quality * 0.25));
                num *= 1.2f;
                __result = (int)num;
                return false;                
            }
            return true;            
        }
    }
}
