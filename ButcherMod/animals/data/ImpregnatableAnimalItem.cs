using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals
{
    public interface ImpregnatableAnimalItem 
    {
        int MinimumDaysUtillBirth { get; set; }
        
    }
}
