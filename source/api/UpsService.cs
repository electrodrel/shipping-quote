using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ShippingQuote.Api
{
    public sealed class UpsService : IShippingApiService<Ups>
    {
        private HttpClient _httpClient;

        public UpsService(HttpClient client)
        {
            this._httpClient = client;
            this._httpClient.DefaultRequestHeaders
            .Accept
            .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));
        }

        public async Task<decimal> GetTotal(Ups requestData)
        {
            var xml = GetXml(requestData);
            var requestBody = new StringContent(xml, Encoding.UTF8, "application/xml");
            var response = await _httpClient.PostAsync("delivery/price", requestBody);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = XDocument.Parse(content).Element("xml").Element("quote").Value;
            var quote = decimal.Parse(result);
            return quote;
        }

        private string GetXml(Ups requestData)
        {
            string xml = string.Empty;

            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(requestData.GetType());
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter))
                {
                    xmlSerializer.Serialize(xmlWriter, requestData);
                    xml = stringWriter.ToString();
                }
            }
            return xml;
        }
    }
}