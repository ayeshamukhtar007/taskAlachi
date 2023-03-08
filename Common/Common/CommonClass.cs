using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public  delegate void Edelegate(string st);
    public static class CommonClass
    {
        public static event Edelegate Common;
        public static void InvokeMethod(string st)
        {
            Console.WriteLine("Common here "+st);
            Common?.Invoke(st);
        }
    }
}
