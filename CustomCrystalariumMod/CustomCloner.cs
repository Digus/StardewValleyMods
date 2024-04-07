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
        public string QualifiedItemId;
        public string Name;
        public bool GetObjectBackOnChange;
        public bool GetObjectBackImmediately;
        public bool KeepQuality = true;
        public bool BlockChange;
        public bool EnableCloneEveryObject;
        public int DefaultCloningTime = 5000;
        public bool UsePfmForInput;
        public Dictionary<object, int> CloningData;
        public Dictionary<string, int> CloningDataId = new Dictionary<string, int>();
    }
}
