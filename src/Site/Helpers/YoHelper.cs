using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using TreeGecko.Library.Common.Helpers;

namespace Site.Helpers
{
    public static class YoHelper
    {
        public static void SendYo(string _username, string _link)
        {
            const string URL = "http://api.justyo.co/yo/";
            string key = Config.GetSettingValue("YoApiKey");

            NameValueCollection nvc = new NameValueCollection
            {
                {"api_token", key},
                {"username", _username},
                {"link", _link}
            };

            using (WebClient client = new WebClient())
            {
                var response = client.UploadValues(URL, "POST", nvc);

            }
            
        }


    }
}