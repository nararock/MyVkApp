using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVkApp.SerializationClass
{
    public class VKUserProfile
    {
        public string id;
        public string first_name;
        public string last_name;
        public bool is_closed;
        public string deactivated;
        public List<string> sourceID = [];
    }
}
