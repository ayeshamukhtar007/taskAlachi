using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CommonClass
    {
        public event EventHandler<string> Common;
        public void InvokeMethod(string st)
        {
            Console.WriteLine("Common here "+st);
            Common?.Invoke(this, st);
        }
    }
}
