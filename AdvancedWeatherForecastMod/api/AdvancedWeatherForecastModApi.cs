using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Force.DeepCloner;
using StardewModdingAPI.Utilities;
using xTile.Dimensions;

namespace AdvancedWeatherForecastMod.api
{
    public class AdvancedWeatherForecastModApi : IAdvancedWeatherForecastModApi
    {
        public Dictionary<string, Dictionary<long, string>> GetWeatherForecastData()
        {
            return WeatherDataRepository.GetWeatherData().DeepClone();
        }

        public string? GetWeatherForecast(string locationContextId, SDate date)
        {
            string? weather = null;
            if (WeatherDataRepository.GetWeatherData().TryGetValue(locationContextId, out var locationWeatherData))
            {
                locationWeatherData.TryGetValue(date.DaysSinceStart, out weather);
            }
            return weather;
        }

        /// <summary>
        /// Set the weather for a particular location and date.
        /// </summary>
        /// <param name="locationContextId">The context id of a location</param>
        /// <param name="date">The smapi date</param>
        /// <param name="weather">The weather identification string</param>
        public void SetWeatherForecast(string locationContextId, SDate date, string weather)
        {
            var weatherData = WeatherDataRepository.GetWeatherData();
            if (!weatherData.ContainsKey(locationContextId)) weatherData.Add(locationContextId, new Dictionary<long, string>());
            weatherData[locationContextId][date.DaysSinceStart] = weather;
        }
    }
}
