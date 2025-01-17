using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley;
using static StardewValley.Menus.Billboard;
using AdvancedWeatherForecastMod.api;
using StardewModdingAPI;
using DataLoader = AdvancedWeatherForecastMod.common.DataLoader;

namespace AdvancedWeatherForecastMod.overrides
{
    public class BillboardOverrides
    {
        public static void draw(Billboard __instance, SpriteBatch b)
        {
            try
            {
                bool dailyQuestBoard = AdvancedWeatherForecastModEntry.ModHelper.Reflection.GetField<bool>(__instance, "dailyQuestBoard").GetValue();
                if (!dailyQuestBoard)
                {
                    for (int i = 0; i < __instance.calendarDays.Count; i++)
                    {
                        ClickableTextureComponent component = __instance.calendarDays[i];
                        __instance.calendarDayData.TryGetValue(component.myID, out var day);
                        bool wedding = day?.Type.HasFlag(BillboardEventType.Wedding) ?? false;
                        if (!wedding)
                        {

                            int weatherIcon = 0;
                            string weather;
                            if (i == Game1.dayOfMonth % 28)
                            {
                                weather = Game1.currentLocation.GetWeather().WeatherForTomorrow;
                            }
                            else
                            {
                                var weatherData = WeatherDataRepository.GetWeatherData();
                                var locationWeatherData = weatherData[Game1.currentLocation.locationContextId];
                                if (i >= Game1.dayOfMonth)
                                {
                                    if (!locationWeatherData.ContainsKey(Game1.Date.TotalDays + 1 + i - Game1.dayOfMonth)) continue;
                                    weather = locationWeatherData[Game1.Date.TotalDays + 1 + i - Game1.dayOfMonth];
                                }
                                else
                                {
                                    if (!locationWeatherData.ContainsKey(Game1.Date.TotalDays + 29 + i - Game1.dayOfMonth)) continue;
                                    weather = locationWeatherData[Game1.Date.TotalDays + 29 + i - Game1.dayOfMonth];
                                }
                            }
                            var texture = Game1.mouseCursors;
                            Rectangle? sourceRectangle = null;
                            switch (weather)
                            {
                                case Game1.weather_green_rain:
                                    b.Draw(Game1.mouseCursors_1_6, new Vector2(component.bounds.Right - 48, component.bounds.Top + 4), new Rectangle(244, 294, 11, 7), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, .75f);
                                    continue;
                                case Game1.weather_festival:
                                    weatherIcon = 1;
                                    break;
                                case Game1.weather_lightning:
                                    weatherIcon = 5;
                                    break;
                                case Game1.weather_snow:
                                    weatherIcon = 7;
                                    break;
                                case Game1.weather_rain:
                                    weatherIcon = 4;
                                    break;
                                case Game1.weather_debris:
                                    if (i >= Game1.dayOfMonth)
                                    {
                                        if (Game1.IsSpring) weatherIcon = 3;
                                        if (Game1.IsFall) weatherIcon = 6;
                                        if (Game1.IsWinter) weatherIcon = 7;
                                    }
                                    else
                                    {
                                        if (Game1.IsWinter) weatherIcon = 3;
                                        if (Game1.IsSummer) weatherIcon = 6;
                                        if (Game1.IsFall) weatherIcon = 7;
                                    }

                                    break;
                                default:
                                    if (DataLoader.CloudySkiesApi != null && weather != null)
                                    {
                                        if (DataLoader.CloudySkiesApi.TryGetWeather(weather, out IWeatherData? cloudySkyWeatherData))
                                        {
                                            texture = DataLoader.Helper.GameContent.Load<Texture2D>(cloudySkyWeatherData.IconTexture);
                                            sourceRectangle = new Rectangle(cloudySkyWeatherData.IconSource.X, cloudySkyWeatherData.IconSource.Y + 1, 11, 7);
                                            break;
                                        }
                                    }
                                    weatherIcon = 2;
                                    break;
                            };
                            if (sourceRectangle == null)
                            {
                                sourceRectangle = new Rectangle(317 + 12 * weatherIcon, 422, 11, 7);
                            }
                            b.Draw(texture, new Vector2(component.bounds.Right - 48, component.bounds.Top + 4), sourceRectangle, Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, .5f);

                        }

                        Game1.mouseCursorTransparency = 1f;
                        __instance.drawMouse(b);
                        var hoverText = AdvancedWeatherForecastModEntry.ModHelper.Reflection.GetField<string>(__instance, "hoverText").GetValue();
                        if (hoverText.Length > 0)
                        {
                            IClickableMenu.drawHoverText(b, hoverText, Game1.dialogueFont);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AdvancedWeatherForecastModEntry.ModMonitor.LogOnce($"Fail to draw the weather indicator in the calendar. Exception message: '{e.Message}'", LogLevel.Error);
                AdvancedWeatherForecastModEntry.ModMonitor.LogOnce(e.StackTrace, LogLevel.Trace);
            }
        }
    }
}
