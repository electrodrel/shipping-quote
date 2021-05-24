
using System.Collections.Generic;

namespace ShippingQuote
{
    public class Fedex
    {
        public string Consignee { get; set;}
        public string Consignor {get; set;}
        public List<float[]> Cartons {get; set;}

        public Fedex(string consignee, string consignor, List<float[]> cartons)
        {
            this.Consignee = consignee;
            this.Consignor = consignor;
            this.Cartons = cartons;
        }
    }
}