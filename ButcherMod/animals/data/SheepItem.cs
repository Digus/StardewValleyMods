using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.animals
{
    public class SheepItem : AnimalItem , WoolAnimalItem, ImpregnatableAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumNumberOfExtraWool { get; set; }
        public int MaximumNumberOfExtraWool { get; set; }        
        public int MinimumDaysUtillBirth { get; set; }

        public SheepItem()
        {
            MinimalNumberOfMeat = 4;
            MaximumNumberOfMeat = 16;
            MinimumNumberOfExtraWool = 0;
            MaximumNumberOfExtraWool = 2;
            MinimumDaysUtillBirth = 8;
        }
    }
}
