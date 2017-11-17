using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.animals
{
    public class AnimalItem
    {
        public int MinimalNumberOfMeat;
        public int MaximumNumberOfMeat;

        public AnimalItem(int minimalNumberOfMeat, int maximumNumberOfMeat)
        {
            this.MinimalNumberOfMeat = minimalNumberOfMeat;
            this.MaximumNumberOfMeat = maximumNumberOfMeat;
        }
    }
}
