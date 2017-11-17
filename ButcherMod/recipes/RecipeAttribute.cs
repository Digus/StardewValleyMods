using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.cooking
{
    internal class RecipeAttribute : Attribute
    {
        public string Recipe;

        public RecipeAttribute(string recipe)
        {
            this.Recipe = recipe;
        }
    }
}
