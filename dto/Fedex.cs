
using System.Collections.Generic;

namespace ShippingQuote
{
    public class Fedex
    {
        public string Consignee { get; set;}
        public string Consignor {get; set;}
        public List<Package> Cartons {get; set;}

        public Fedex(string consignee, string consignor, List<Package> cartons)
        {
            this.Consignee = consignee;
            this.Consignor = consignor;
            this.Cartons = cartons;
        }
    }
}