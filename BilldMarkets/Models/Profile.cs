using System;
using System.Collections.Generic;
using System.Text;

namespace BilldMarkets.Models
{
    class Profile
    {
        public List<Stock> Stocks { get; set; }
        public List<Crypto> Cryptos { get; set; }

        public Profile()
        {
            Stocks = new List<Stock>();
            Cryptos = new List<Crypto>();
        }
    }

    public class Stock : Asset
    {
        public decimal Open { get; set; }
    }

    public class Crypto : Asset
    {
        public string Address { get; set; }
    }

    public class Asset
    {
        public string Name { get; set; }
        public decimal LastValue { get; set; }
        public decimal NewValue { get; set; }
        public bool Long { get; set; }
        public decimal AveragePrice { get; set; }
    }
}
