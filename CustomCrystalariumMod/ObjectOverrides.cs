
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace CustomCrystalariumMod
{
    internal class ObjectOverrides
    {
        public static bool GetMinutesForCrystalarium(ref int whichGem, ref int __result)
        {
            if (DataLoader.CrystalariumData.ContainsKey(whichGem))
            {
                __result = DataLoader.CrystalariumData[whichGem];
                return false;
            }
            else
            {
                var itemCategory = new Object(whichGem, 1, false).Category;
                if (DataLoader.CrystalariumData.ContainsKey(itemCategory))
                {
                    __result = DataLoader.CrystalariumData[itemCategory];
                    return false;
                }
            }
            return true;
        }

        public static bool PerformObjectDropInAction(ref Object __instance, ref Item dropInItem, ref bool probe, ref Farmer who, ref bool __result)
        {
            if (dropInItem is Object object1)
            {
                if (!(__instance.heldObject.Value != null && !__instance.Name.Equals("Recycling Machine") &&
                      !__instance.Name.Equals("Crystalarium") ||object1 != null && (bool) (object1.bigCraftable.Value)))
                {
                    if (__instance.Name.Equals("Crystalarium"))
                    {
                        if ((__instance.heldObject.Value == null || __instance.heldObject.Value.ParentSheetIndex != object1.ParentSheetIndex) 
                             && (__instance.heldObject.Value == null || __instance.MinutesUntilReady > 0))
                        { 
                            int minutesUntilReady;
                            if (DataLoader.CrystalariumData.ContainsKey(object1.ParentSheetIndex))
                            {
                                minutesUntilReady = DataLoader.CrystalariumData[object1.ParentSheetIndex];
                            }
                            else if (DataLoader.CrystalariumData.ContainsKey(object1.Category))
                            {
                                minutesUntilReady = DataLoader.CrystalariumData[object1.Category];
                            }
                            else
                            {
                                return true;
                            }
                            if ((bool)__instance.bigCraftable.Value && !probe &&
                                (object1 != null && __instance.heldObject.Value == null))
                            {
                                __instance.scale.X = 5f;
                            }
                            __instance.heldObject.Value = (Object)object1.getOne();
                            if (!probe)
                            {
                                who.currentLocation.playSound("select");
                                __instance.MinutesUntilReady = minutesUntilReady;
                            }
                            __result = true;
                            return false;
                        }
                    }

                }
            }

            return true;
        }
    }
}
