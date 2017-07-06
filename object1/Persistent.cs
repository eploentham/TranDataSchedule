using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tranDataSchedule.object1
{
    public class Persistent
    {
        public String table = "";
        public String pkField = "";
        public String sited = "";
        Random r = new Random();
        public String getGenID()
        {
            return r.Next().ToString();
        }
    }
}
