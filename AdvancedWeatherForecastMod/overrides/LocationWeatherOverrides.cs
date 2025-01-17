using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Force.DeepCloner;
using HarmonyLib;
using Netcode;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.LocationContexts;
using StardewValley.Network;
using DataLoader = AdvancedWeatherForecastMod.common.DataLoader;

namespace AdvancedWeatherForecastMod.overrides
{
    public class LocationWeatherOverrides
    {
        public static void UpdateDailyWeather_Prefix(LocationWeather __instance, string locationContextId)
        {
            if (!WeatherForecastController.IsFutureDay()) return;
            WeatherForecastController.AddDateFutureDay();
        }

        public static void UpdateDailyWeather_Postfix(LocationWeather __instance, string locationContextId)
        {
            WeatherForecastController.ApplyWeatherModificationsForDateTomorrow(__instance);
            if (!WeatherForecastController.IsFutureDay()) return;
            WeatherForecastController.UpdateLocationWeatherData(__instance, locationContextId);
            WeatherForecastController.SubtractDateFutureDay();
        }

        public static void UpdateWeatherForNewDay_Postfix()
        {
            if (DataLoader.ModConfig.DaysInAdvanceForecast <= 1) return;

            WeatherForecastController.UpdateWeatherForTomorrow();
            WeatherForecastController.CalculateWeatherForNewDay();
            WeatherDataRepository.ClearPastWeatherData();
        }
    }
}
