using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCrystalariumMod
{
    public class CustomCloner
    {
        public string ModUniqueID;
        public string Name;
        public bool GetObjectBackOnChange;
        public bool GetObjectBackImmediately;
        public bool UsePfmForInput;
        public Dictionary<object, int> CloningData;
        internal Dictionary<int, int> CloningDataId = new Dictionary<int, int>();
    }
}
