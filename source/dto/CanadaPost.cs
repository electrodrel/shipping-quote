
using System.Collections.Generic;

namespace ShippingQuote
{
    public class CanadaPost
    {
        public string ContactAddress { get; set;}
        public string WarehouseAddress {get; set;}
        public List<Package> PackageDimensions {get; set;}

        public CanadaPost(string contactAddress, string warehouseAddress, List<Package> packageDimensions)
        {
            this.ContactAddress = contactAddress;
            this.WarehouseAddress = warehouseAddress;
            this.PackageDimensions = packageDimensions;
        }
    }
}