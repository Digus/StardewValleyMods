using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailFrameworkMod.APIs
{
    public interface IQuestApi
    {
        /// <summary>
        /// Returns an quest id of managed quest
        /// </summary>
        /// <param name="fullQuestName">A fullqualified name of quest (questName@youdid)</param>
        /// <returns>
        /// Quest id if the quest with <param>fullQuestName</param> exists and it's managed, otherwise returns -1
        /// </returns>
        int ResolveQuestId(string fullQuestName);
    }
}
