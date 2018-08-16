using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoSocialAPI.Models
{
    public class LinkedIn
    {
        public string firstName { get; set; }
        public string headline { get; set; }
        public string id { get; set; }
        public string lastName { get; set; }
        public SiteStandardProfileRequest siteStandardProfileRequest { get; set; }
        
    }
    public class SiteStandardProfileRequest
    {
        public string url { get; set; }
    }
}