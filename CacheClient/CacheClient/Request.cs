using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheClient
{
   
    public  class Request
    {
        public string Operation { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }

    }
}
