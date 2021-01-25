using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Buildings;

namespace AnimalHusbandryMod.common
{
    public class AnimalUtility
    {
        public static IEnumerable<FarmAnimal> FindAnimals(Func<FarmAnimal, bool> filter)
        {
            IEnumerable<FarmAnimal> animals = Game1.getFarm().animals.Values.Where(filter);
            foreach (Building b in Game1.getFarm().buildings)
            {
                if (b.indoors.Value is AnimalHouse animalHouse)
                {
                    animals = animals.Concat(animalHouse.animals.Values.Where(filter));
                }
            }
            return animals;
        }
    }
}