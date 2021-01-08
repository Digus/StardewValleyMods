using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace ProducerFrameworkMod.ContentPack
{
    public class ContentPackConfig
    {

        private LogLevel _defaultWarningsLogLevel = LogLevel.Warn;
        public LogLevel DefaultWarningsLogLevel
        {
            get => _defaultWarningsLogLevel;
            set => _defaultWarningsLogLevel = value < LogLevel.Warn ? value : LogLevel.Warn;
        }
    }
}
