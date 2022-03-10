﻿using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;


namespace AnimalHusbandryMod.animals.data
{
    public class PregnancyData
    {
        public Dictionary<string, string> PreganancyItems;

        public PregnancyData()
        {
            PreganancyItems = new Dictionary<string, string>();
        }

        public bool MatchingPregnancyItem(FarmAnimal animal, SObject o)
        {
            if (animal == null || o == null)
            {
                return false;
            }
            else if (!PreganancyItems.ContainsKey(animal.displayType))
            {
                AnimalHusbandryModEntry.monitor.Log($"Dont know anything about {animal.displayType}");
                return false;
            }
            return PreganancyItems.ContainsKey(animal.displayType) && PreganancyItems[animal.displayType].Equals(o.Name);
        }
    }
}
