﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalHusbandryMod.animals.data
{
    public class CowItem : AnimalItem, MeatAnimalItem, TreatItem, ImpregnatableAnimalItem
    {
        public int MinimalNumberOfMeat { get; set; }
        public int MaximumNumberOfMeat { get; set; }
        public int MinimumDaysBetweenTreats { get; set; }
        public object[] LikedTreats { get; set; }
        public ISet<string> LikedTreatsId { get; set; }
        public int? MinimumDaysUtillBirth { get; set; }
        public bool CanUseDeluxeItemForPregnancy { get; set; }

        public CowItem()
        {
            MinimalNumberOfMeat = 5;
            MaximumNumberOfMeat = 20;
            MinimumDaysUtillBirth = 12;
            MinimumDaysBetweenTreats = 5;
            LikedTreats = new object[] { 78, 264, 300, 184 };
            LikedTreatsId = new HashSet<string>();
            CanUseDeluxeItemForPregnancy = true;
        }
    }
}
