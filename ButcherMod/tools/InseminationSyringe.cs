using CustomElementHandler;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherMod.tools
{
    public class InseminationSyringe : Tool, ISaveElement
    {
        private FarmAnimal _animal;

        public new static int initialParentTileIndex = 546;
        public new static int indexOfMenuItemView = 546;

        public InseminationSyringe() : base("Insemination Syringe", -1, initialParentTileIndex, indexOfMenuItemView, false, 1)
        {

        }

        protected override string loadDisplayName()
        {
            return DataLoader.i18n.Get("Tool.InseminationSyringe.Name");
        }

        protected override string loadDescription()
        {
            return DataLoader.i18n.Get("Tool.InseminationSyringe.Description");
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            throw new NotImplementedException();
        }

        public object getReplacement()
        {
            throw new NotImplementedException();
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            throw new NotImplementedException();
        }
    }
}
