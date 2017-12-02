using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.animals
{
    public class DuckItem : AnimalItem, FeatherAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumNumberOfFeatherChances { get; set; }
        public int MaximumNumberOfFeatherChances { get; set; }

        public DuckItem()
        {
            MinimalNumberOfMeat = 2;
            MaximumNumberOfMeat = 6;
            MinimumNumberOfFeatherChances = 0;
            MaximumNumberOfFeatherChances = 2;
        }
    }
}
