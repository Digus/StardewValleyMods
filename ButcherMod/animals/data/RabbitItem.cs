using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.animals
{
    public class RabbitItem : AnimalItem, WoolAnimalItem, ImpregnatableAnimalItem, FeetAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumNumberOfExtraWool { get; set; }
        public int MaximumNumberOfExtraWool { get; set; }
        public int MinimumNumberOfFeetChances { get; set; }
        public int MaximumNumberOfFeetChances { get; set; }
        public int MinimumDaysUtillBirth { get; set; }

        public RabbitItem()
        {

            MinimalNumberOfMeat = 1;
            MaximumNumberOfMeat = 4;
            MinimumNumberOfExtraWool = 0;
            MaximumNumberOfExtraWool = 1;
            MinimumNumberOfFeetChances = 0;
            MaximumNumberOfFeetChances = 4;
            MinimumDaysUtillBirth = 9;
        }    
    }
}
