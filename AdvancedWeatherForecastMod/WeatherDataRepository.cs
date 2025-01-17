using StardewValley.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace AdvancedWeatherForecastMod
{
    internal class WeatherDataRepository
    {
        private static Dictionary<string, Dictionary<long, string>>? _weatherData = null;

        public static void ClearWeatherData()
        {
            foreach (var weatherDataValue in _weatherData.Values)
            {
                weatherDataValue.Clear();
            }

            AdvancedWeatherForecastModEntry.ModHelper.Data.WriteSaveData<Dictionary<string, Dictionary<long, string>>>("WeatherData", _weatherData);
        }

        public static void ClearWeatherDataCache()
        {
            _weatherData = null;
        }

        public static Dictionary<string,Dictionary<long, string>> GetWeatherData()
        {
            return _weatherData = _weatherData ?? AdvancedWeatherForecastModEntry.ModHelper.Data.ReadSaveData<Dictionary<string, Dictionary<long, string>>>("WeatherData") ?? new Dictionary<string, Dictionary<long, string>>();
        }
        
        public static void SaveWeatherData()
        {
            AdvancedWeatherForecastModEntry.ModHelper.Data.WriteSaveData<Dictionary<string, Dictionary<long, string>>>("WeatherData", _weatherData);
        }

        public static void ClearPastWeatherData()
        {
            if (_weatherData == null) return;
            foreach (var locationWeatherData in _weatherData.Values)
            {
                locationWeatherData.RemoveWhere(i => i.Key <= Game1.Date.TotalDays);
            }
        }
    }
}
