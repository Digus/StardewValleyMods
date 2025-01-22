using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedWeatherForecastMod.integrations;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace AdvancedWeatherForecastMod.common
{
    public class DataLoader
    {
        public static IModHelper Helper;
        public static ITranslationHelper I18N;
        public static ModConfig ModConfig;
        public static ICloudySkiesApi? CloudySkiesApi;

        public DataLoader(IModHelper helper, IManifest manifest)
        {
            Helper = helper;
            I18N = helper.Translation;
            ModConfig = helper.ReadConfig<ModConfig>();

            LoadMail();

            CreateConfigMenu(manifest);
            try
            {
                CloudySkiesApi = Helper.ModRegistry.GetApi<ICloudySkiesApi>("leclair.cloudyskies");
            }
            catch (Exception e)
            {
                AdvancedWeatherForecastModEntry.ModMonitor.Log("Fail to load Cloud Skies API. Any expected integration won't work.",LogLevel.Warn);
            }
        }

        internal static void LoadMail()
        {
            IMailFrameworkModApi mailFrameworkModApi = Helper.ModRegistry.GetApi<IMailFrameworkModApi>("DIGUS.MailFrameworkMod");

            mailFrameworkModApi?.RegisterLetter(
                new ApiLetter
                {
                    Id = "AdvancedWeatherForecastLetter"
                    , Text = "AdvancedWeatherForecast.Letter"
                    , Title = "AdvancedWeatherForecast.Letter.Title"
                    , I18N = I18N
                }, (l) => !Game1.player.mailReceived.Contains(l.Id) && !ModConfig.DisableForecastLetter
                , (l) => Game1.player.mailReceived.Add(l.Id)
            );
        }

        private void CreateConfigMenu(IManifest manifest)
        {
            GenericModConfigMenuApi? api = Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api == null) return;

            api.Register(manifest, () => ModConfig = new ModConfig(), () => Helper.WriteConfig(ModConfig));

            api.AddNumberOption(manifest, () => ModConfig.DaysInAdvanceForecast, (val) => ModConfig.DaysInAdvanceForecast = val, () => I18N.Get("AdvancedWeatherForecast.ConfigMenu.DaysInAdvanceForecast.Name"), () => I18N.Get("AdvancedWeatherForecast.ConfigMenu.DaysInAdvanceForecast.Tooltip") , 1, 27);
            api.AddBoolOption(manifest, () => ModConfig.DisableForecastLetter, (val) => ModConfig.DisableForecastLetter = val, () => I18N.Get("AdvancedWeatherForecast.ConfigMenu.DisableForecastLetter.Name"), () => I18N.Get("AdvancedWeatherForecast.ConfigMenu.DisableForecastLetter.Tooltip"));
        }
    }
}
