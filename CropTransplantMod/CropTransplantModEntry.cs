using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace CropTransplantMod
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class CropTransplantModEntry : Mod
    {
        public static IMonitor ModMonitor;
        public static IModEvents Events;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            Events = helper.Events;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.Saving += OnSaving;
        }

        
        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            new DataLoader(Helper);

            var harmony = HarmonyInstance.Create("Digus.CustomCrystalariumMod");

            var utilityTryToPlaceItem = typeof(Utility).GetMethod("tryToPlaceItem");
            var objectOverridesTryToPlaceItem = typeof(TransplantOverrides).GetMethod("TryToPlaceItem");
            harmony.Patch(utilityTryToPlaceItem, new HarmonyMethod(objectOverridesTryToPlaceItem), null);

            var utilityCanGrabSomethingFromHere = typeof(Utility).GetMethod("canGrabSomethingFromHere");
            var objectOverridesCanGrabSomethingFromHere = typeof(TransplantOverrides).GetMethod("CanGrabSomethingFromHere");
            harmony.Patch(utilityCanGrabSomethingFromHere, new HarmonyMethod(objectOverridesCanGrabSomethingFromHere), null);

            var utilityPlayerCanPlaceItemHere = typeof(Utility).GetMethod("playerCanPlaceItemHere");
            var objectOverridesPlayerCanPlaceItemHere = typeof(TransplantOverrides).GetMethod("PlayerCanPlaceItemHere");
            harmony.Patch(utilityPlayerCanPlaceItemHere, new HarmonyMethod(objectOverridesPlayerCanPlaceItemHere), null);

            var hoeDirtPerformUseAction = typeof(HoeDirt).GetMethod("performUseAction");
            var objectOverridesPerformUseAction = typeof(TransplantOverrides).GetMethod("PerformUseAction");
            harmony.Patch(hoeDirtPerformUseAction, new HarmonyMethod(objectOverridesPerformUseAction), null);

            var fruitTreePerformUseAction = typeof(FruitTree).GetMethod("performUseAction");
            var transplantOverridesFruitTreePerformUseAction = typeof(TransplantOverrides).GetMethod("FruitTreePerformUseAction");
            harmony.Patch(fruitTreePerformUseAction, null, new HarmonyMethod(transplantOverridesFruitTreePerformUseAction));

            var game1PressUseToolButton = typeof(Game1).GetMethod("pressUseToolButton");
            var objectOverridesPressUseToolButton = typeof(TransplantOverrides).GetMethod("PressUseToolButton");
            harmony.Patch(game1PressUseToolButton, new HarmonyMethod(objectOverridesPressUseToolButton), null);

            if (DataLoader.ModConfig.EnableSoilTileUnderTrees)
            {
                var treeDraw = typeof(Tree).GetMethod("draw");
                var transplantOverridesPreTreeDraw = typeof(TransplantOverrides).GetMethod("PreTreeDraw");
                harmony.Patch(treeDraw, new HarmonyMethod(transplantOverridesPreTreeDraw), null);
            }
        }

        /// <summary>Raised before the game begins writes data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (Game1.player.ActiveObject is HeldIndoorPot)
            {
                Game1.player.ActiveObject = TransplantOverrides.RegularPotObject;
                TransplantOverrides.CurrentHeldIndoorPot = null;
            }
        }
    }
}
