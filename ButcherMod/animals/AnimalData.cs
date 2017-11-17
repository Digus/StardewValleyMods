using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.animals
{
    public class AnimalData
    {
        public AnimalItem Cow;
        public AnimalItem Pig;
        public AnimalItem Chicken;
        public FeatherAnimalItem Duck;
        public FeetAnimalItem Rabbit;
        public WoolAnimalItem Sheep;
        public AnimalItem Goat;

        public AnimalData()
        {
            Cow = new AnimalItem(5, 20);
            Pig = new AnimalItem(4, 16);
            Chicken = new AnimalItem(1, 4);
            Duck = new FeatherAnimalItem(2, 6)
            {
                MinimumNumberOfFeatherChances = 0,
                MaximumNumberOfFeatherChances = 2
            };
            Rabbit = new FeetAnimalItem(1, 4)
            {
                MinimumNumberOfExtraWool = 0,
                MaximumNumberOfExtraWool = 1,
                MinimumNumberOfFeetChances = 0,
                MaximumNumberOfFeetChances = 4
            };
            Sheep = new WoolAnimalItem(4, 16)
            {
                MinimumNumberOfExtraWool = 0,
                MaximumNumberOfExtraWool = 2
            };
            Goat = new AnimalItem(3, 8);
        }

        public AnimalItem getAnimalItem(Animal animalEnum)
        {
            switch (animalEnum)
            {
                case Animal.Cow:
                    return Cow;
                case Animal.Pig:
                    return Pig;
                case Animal.Chicken:
                    return Chicken;
                case Animal.Duck:
                    return Duck;
                case Animal.Rabbit:
                    return Rabbit;
                case Animal.Sheep:
                    return Sheep;
                case Animal.Goat:
                    return Goat;
                default:
                    throw new ArgumentException("Invalid Animal");
            }
        }
    }
}
