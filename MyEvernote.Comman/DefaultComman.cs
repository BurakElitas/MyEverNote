using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Comman
{
    public class DefaultComman:IComman
    {
        public string Username()
        {
            return "system";
        }
    }
}
