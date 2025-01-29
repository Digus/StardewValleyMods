using AdvancedWeatherForecastMod.api;
using AdvancedWeatherForecastMod.overrides;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using DataLoader = AdvancedWeatherForecastMod.common.DataLoader;

namespace AdvancedWeatherForecastMod
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class AdvancedWeatherForecastModEntry : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;
        public static IManifest Manifest;

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// Here it loads the custom event handlers for the start of the day, after load and after returning to the title screen.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;
            Manifest = ModManifest;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
        }

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialized at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            new DataLoader(ModHelper, ModManifest);

            var harmony = new Harmony("Digus.AdvancedWeatherForecastMod");

            harmony.Patch(
                original: AccessTools.Method(typeof(LocationWeather), nameof(LocationWeather.UpdateDailyWeather)),
                prefix: new HarmonyMethod(typeof(LocationWeatherOverrides),
                    nameof(LocationWeatherOverrides.UpdateDailyWeather_Prefix)),
                postfix: new HarmonyMethod(typeof(LocationWeatherOverrides),
                    nameof(LocationWeatherOverrides.UpdateDailyWeather_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.UpdateWeatherForNewDay)),
                postfix: new HarmonyMethod(typeof(LocationWeatherOverrides),
                    nameof(LocationWeatherOverrides.UpdateWeatherForNewDay_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Billboard), nameof(Billboard.draw), new[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(BillboardOverrides), nameof(BillboardOverrides.draw))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Billboard), nameof(Billboard.performHoverAction)),
                postfix: new HarmonyMethod(typeof(BillboardOverrides), nameof(BillboardOverrides.performHoverAction_postfix))
            );

            ModHelper.ConsoleCommands.Add("world_clear_weatherforecastdata", "Clear weather forecast for all locations", (c, a) => WeatherDataRepository.ClearWeatherData());
            ModHelper.ConsoleCommands.Add("world_calculate_weatherforecast", "Calculate weather forecast for all locations. Won't override data, needs to clear first.", (c, a) => WeatherForecastController.CalculateAllForecast());

        }

        /// <summary>Raised after the player loads a save slot and the world is initialized.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            WeatherDataRepository.ClearWeatherDataCache();
            WeatherForecastController.CalculateAllForecast();
        }

        /// <summary>Raised before the game is saved.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaving(object? sender, SavingEventArgs e)
        {
            WeatherDataRepository.SaveWeatherData();
        }

        /// <inheritdoc />
        public override object? GetApi()
        {
            return new AdvancedWeatherForecastModApi();
        }
    }
}
