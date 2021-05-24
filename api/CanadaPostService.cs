using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShippingQuote.Api
{
    public sealed class CanadaPostService : IShippingApiService<CanadaPost>
    {
        private HttpClient _httpClient;
         
        public CanadaPostService(HttpClient client)
        {            
            this._httpClient = client;
            this._httpClient.DefaultRequestHeaders
            .Accept
            .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<decimal> GetTotal(CanadaPost requestData)
        {
            var requestBody = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("delivery/price", requestBody);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            using (JsonDocument document = JsonDocument.Parse(content))
            {
                var amount = document.RootElement.GetProperty("amount").GetDecimal();
                return amount;
            }
        }
    }
}