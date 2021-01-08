using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerFrameworkMod
{
    public class RestrictionException : Exception
    {
        public bool ShowMessage;

        public RestrictionException()
        {
            ShowMessage = false;
        }

        public RestrictionException(string message) : base(message)
        {
            ShowMessage = true;
        }
    }
}
