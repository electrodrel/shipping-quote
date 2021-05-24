using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShippingQuote.Api;

namespace ShippingQuote
{
    public class ShippingCalculator : IShippingCalculator
    {
        public IShippingApiService<Fedex> FedexService { get; }
        public IShippingApiService<CanadaPost> CanadaPostService { get; } 
        public IShippingApiService<Ups> UpsService { get; }

        private readonly ILogger<ShippingCalculator> _logger;

        public Carriers Carriers { get; }

        public ShippingCalculator(ILogger<ShippingCalculator> logger, IOptions<Carriers> carriers, IShippingApiService<Fedex> fedexService, IShippingApiService<CanadaPost> canadaPostService, IShippingApiService<Ups> upsService)
        {
            this._logger = logger;
            this.Carriers = carriers.Value;
            this.FedexService = fedexService;
            this.CanadaPostService = canadaPostService;
            this.UpsService = upsService;
        }

        public KeyValuePair<string, decimal> FindBestShippingDeal(AddressInfo fromAddress, AddressInfo toAddress, List<Measurement> measurements)
        {
            if (fromAddress == null || toAddress == null || measurements == null)
            {
                throw new ArgumentNullException("Invalid input data. From and To addresses and Measurements should not be null.");
            }

            if (!measurements.Any())
            {
                throw new ArgumentException("At least one package measurement is expected");
            }
            // should exlude negative measurements?

            var fullFromAddress = fromAddress.GetFullAddress();
            var fullToAddress = toAddress.GetFullAddress();

            if (string.IsNullOrWhiteSpace(fullFromAddress) || string.IsNullOrWhiteSpace(fullToAddress))
            {
                throw new ArgumentException("Invaid input data. From or To addresses are empty");
            }
            // in reality the key should be an ID of the carrier, not a string name
            var tasks = new Dictionary<string, Task<decimal>>();
            //fedex
            var dimensionsInFedexMeasurementSystem = measurements.Select(d => d.ConvertTo(this.Carriers.Fedex.MeasurementSystem));
            var fedexTask = this.FedexService.GetTotal(new Fedex(fullFromAddress, fullToAddress, dimensionsInFedexMeasurementSystem.Select(d => new float[] { d.Height, d.Width, d.Length }).ToList()));
            tasks.Add("fedex", fedexTask);
            //canadaPost
            var dimensionsInCanadaPostMeasurementSystem = measurements.Select(d => d.ConvertTo(this.Carriers.CanadaPost.MeasurementSystem));
            var canadaPostTask = this.CanadaPostService.GetTotal(new CanadaPost(fullFromAddress, fullToAddress, dimensionsInCanadaPostMeasurementSystem.Select(d => new float[] { d.Height, d.Width, d.Length }).ToList()));
            tasks.Add("canadaPost", canadaPostTask);
            //UPS
            var dimensionsInUpsMeasurementSystem = measurements.Select(d => d.ConvertTo(this.Carriers.Ups.MeasurementSystem));
            var upsTask = this.UpsService.GetTotal(new Ups(fullFromAddress, fullToAddress, dimensionsInUpsMeasurementSystem.Select(d => new Package(d.Height, d.Width, d.Length)).ToList()));
            tasks.Add("ups", upsTask);
            
            try
            {
                Task.WaitAll(tasks.Values.ToArray());
            }
            catch (AggregateException e)
            {
                for (int i = 0; i < e.InnerExceptions.Count; i++)
                {
                    _logger.LogError(e.InnerExceptions[i], "Failed to call one of the external API");
                }
            }
            var results = tasks.Where(t => t.Value.IsCompletedSuccessfully).Select(t => new KeyValuePair<string, decimal>(t.Key, t.Value.Result));
            if (results.Any())
            {
                return results.OrderBy(kvp => kvp.Value).First();
            }
            _logger.LogCritical("Not a single API returned Success. The best deal is -1$");
            return new KeyValuePair<string, decimal>(string.Empty, -1);
        }
    }
}