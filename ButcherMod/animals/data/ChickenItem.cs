using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.animals
{
    public class ChickenItem : AnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }

        public ChickenItem()
        {
            MinimalNumberOfMeat = 1;
            MaximumNumberOfMeat = 4;
        }
    }
}
