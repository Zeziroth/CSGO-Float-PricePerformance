namespace PricePerformance
{
    public class Article
    {
        public string listingid { get; set; }
        public int price { get; set; }
        public int fee { get; set; }
        public int publisher_fee_app { get; set; }
        public string publisher_fee_percent { get; set; }
        public int currencyid { get; set; }
        public int steam_fee { get; set; }
        public int publisher_fee { get; set; }
        public int converted_price { get; set; }
        public int converted_fee { get; set; }
        public int converted_currencyid { get; set; }
        public int converted_steam_fee { get; set; }
        public int converted_publisher_fee { get; set; }
        public int converted_price_per_unit { get; set; }
        public int converted_fee_per_unit { get; set; }
        public int converted_steam_fee_per_unit { get; set; }
        public int converted_publisher_fee_per_unit { get; set; }
        public Asset asset { get; set; }
    }
}
