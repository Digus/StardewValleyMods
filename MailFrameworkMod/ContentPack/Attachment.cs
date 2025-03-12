using System.Collections.Generic;

namespace MailFrameworkMod.ContentPack
{
    public class Attachment
    {
        public ItemType Type;
        public string Index;
        public int? Stack;
        public int Quality;
        public string Name;
        public int? UpgradeLevel;
        public List<string> RequireMailReceived;
        public bool RequireAllMailReceived;
        public int ProbabilityWeight = 1;
        public string RandomGroup = "";
    }
}
