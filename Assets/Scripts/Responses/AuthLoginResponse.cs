using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Responses
{
    public class AuthLoginResponse
    {
        public int ok;
        public string _id;
        public Steam steam;
        public DateTime registeredDate;
        public string username;
    }

    public class Steam
    {
        public string steamid;
        public string personaname;
        public string profileurl;
    }

}
