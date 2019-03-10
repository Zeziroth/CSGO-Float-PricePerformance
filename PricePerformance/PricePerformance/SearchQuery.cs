using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PricePerformance
{

    public class SearchQuery
    {
        public bool success { get; set; }
        public int start { get; set; }
        public int pagesize { get; set; }
        public int total_count { get; set; }
        public Searchdata searchdata { get; set; }
        public List<Result> results { get; set; }
    }
}
