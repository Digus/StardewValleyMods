using AnimalHusbandryMod.animals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.farmer
{
    public class FarmerData
    {
        public List<PregnancyItem> PregnancyData;

        public FarmerData()
        {
            PregnancyData = new List<PregnancyItem>();
        }
    }
}
