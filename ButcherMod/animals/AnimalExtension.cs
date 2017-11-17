using ButcherMod.meats;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.animals
{
    public static class AnimalExtension
    {
        public static Meat GetMeat(this Animal value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute = System.Attribute.GetCustomAttribute(field, typeof(MeatAttribute)) as MeatAttribute;

            return attribute.Meat;
        }
    }
}
