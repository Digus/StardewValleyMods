using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals
{
    public class GoatItem : AnimalItem, ImpregnatableAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumDaysUtillBirth { get; set; }

        public GoatItem()
        {
            MinimalNumberOfMeat = 3;
            MaximumNumberOfMeat = 8;
            MinimumDaysUtillBirth = 10;
        }
    }
}
