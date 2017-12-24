using System;
using AnimalHusbandryMod.meats;

namespace AnimalHusbandryMod.animals
{
    internal class MeatAttribute : Attribute
    {
        public Meat Meat;

        public MeatAttribute(Meat meat)
        {
            this.Meat = meat;
        }
    }
}