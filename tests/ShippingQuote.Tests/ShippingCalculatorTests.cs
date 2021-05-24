using System;
using Bogus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ShippingQuote.Api;
using Xunit;

namespace ShippingQuote.Tests
{
    [UnitTest]
    public class ShippingCalculatorTests
    {
        private readonly Faker _fake;
        private readonly IOptions<Carriers> _carriers;
        private readonly Mock<ILogger<ShippingCalculator>> _loggerMock;        
        private readonly Mock<IShippingApiService<Fedex>> _fedexService;
        private readonly Mock<IShippingApiService<CanadaPost>> _canadaPostService;
        private readonly Mock<IShippingApiService<Ups>> _upsService;

        private readonly ShippingCalculator _subject;

        public ShippingCalculatorTests()
        {
            this._fake = new Faker();
            
            this._carriers = Options.Create<Carriers>(GetCarriers());
            this._loggerMock = new Mock<ILogger<ShippingCalculator>>();            
            this._fedexService = new Mock<IShippingApiService<Fedex>>();
            this._canadaPostService = new Mock<IShippingApiService<CanadaPost>>();
            this._upsService = new Mock<IShippingApiService<Ups>>();
            this._subject = new ShippingCalculator(this._loggerMock.Object, this._carriers, this._fedexService.Object, this._canadaPostService.Object, this._upsService.Object);
        }

        [Fact]
        public void Test1()
        {

        }

        private Carriers GetCarriers()
        {
            var carriers = new Carriers();
            carriers.Fedex = new CarrierInfo() 
            { 
                BaseUrl = new Uri(this._fake.Internet.Url()),
                MeasurementSystem = MeasurementSystem.MetricMetres
            }
            carriers.CanadaPost = new CarrierInfo()
            {
                BaseUrl = new Uri(this._fake.Internet.Url()),
                MeasurementSystem = MeasurementSystem.MetricMetres
            }
            
            carriers.CanadaPost = new CarrierInfo()
            {
                BaseUrl = new Uri(this._fake.Internet.Url()),
                MeasurementSystem = MeasurementSystem.MetricMetres
            }

            return carriers;
        }
    }
}
