using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

namespace AnimalHusbandryMod.animals
{
    public class AnimalStatus
    {
        public long Id;
        [JsonProperty]
        private string _lastDayFeedTreat;
        [JsonIgnore]
        public SDate LastDayFeedTreat
        {
            get => _lastDayFeedTreat == null ? null : new SDate(Convert.ToInt32(_lastDayFeedTreat.Split(' ')[0]), _lastDayFeedTreat.Split(' ')[1], Convert.ToInt32(_lastDayFeedTreat.Split(' ')[2].Replace("Y","")));
            set => _lastDayFeedTreat = value?.ToString();
        }
        public Dictionary<int,int> FeedTreatsQuantity;

        public AnimalStatus(long id)
        {
            Id = id;
            FeedTreatsQuantity = new Dictionary<int, int>();
        }
    }
}
