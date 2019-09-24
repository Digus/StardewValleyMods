using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals.data;
using static AnimalHusbandryMod.common.DataLoader;

namespace AnimalHusbandryMod.animals.events
{
    public abstract class AbstractAct : IAnimalContestAct
    {
        public abstract string NpcName { get; }
        public abstract string GetAct(AnimalContestItem animalContestInfo, List<AnimalContestItem> history);

        protected string TranslationKey(string keyPostFix)
        {
            return $"AnimalContest.Dialog.{NpcName}Act.{keyPostFix}";
        }

        protected string GetDialog(string keyPostFix)
        {
            return i18n.Get(TranslationKey(keyPostFix));
        }

        protected string GetDialog(string keyPostFix, object tokens)
        {
            return i18n.Get(TranslationKey(keyPostFix),tokens);
        }
    }
}
