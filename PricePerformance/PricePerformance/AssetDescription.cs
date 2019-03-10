using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PricePerformance
{
    public class AssetDescription
    {
        public int appid { get; set; }
        public string classid { get; set; }
        public string instanceid { get; set; }
        public string background_color { get; set; }
        public string icon_url { get; set; }
        public int tradable { get; set; }
        public string name { get; set; }
        public string name_color { get; set; }
        public string type { get; set; }
        public string market_name { get; set; }
        public string market_hash_name { get; set; }
    }
}
