using System;
using ButcherMod.meats;

namespace ButcherMod.animals
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