using AnimalHusbandryMod.animals.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals;

namespace AnimalHusbandryMod.farmer
{
    public class FarmerData
    {
        public List<PregnancyItem> PregnancyData;
        public List<AnimalStatus> AnimalData;

        public FarmerData()
        {
            PregnancyData = new List<PregnancyItem>();
            AnimalData = new List<AnimalStatus>();
        }
    }
}
