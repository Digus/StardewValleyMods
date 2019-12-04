using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerFrameworkMod.ContentPack
{
    public class ProducerItem
    {
        public string ProducerName;
        public string InputIdentifier;
        public int InputStack = 1;
        public string FuelIdentifier;
        public int? FuelStack;
        public int MinutesUntilReady;
        public string OutputIdentifier;
        public string OutputName;
        public AffixType? CompoundOutputName;
        public bool InputPriceBased;
        public int OutputPriceIncrement = 0;
        public double OutputPriceMultiplier = 1;
        public int? OutputQuality = 0;
        public int OutputStack = 1;
        public List<string> Sounds = new List<string>();
        public PlacingAnimation PlacingAnimation;
        internal int OutputIndex;
    }

    public enum AffixType
    {
        Prefix,
        Sulfix
    }

    public enum PlacingAnimation
    {
        Boobles,
        Smoke
    }
}
