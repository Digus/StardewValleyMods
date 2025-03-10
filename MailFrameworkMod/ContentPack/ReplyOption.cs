using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailFrameworkMod.ContentPack
{
    public class ReplyOption
    {
        public string ReplyKey;
        public string ReplyOptionDialog;
        public List<string> RequireMailReceived;
        public bool RequireAllMailReceived;
        public List<string> MailReceivedToAdd;
        public List<string> MailReceivedToRemove;
        public string ReplyResponseDialog;
        public int Cost = 0;
    }
}
