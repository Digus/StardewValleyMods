using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace CropTransplantMod
{
    public class CropTransplantModEntry : Mod
    {
        public static IMonitor ModMonitor;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            new DataLoader(helper);

            var harmony = HarmonyInstance.Create("Digus.CustomCrystalariumMod");

            var utilityTryToPlaceItem = typeof(Utility).GetMethod("tryToPlaceItem");
            var objectOverridesTryToPlaceItem = typeof(ObjectOverrides).GetMethod("TryToPlaceItem");
            harmony.Patch(utilityTryToPlaceItem, new HarmonyMethod(objectOverridesTryToPlaceItem), null);

            var utilityCanGrabSomethingFromHere = typeof(Utility).GetMethod("canGrabSomethingFromHere");
            var objectOverridesCanGrabSomethingFromHere = typeof(ObjectOverrides).GetMethod("CanGrabSomethingFromHere");
            harmony.Patch(utilityCanGrabSomethingFromHere, new HarmonyMethod(objectOverridesCanGrabSomethingFromHere), null);

            var hoeDirtPerformUseAction = typeof(HoeDirt).GetMethod("performUseAction");
            var objectOverridesPerformUseAction = typeof(ObjectOverrides).GetMethod("PerformUseAction");
            harmony.Patch(hoeDirtPerformUseAction, new HarmonyMethod(objectOverridesPerformUseAction), null);

            var game1PressUseToolButton = typeof(Game1).GetMethod("pressUseToolButton");
            var objectOverridesPressUseToolButton = typeof(ObjectOverrides).GetMethod("PressUseToolButton");
            harmony.Patch(game1PressUseToolButton, new HarmonyMethod(objectOverridesPressUseToolButton), null);

            var utilityPlayerCanPlaceItemHere = typeof(Utility).GetMethod("playerCanPlaceItemHere");
            var objectOverridesPlayerCanPlaceItemHere = typeof(ObjectOverrides).GetMethod("PlayerCanPlaceItemHere");
            harmony.Patch(utilityPlayerCanPlaceItemHere, new HarmonyMethod(objectOverridesPlayerCanPlaceItemHere), null);

            SaveEvents.BeforeSave += (x, y) =>
            {
                if (Game1.player.ActiveObject is HeldIndoorPot pot)
                {
                    Game1.player.ActiveObject = ObjectOverrides.RegularPotObject;
                    ObjectOverrides.CurrentHeldIndoorPot = null;
                }
            };

            var temp = typeof(Crop).GetMethod("drawWithOffset");
            var temp2 = typeof(ObjectOverrides).GetMethod("drawWithOffset");
            harmony.Patch(temp, new HarmonyMethod(temp2), null);

        }
    }
}
