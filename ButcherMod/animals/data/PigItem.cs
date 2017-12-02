using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.animals
{
    public class PigItem : AnimalItem, ImpregnatableAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumDaysUtillBirth { get; set; }

        public PigItem()
        {
            MinimalNumberOfMeat = 4;
            MaximumNumberOfMeat = 16;
            MinimumDaysUtillBirth = 18;
        }
    }
}
