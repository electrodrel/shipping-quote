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

        public decimal FindBestShippingDeal(string fromAddress, string toAddress, List<Measurement> dimensions)
        {
            var tasks = new List<Task<decimal>>();
            //fedex
            var fedexInfo = this.Carriers.Fedex;
            var dimensionsInFedexMeasurementSystem = dimensions.Select(d => d.ConvertTo(fedexInfo.MeasurementSystem));
            tasks.Add(this.FedexService.GetTotal(new Fedex(fromAddress, toAddress, dimensionsInFedexMeasurementSystem.Select(d => new Package(d.Height, d.Width, d.Length)).ToList())));
            //canadaPost
            var canadaPostInfo = this.Carriers.CanadaPost;
            var dimensionsInCanadaPostMeasurementSystem = dimensions.Select(d => d.ConvertTo(canadaPostInfo.MeasurementSystem));
            tasks.Add(this.CanadaPostService.GetTotal(new CanadaPost(fromAddress, toAddress, dimensionsInCanadaPostMeasurementSystem.Select(d => new Package(d.Height, d.Width, d.Length)).ToList())));
            //UPS
            var upsInfo = this.Carriers.Ups;
            var dimensionsInUpsMeasurementSystem = dimensions.Select(d => d.ConvertTo(upsInfo.MeasurementSystem));
            tasks.Add(this.UpsService.GetTotal(new Ups(fromAddress, toAddress, dimensionsInUpsMeasurementSystem.Select(d => new Package(d.Height, d.Width, d.Length)).ToList())));
            
            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException e)
            {
                for (int i = 0; i < e.InnerExceptions.Count; i++)
                {
                    _logger.LogError(e.InnerExceptions[i], "Failed to call one of the external API");
                }
            }
            var results = tasks.Where(t => t.IsCompletedSuccessfully).Select(t => t.Result).Select(r => r).ToList();
            if (results.Any())
            {
                return results.Min();
            }
            _logger.LogCritical("Not a single API returned Success. The best deal is -1$");
            return -1;
        }
    }
}