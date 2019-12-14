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
        public List<string> ExcludeIdentifiers;
        public string FuelIdentifier;
        public int FuelStack = 1;
        public Dictionary<string, int> AdditionalFuel = new Dictionary<string, int>();
        public int MinutesUntilReady;
        public List<OutputConfig> AdditionalOutputs = new List<OutputConfig>();
        public List<string> Sounds = new List<string>();
        public PlacingAnimation? PlacingAnimation;
        public string PlacingAnimationColorName;
        internal List<Tuple<int,int>> FuelList = new List<Tuple<int, int>>();
        internal Color PlacingAnimationColor = Color.White;
        internal List<OutputConfig> OutputConfigs = new List<OutputConfig>();

        //Default output
        public string OutputIdentifier;
        public string OutputName;
        public Object.PreserveType? PreserveType;
        public bool InputPriceBased;
        public int OutputPriceIncrement = 0;
        public double OutputPriceMultiplier = 1;
        public bool KeepInputQuality;
        public int OutputQuality = 0;
        public int OutputStack = 1;
        public int OutputMaxStack = 1;
        public StackConfig SilverQualityInput = new StackConfig();
        public StackConfig GoldQualityInput = new StackConfig();
        public StackConfig IridiumQualityInput = new StackConfig();
    }
}
