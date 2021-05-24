using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShippingQuote
{
    public interface IShippingCalculator
    {
        KeyValuePair<string, decimal> FindBestShippingDeal(AddressInfo fromAddress, AddressInfo toAddress, List<Measurement> dimensions);
    }
}