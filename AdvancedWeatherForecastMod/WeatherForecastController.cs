using StardewValley.GameData.LocationContexts;
using StardewValley.Network;
using StardewValley;
using System.Collections.Generic;
using Force.DeepCloner;
using StardewModdingAPI.Utilities;
using DataLoader = AdvancedWeatherForecastMod.common.DataLoader;

namespace AdvancedWeatherForecastMod
{
    internal class WeatherForecastController
    {
        private static int _futureDay = 0;

        public static bool IsFutureDay()
        {
            return Game1.stats.DaysPlayed > 0 && _futureDay > 0;
        }

        public static void AddDateFutureDay()
        {
            Game1.stats.DaysPlayed = (uint)(Game1.stats.DaysPlayed + _futureDay);
            var futureDate = SDate.From(Game1.Date).AddDays(_futureDay);
            Game1.dayOfMonth = futureDate.Day;
            Game1.season = futureDate.Season;
            Game1.year = futureDate.Year;
        }

        public static void SubtractDateFutureDay()
        {
            Game1.stats.DaysPlayed = (uint)(Game1.stats.DaysPlayed - _futureDay);
            var originalDate = SDate.From(Game1.Date).AddDays(-1 * _futureDay);
            Game1.dayOfMonth = originalDate.Day;
            Game1.season = originalDate.Season;
            Game1.year = originalDate.Year;
        }

        public static void CalculateAllForecast()
        {
            CalculateWeatherForecast(1);
        }

        public static void CalculateWeatherForNewDay()
        {
            CalculateWeatherForecast(DataLoader.ModConfig.DaysInAdvanceForecast - 1);
        }
        
        private static void CalculateWeatherForecast( int startAt)
        {
            var weatherData = WeatherDataRepository.GetWeatherData();

            foreach (KeyValuePair<string, LocationContextData> locationContextDataPair in Game1.locationContextData)
            {
                var location = locationContextDataPair.Key;
                var contextData = locationContextDataPair.Value;
                if (!weatherData.ContainsKey(location)) weatherData.Add(location, new Dictionary<long, string>());
                var locationWeatherData = weatherData[location];
                for (var i = startAt; i < DataLoader.ModConfig.DaysInAdvanceForecast; i++)
                {
                    _futureDay = i;
                    var dateTotalDays = Game1.Date.TotalDays + i + 1;
                    if (!locationWeatherData.ContainsKey(dateTotalDays) && contextData.CopyWeatherFromLocation == null)
                    {
                        var locationWeather = new LocationWeather();
                        locationWeather.UpdateDailyWeather(location, contextData, Game1.random);
                        locationWeatherData[dateTotalDays] = locationWeather.WeatherForTomorrow;
                    }
                }
            }
            _futureDay = 0;
            CheckWeatherDataToCopy(weatherData);
        }

        public static void UpdateWeatherForTomorrow()
        {
            var weatherData = WeatherDataRepository.GetWeatherData();
            foreach (var location in Game1.locationContextData.Keys)
            {
                if (!weatherData[location].ContainsKey(Game1.Date.TotalDays + 1)) continue;
                var weatherForLocation = Game1.netWorldState.Value.GetWeatherForLocation(location);
                if (weatherForLocation.WeatherForTomorrow is Game1.weather_festival or Game1.weather_wedding) weatherData[location][Game1.Date.TotalDays + 1] = weatherForLocation.WeatherForTomorrow;
                weatherForLocation.WeatherForTomorrow = weatherData[location][Game1.Date.TotalDays + 1];
            }
            Game1.weatherForTomorrow = Game1.netWorldState.Value.GetWeatherForLocation("Default").WeatherForTomorrow;
        }

        public static void UpdateLocationWeatherData(LocationWeather locationWeather, string locationContextId)
        {
            var weatherData = WeatherDataRepository.GetWeatherData();
            var locationWeatherData = weatherData[locationContextId];
            locationWeatherData[Game1.Date.TotalDays + 1] = locationWeather.WeatherForTomorrow;
        }

        public static void ApplyWeatherModificationsForDateTomorrow(LocationWeather locationWeather)
        {
            locationWeather.WeatherForTomorrow = Game1.getWeatherModificationsForDate(SDate.FromDaysSinceStart(Game1.Date.TotalDays + 2).ToWorldDate(), locationWeather.WeatherForTomorrow);
        }

        private static void CheckWeatherDataToCopy(Dictionary<string, Dictionary<long, string>> weatherData)
        {
            foreach (KeyValuePair<string, LocationContextData> pair in Game1.locationContextData)
            {
                var contextToCopy = pair.Value.CopyWeatherFromLocation;
                if (contextToCopy != null)
                {
                    weatherData[pair.Key] = weatherData[contextToCopy].DeepClone();
                }
            }
        }
    }
}
