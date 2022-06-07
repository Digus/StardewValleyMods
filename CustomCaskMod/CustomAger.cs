using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCaskMod
{
    public class CustomAger
    {
        public string ModUniqueID;
        public string Name;
        public bool EnableAgingAnywhere;
        public bool EnableMoreThanOneQualityIncrementPerDay;
        public Dictionary<object, float> AgingData;
        public Dictionary<int, float> AgingDataId = new Dictionary<int, float>();
        public List<string> OverrideMod = new List<string>();
        public List<string> MergeIntoMod = new List<string>();
    }
}
