using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.animals
{
    public class FeatherAnimalItem : AnimalItem
    {
        public int MinimumNumberOfFeatherChances;
        public int MaximumNumberOfFeatherChances;

        public FeatherAnimalItem(int minimalNumberOfMeat, int maximumNumberOfMeat) : base(minimalNumberOfMeat, maximumNumberOfMeat)
        {
        }
    }
}
