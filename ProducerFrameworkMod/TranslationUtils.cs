using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerFrameworkMod
{
    internal class TranslationUtils
    {
        internal const string ContextTagPrefix = "pfm_key_";
        private static readonly Dictionary<int, string> Translations = new Dictionary<int, string>();

        internal static void AddTranslation(string key, string value)
        {
            Translations[GenerateHashFromKey(key)] = value;
        }

        internal static string GetTranslationFromContextTag(string context)
        {
            return Translations[int.Parse(context.Replace(ContextTagPrefix, ""))];
        }

        internal static string GetTranslationFromKey(string key)
        {
            return Translations[GenerateHashFromKey(key)];
        }

        internal static string GetContextTagForKey(string key)
        {
            return ContextTagPrefix + GenerateHashFromKey(key);
        }

        internal static int GenerateHashFromKey(string key)
        {
            return key.GetHashCode();
        }
    }
}
