using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.animals
{
    public class FeetAnimalItem : WoolAnimalItem
    {
        public int MinimumNumberOfFeetChances;
        public int MaximumNumberOfFeetChances;

        public FeetAnimalItem(int minimalNumberOfMeat, int maximumNumberOfMeat) : base(minimalNumberOfMeat, maximumNumberOfMeat)
        {
        }
    }
}
