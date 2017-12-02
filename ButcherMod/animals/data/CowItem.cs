using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.animals
{
    public class CowItem : AnimalItem, ImpregnatableAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumDaysUtillBirth { get; set; }

        public CowItem()
        {
            MinimalNumberOfMeat = 5;
            MaximumNumberOfMeat = 20;
            MinimumDaysUtillBirth = 12;
        }
    }
}
