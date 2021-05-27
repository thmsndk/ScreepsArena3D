using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScreepsArenaApi.Responses
{
    public class ArenaListResponse
    {
        public int ok { get; set; }
        public Arena[] arenas { get; set; }
    }

    public class Arena
    {
        public string _id { get; set; }
        public string name { get; set; }
        public bool advanced { get; set; }
        public int rating { get; set; }
        public int rank { get; set; }
        public int games { get; set; }
        public bool qualifying { get; set; }
    }

}
