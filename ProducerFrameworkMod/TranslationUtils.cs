using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;

namespace ProducerFrameworkMod
{
    internal class TranslationUtils
    {
        private static readonly Dictionary<int, string> Translations = new Dictionary<int, string>();

        private static LocalizedContentManager.LanguageCode? _activeLanguageCode = null;

        internal static void AddTranslation(int index, string value)
        {
            if (Game1.content.GetCurrentLanguage() != _activeLanguageCode)
            {
                _activeLanguageCode = Game1.content.GetCurrentLanguage();
                Translations.Clear();
            }

            if (!Translations.ContainsKey(index))
            {
                Translations[index] = value;
            }
            else if (Translations[index] != value)
            {
                ProducerFrameworkModEntry.ModMonitor.Log($"There is already a translation for the object with the index '{index}'. The translation '{value}' will be ignored.",LogLevel.Warn);
            }
        }

        internal static string GetTranslationFromIndex(int index)
        {
            return Translations[index];
        }

        internal static bool HasTranslationForIndex(int index)
        {
            return Translations.ContainsKey(index);
        }
    }
}
