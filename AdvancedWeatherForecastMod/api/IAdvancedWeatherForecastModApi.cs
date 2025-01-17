using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Utilities;

namespace AdvancedWeatherForecastMod.api
{
    public interface IAdvancedWeatherForecastModApi
    {
        /// <summary>
        /// Get a copy of weather forecast data. Changing this dictionary does not alter the weather.
        /// The key to the dictionary is the locationContextId, that is different from the location name.
        /// The value of the dictionary is another dictionary that the key is the date number of days since the start of the game and the value is the weather string identification.
        /// 
        /// </summary>
        /// <returns>The dictionary containing the weather data for all locations.</returns>
        public Dictionary<string,Dictionary<long,string>> GetWeatherForecastData();
        /// <summary>
        /// Get the weather for a particular location and date.
        /// </summary>
        /// <param name="locationContextId">The context id of a location</param>
        /// <param name="date">The smapi date</param>
        /// <returns>The weather identification string, null if there is no weather for that location and date</returns>
        public string? GetWeatherForecast(string locationContextId, SDate date);
        /// <summary>
        /// Set the weather for a particular location and date.
        /// </summary>
        /// <param name="locationContextId">The context id of a location</param>
        /// <param name="date">The smapi date</param>
        /// <param name="weather">The weather identification string</param>
        public void SetWeatherForecast(string locationContextId, SDate date, string weather);
    }
}
