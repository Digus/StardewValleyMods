using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals
{
    public interface WoolAnimalItem 
    {
        int MinimumNumberOfExtraWool { get; set; }
        int MaximumNumberOfExtraWool { get; set; }
    }
}
