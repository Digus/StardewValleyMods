using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Utilities;

namespace AnimalHusbandryMod.animals
{
    public class AnimalStatus
    {
        public long Id;
        public SDate LastDayFeedTreat;
        public Dictionary<int,int> FeedTreatsQuantity;

        public AnimalStatus(long id)
        {
            Id = id;
            FeedTreatsQuantity = new Dictionary<int, int>();
        }
    }
}
