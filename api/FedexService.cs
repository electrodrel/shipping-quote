using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShippingQuote.Api
{
    public sealed class FedexService : IShippingApiService<Fedex>
    {
        private HttpClient _httpClient;
         
        public FedexService(HttpClient client)
        {            
            this._httpClient = client;
            this._httpClient.DefaultRequestHeaders
            .Accept
            .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<decimal> GetTotal(Fedex requestData)
        {
            var requestBody = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("shipping/quote", requestBody);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            using (JsonDocument document = JsonDocument.Parse(content))
            {
                var total = document.RootElement.GetProperty("total").GetDecimal();
                return total;
            }
        }
    }
}