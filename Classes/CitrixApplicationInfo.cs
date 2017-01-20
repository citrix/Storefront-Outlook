using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StorefrontApps_Outlook.Classes
{
    public class CitrixApplicationInfo
    {
        public string ID { get; set; }
        public String AppTitle { get; set; }
        public String AppLaunchURL { get; set; }
        public String AppIconUrl { get; set; }
        public String AppDesc { get; set; }
		public byte[] AppIcon { get; set; }
		public CitrixAuthCredential Auth { get; set; }
		public string StorefrontURL { get; set; }
    }
}
