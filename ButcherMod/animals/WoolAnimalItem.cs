using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.animals
{
    public class WoolAnimalItem : AnimalItem
    {
        public int MinimumNumberOfExtraWool;
        public int MaximumNumberOfExtraWool;

        public WoolAnimalItem(int minimalNumberOfMeat, int maximumNumberOfMeat) : base(minimalNumberOfMeat, maximumNumberOfMeat)
        {
        }
    }
}
