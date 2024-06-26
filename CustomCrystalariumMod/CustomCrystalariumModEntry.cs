﻿using System.Linq;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SObject = StardewValley.Object;

namespace CustomCrystalariumMod
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class CustomCrystalariumModEntry : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;
        public static IManifest Manifest;

        public static DataLoader DataLoader;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;
            Manifest = ModManifest;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.UpdateTicking += OnGameLoopOnUpdateTicking;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            helper.ConsoleCommands.Add("config_reload_contentpacks_customcrystalariummod", "Reload all content packs for custom crystalarium mod.", DataLoader.LoadContentPacksCommand);
        }

        private void OnGameLoopOnUpdateTicking(object sender, UpdateTickingEventArgs args)
        {
            if (args.Ticks == 120)
            {
                DataLoader.LoadCrystalariumDataIds();
                DataLoader.LoadContentPacksCommand();
                Helper.Events.GameLoop.UpdateTicking -= OnGameLoopOnUpdateTicking;
            }
        }

        /*********
         ** Private methods
         *********/
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            DataLoader = new DataLoader(ModHelper, ModManifest);
            var harmony = new Harmony("Digus.CustomCrystalariumMod");

            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), "OutputMachine"),
                postfix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.OutputMachine)) { priority = Priority.HigherThanNormal }
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.PerformObjectDropInAction)) { priority = Priority.HigherThanNormal}
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performRemoveAction)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.PerformRemoveAction)) { priority = Priority.HigherThanNormal }
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performToolAction)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.PerformToolAction)) { priority = Priority.HigherThanNormal }
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.checkForAction)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.CheckForAction_prefix)) { priority = Priority.HigherThanNormal },
                postfix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.CheckForAction_postfix)) { priority = Priority.HigherThanNormal }
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.TryApplyFairyDust)),
                prefix: new HarmonyMethod(typeof(ObjectOverrides), nameof(ObjectOverrides.TryApplyFairyDust)) { priority = Priority.HigherThanNormal }
            );
        }

        /// <summary>Raised after the player loads a save slot and the world is initialized.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private static void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            DataLoader.LoadContentPacksCommand();
        }
    }
}
