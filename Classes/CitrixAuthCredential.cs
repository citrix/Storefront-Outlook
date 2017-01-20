using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorefrontApps_Outlook.Classes
{
    public class CitrixAuthCredential
    {
        public string AuthToken { get; set; }
        public string SessionID { get; set; }
        public string CSRFToken { get; set; }
        public string CookiePath { get; set; }
        public string CookieHost { get; set; }
        public CitrixAuthCredential()
        {
            this.AuthToken = null;
            this.SessionID = null;
            this.CSRFToken = null;
            this.CookiePath = null;
        }
    }
}
