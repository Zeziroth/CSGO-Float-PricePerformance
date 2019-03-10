using System.Collections.Generic;

namespace PricePerformance
{
    public class Asset
    {
        public int currency { get; set; }
        public int appid { get; set; }
        public string contextid { get; set; }
        public string id { get; set; }
        public string amount { get; set; }
        public IList<MarketAction> market_actions { get; set; }
    }
}
