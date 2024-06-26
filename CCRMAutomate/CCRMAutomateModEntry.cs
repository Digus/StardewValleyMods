﻿using CCRMAutomate.Automate;
using Pathoschild.Stardew.Automate;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CCRMAutomate
{
    public class CCRMAutomateModEntry : Mod
    {
        internal static IMonitor ModMonitor { get; set; }
        internal new static IModHelper Helper { get; set; }

        public static ModConfig ModConfig;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            Helper = helper;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            ModConfig = Helper.ReadConfig<CCRMAutomate.ModConfig>();
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            IAutomateAPI automate = Helper.ModRegistry.GetApi<IAutomateAPI>("Pathoschild.Automate");
            automate?.AddFactory(new CustomCrystalariumAutomationFactory());
        }
    }
}
