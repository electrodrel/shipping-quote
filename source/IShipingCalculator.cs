using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShippingQuote
{
    public interface IShippingCalculator
    {
        decimal FindBestShippingDeal(string fromAddress, string toAddress, List<Measurement> dimensions);
    }
}