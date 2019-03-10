using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace PricePerformance
{
    public static class Web
    {
        public static WebClient Client { get; private set; } = new WebClient();

        public static void Initialize()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            Client.Proxy = null;
        }

        public static string MakeRequest(string url)
        {
            string output = "";

            try
            {
                output = Client.DownloadString(url);
            }
            catch { }

            return output;
        }
    }
}
