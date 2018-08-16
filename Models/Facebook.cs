using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoSocialAPI.Models
{
    public class Facebook
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
    }

    public class FacebookData
    {
        public string category { get; set; }
        public string link { get; set; }
        public string name { get; set; }
        public string id { get; set; }

    }

}