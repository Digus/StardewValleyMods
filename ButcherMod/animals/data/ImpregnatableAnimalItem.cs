using System;

namespace AnimalHusbandryMod.animals.data
{
    public interface ImpregnatableAnimalItem 
    {
        int? MinimumDaysUtillBirth { get; set; }
        bool CanUseDeluxeItemForPregnancy { get; set; }
    }
}
