using HarmonyLib;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.common;
using AnimalHusbandryMod.recipes;
using DataLoader = AnimalHusbandryMod.common.DataLoader;
using SObject = StardewValley.Object;

namespace AnimalHusbandryMod.meats
{
    public class MeatOverrides
    {
        public static bool sellToStorePrice(SObject __instance, ref int __result)
        {
            if (!DataLoader.ModConfig.DisableRancherMeatPriceAjust && Game1.player.professions.Contains(0) && __instance.Category == -14)
            {
                float num = (float)(int)((double)__instance.Price * (1.0 + (double)__instance.Quality * 0.25));
                num *= 1.2f;
                if (num > 0f)
                {
                    num = Math.Max(1f, num * Game1.MasterPlayer.difficultyModifier);
                }
                __result = (int)num;
                return false;                
            }
            return true;            
        }

        public static bool isPotentialBasicShipped(ref int category, ref bool __result)
        {
            if  (category == -14)
            {
                __result = true;
                return false;
            }

            return true;
        }

        public static bool countsForShippedCollection(SObject __instance, ref bool __result)
        {
            if (__instance.Category == -14)
            {
                __result = true;
                return false;
            }

            return true;
        }

        public static void ReadBook(SObject __instance)
        {
            if (__instance.ItemId == "Book_QueenOfSauce")
            {
                RecipesLoader.AddAllMeatRecipes();
            }
        }
    }
}
