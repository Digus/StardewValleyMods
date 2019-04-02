using System.Reflection;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace CustomCrystalariumMod
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class CustomCrystalariumModEntry : Mod
    {
        public static IMonitor ModMonitor;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
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

            var objectGetMinutesForCrystalarium = typeof(Object).GetMethod("getMinutesForCrystalarium", BindingFlags.NonPublic | BindingFlags.Instance);
            var objectOverridesGetMinutesForCrystalarium = typeof(ObjectOverrides).GetMethod("GetMinutesForCrystalarium");
            harmony.Patch(objectGetMinutesForCrystalarium, new HarmonyMethod(objectOverridesGetMinutesForCrystalarium), null);

            var objectPerformObjectDropInAction = typeof(Object).GetMethod("performObjectDropInAction");
            var objectOverridesPerformObjectDropInAction = typeof(ObjectOverrides).GetMethod("PerformObjectDropInAction");
            harmony.Patch(objectPerformObjectDropInAction, new HarmonyMethod(objectOverridesPerformObjectDropInAction), null);

            if (DataLoader.ModConfig.GetObjectBackOnChange && !DataLoader.ModConfig.GetObjectBackImmediately)
            {
                var objectPerformRemoveAction = typeof(Object).GetMethod("performRemoveAction");
                var objectOverridesPerformRemoveAction = typeof(ObjectOverrides).GetMethod("PerformRemoveAction");
                harmony.Patch(objectPerformRemoveAction, new HarmonyMethod(objectOverridesPerformRemoveAction), null);
            }
        }
    }
}
