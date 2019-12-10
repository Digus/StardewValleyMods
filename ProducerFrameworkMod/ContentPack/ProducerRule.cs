using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod.ContentPack
{
    public class ProducerRule
    {
        public string ProducerName;
        public string InputIdentifier;
        public int InputStack = 1;
        public string FuelIdentifier;
        public int FuelStack = 1;
        public int MinutesUntilReady;
        public string OutputIdentifier;
        public string OutputName;
        public AffixType? CompoundOutputName;
        public Object.PreserveType? PreserveType;
        public bool InputPriceBased;
        public int OutputPriceIncrement = 0;
        public double OutputPriceMultiplier = 1;
        public bool KeepInputQuality;
        public int? OutputQuality = 0;
        public int OutputStack = 1;
        public List<string> Sounds = new List<string>();
        public PlacingAnimation? PlacingAnimation;
        public string PlacingAnimationColorName;
        internal int OutputIndex;
        internal int? FuelIndex;
        internal Color PlacingAnimationColor = Color.White;
    }
}
