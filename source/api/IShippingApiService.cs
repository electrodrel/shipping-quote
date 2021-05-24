using System.Threading.Tasks;

namespace ShippingQuote.Api
{
    public interface IShippingApiService<in T>
    {
        Task<decimal> GetTotal(T requestData);
    }
}