﻿using AnimalHusbandryMod.meats;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using AnimalHusbandryMod.common;

namespace AnimalHusbandryMod.animals
{
    public static class AnimalExtension
    {
        public static Meat GetMeat(this Animal value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute = System.Attribute.GetCustomAttribute(field, typeof(MeatAttribute)) as MeatAttribute;

            return attribute.Meat;
        }

        public static Animal? GetAnimalFromType(string type)
        {
            if (!AnimalData.BaseGameAnimals.Contains(type) && DataLoader.AnimalData.CustomAnimals.Exists(a => type.Contains(a.Name)))
            {
                return Animal.CustomAnimal;
            }
            foreach (Animal animal in System.Enum.GetValues(typeof(Animal)))
            {
                if (type.Contains(animal.ToString()) && animal != Animal.CustomAnimal)
                {
                    return animal;
                }
            }
            return null;
        }

        public static string GetBabyAnimalName(this Animal value)
        {
            return DataLoader.i18n.Get($"Animal.{value.ToString()}.BabyName");
        }

        public static string GetBabyAnimalNameByType(string type)
        {
            return GetAnimalFromType(type)?.GetBabyAnimalName();
        }
    }
}
