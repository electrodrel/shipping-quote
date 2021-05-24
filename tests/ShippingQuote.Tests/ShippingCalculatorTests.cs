using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ShippingQuote.Api;
using Xunit;

namespace ShippingQuote.Tests
{
    public class ShippingCalculatorTests
    {
        private readonly Faker _fake;
        private readonly IOptions<Carriers> _carriers;
        private readonly Mock<ILogger<ShippingCalculator>> _loggerMock;        
        private readonly Mock<IShippingApiService<Fedex>> _fedexServiceMock;
        private readonly Mock<IShippingApiService<CanadaPost>> _canadaPostServiceMock;
        private readonly Mock<IShippingApiService<Ups>> _upsServiceMock;

        private readonly ShippingCalculator _subject;

        public ShippingCalculatorTests()
        {
            this._fake = new Faker();
            
            this._carriers = Options.Create<Carriers>(GetCarriers());
            this._loggerMock = new Mock<ILogger<ShippingCalculator>>();            
            this._fedexServiceMock = new Mock<IShippingApiService<Fedex>>();
            this._canadaPostServiceMock = new Mock<IShippingApiService<CanadaPost>>();
            this._upsServiceMock = new Mock<IShippingApiService<Ups>>();
            this._subject = new ShippingCalculator(this._loggerMock.Object, this._carriers, this._fedexServiceMock.Object, this._canadaPostServiceMock.Object, this._upsServiceMock.Object);
        }

        [Fact]
        public void FindBestShippingDeal_WhenFromAddressIsNull_ThrowsException()
        {
            AddressInfo fromAddress = null;
            var toAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var measurements = new List<Measurement>() { new Measurement(this._fake.Random.Float(), this._fake.Random.Float(), this._fake.Random.Float(), MeasurementSystem.ImperialFeet) };
            Assert.Throws<ArgumentNullException>(() => this._subject.FindBestShippingDeal(fromAddress, toAddress, measurements));
        }

        [Fact]
        public void FindBestShippingDeal_WhenToAddressIsNull_ThrowsException()
        {
            var fromAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            AddressInfo toAddress = null;
            var measurements = new List<Measurement>() { new Measurement(this._fake.Random.Float(), this._fake.Random.Float(), this._fake.Random.Float(), MeasurementSystem.ImperialFeet) };
            Assert.Throws<ArgumentNullException>(() => this._subject.FindBestShippingDeal(fromAddress, toAddress, measurements));
        }

        [Fact]
        public void FindBestShippingDeal_WhenDimensionsAreNull_ThrowsException()
        {
            var fromAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var toAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            List<Measurement> measurements = null;
            Assert.Throws<ArgumentNullException>(() => this._subject.FindBestShippingDeal(fromAddress, toAddress, measurements));
        }

        [Fact]
        public void FindBestShippingDeal_WhenDimensionsAreEmpty_ThrowsException()
        {
            var fromAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var toAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var measurements = new List<Measurement>();
            Assert.Throws<ArgumentException>(() => this._subject.FindBestShippingDeal(fromAddress, toAddress, measurements));
        }

        [Fact]
        public void FindBestShippingDeal_WhenFromAddressIsEmpty_ThrowsException()
        {
            var fromAddress = new AddressInfo(string.Empty, null, string.Empty);
            var toAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var measurements = new List<Measurement>() { new Measurement(this._fake.Random.Float(), this._fake.Random.Float(), this._fake.Random.Float(), MeasurementSystem.ImperialFeet) };
            Assert.Throws<ArgumentException>(() => this._subject.FindBestShippingDeal(fromAddress, toAddress, measurements));
        }

        [Fact]
        public void FindBestShippingDeal_WhenToAddressIsEmpty_ThrowsException()
        {
            var fromAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var toAddress = new AddressInfo(string.Empty, string.Empty, null);
            var measurements = new List<Measurement>() { new Measurement(this._fake.Random.Float(), this._fake.Random.Float(), this._fake.Random.Float(), MeasurementSystem.ImperialFeet) };
            Assert.Throws<ArgumentException>(() => this._subject.FindBestShippingDeal(fromAddress, toAddress, measurements));
        }

        [Fact]
        public void FindBestShippingDeal_WhenTaskApiFailed_LogsError()
        {
            this._fedexServiceMock.Setup(m => m.GetTotal(It.IsAny<Fedex>())).Returns(Task.FromResult(this._fake.Random.Decimal()));
            this._canadaPostServiceMock.Setup(m => m.GetTotal(It.IsAny<CanadaPost>())).Returns(Task.FromResult(this._fake.Random.Decimal()));
            this._upsServiceMock.Setup(m => m.GetTotal(It.IsAny<Ups>())).ThrowsAsync(new HttpRequestException("Bad Request"));

            var fromAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var toAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var measurements = new List<Measurement>() { new Measurement(this._fake.Random.Float(), this._fake.Random.Float(), this._fake.Random.Float(), MeasurementSystem.ImperialFeet) };

            this._subject.FindBestShippingDeal(fromAddress, toAddress, measurements);

            this._loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<HttpRequestException>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void FindBestShippingDeal_WhenTaskParsingFailed_LogsError()
        {
            this._fedexServiceMock.Setup(m => m.GetTotal(It.IsAny<Fedex>())).ThrowsAsync(new JsonException("content does not represent a valid single JSON value"));
            this._canadaPostServiceMock.Setup(m => m.GetTotal(It.IsAny<CanadaPost>())).Returns(Task.FromResult(this._fake.Random.Decimal()));
            this._upsServiceMock.Setup(m => m.GetTotal(It.IsAny<Ups>())).Returns(Task.FromResult(this._fake.Random.Decimal()));

            var fromAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var toAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var measurements = new List<Measurement>() { new Measurement(this._fake.Random.Float(), this._fake.Random.Float(), this._fake.Random.Float(), MeasurementSystem.ImperialFeet) };

            this._subject.FindBestShippingDeal(fromAddress, toAddress, measurements);

            this._loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<JsonException>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void FindBestShippingDeal_WhenMultipleTaskFailed_LogsErrors()
        {
            this._fedexServiceMock.Setup(m => m.GetTotal(It.IsAny<Fedex>())).ThrowsAsync(new JsonException("content does not represent a valid single JSON value"));
            this._canadaPostServiceMock.Setup(m => m.GetTotal(It.IsAny<CanadaPost>())).Returns(Task.FromResult(this._fake.Random.Decimal()));
            this._upsServiceMock.Setup(m => m.GetTotal(It.IsAny<Ups>())).ThrowsAsync(new HttpRequestException("Bad Request"));

            var fromAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var toAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var measurements = new List<Measurement>() { new Measurement(this._fake.Random.Float(), this._fake.Random.Float(), this._fake.Random.Float(), MeasurementSystem.ImperialFeet) };

            this._subject.FindBestShippingDeal(fromAddress, toAddress, measurements);

            this._loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(2));
        }

        [Fact]
        public void FindBestShippingDeal_WhenAllTaskFailed_LogsCriticalError()
        {
            this._fedexServiceMock.Setup(m => m.GetTotal(It.IsAny<Fedex>())).ThrowsAsync(new JsonException("content does not represent a valid single JSON value"));
            this._canadaPostServiceMock.Setup(m => m.GetTotal(It.IsAny<CanadaPost>())).ThrowsAsync(new FormatException("The value cannot be represented as a Decimal"));
            this._upsServiceMock.Setup(m => m.GetTotal(It.IsAny<Ups>())).ThrowsAsync(new HttpRequestException("Bad Request"));

            var fromAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var toAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var measurements = new List<Measurement>() { new Measurement(this._fake.Random.Float(), this._fake.Random.Float(), this._fake.Random.Float(), MeasurementSystem.ImperialFeet) };

            this._subject.FindBestShippingDeal(fromAddress, toAddress, measurements);

            this._loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<JsonException>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
            this._loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<FormatException>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
            this._loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<HttpRequestException>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);

            this._loggerMock.Verify(x => x.Log(LogLevel.Critical, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void FindBestShippingDeal_WhenAllTasksSucceed_Returns_CorrectCarrierName()
        {
            var fedexQuote = 5m;
            var canadaPostQuote = 10m;
            var upsQuote = 15m;

            this._fedexServiceMock.Setup(m => m.GetTotal(It.IsAny<Fedex>())).Returns(Task.FromResult(fedexQuote));
            this._canadaPostServiceMock.Setup(m => m.GetTotal(It.IsAny<CanadaPost>())).Returns(Task.FromResult(canadaPostQuote));
            this._upsServiceMock.Setup(m => m.GetTotal(It.IsAny<Ups>())).Returns(Task.FromResult(upsQuote));

            var fromAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var toAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var measurements = new List<Measurement>() { new Measurement(this._fake.Random.Float(), this._fake.Random.Float(), this._fake.Random.Float(), MeasurementSystem.ImperialFeet) };

            var result = this._subject.FindBestShippingDeal(fromAddress, toAddress, measurements);

            Assert.Equal(fedexQuote, result.Value);
            Assert.Equal("fedex", result.Key);
        }

        [Fact]
        public void FindBestShippingDeal_WhenAllTasksSucceed_Returns_Min()
        {
            var fedexQuote = this._fake.Random.Decimal();
            var canadaPostQuote = this._fake.Random.Decimal();
            var upsQuote = this._fake.Random.Decimal();

            this._fedexServiceMock.Setup(m => m.GetTotal(It.IsAny<Fedex>())).Returns(Task.FromResult(fedexQuote));
            this._canadaPostServiceMock.Setup(m => m.GetTotal(It.IsAny<CanadaPost>())).Returns(Task.FromResult(canadaPostQuote));
            this._upsServiceMock.Setup(m => m.GetTotal(It.IsAny<Ups>())).Returns(Task.FromResult(upsQuote));

            var fromAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var toAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var measurements = new List<Measurement>() { new Measurement(this._fake.Random.Float(), this._fake.Random.Float(), this._fake.Random.Float(), MeasurementSystem.ImperialFeet) };

            var result = this._subject.FindBestShippingDeal(fromAddress, toAddress, measurements);

            Assert.Equal(Math.Min(Math.Min(fedexQuote, canadaPostQuote), upsQuote), result.Value);
        }

        [Fact]
        public void FindBestShippingDeal_WhenTaskFailed_ReturnsAvailableResults_Min()
        {
            var fedexQuote = this._fake.Random.Decimal();
            var canadaPostQuote = this._fake.Random.Decimal();

            this._fedexServiceMock.Setup(m => m.GetTotal(It.IsAny<Fedex>())).Returns(Task.FromResult(fedexQuote));
            this._canadaPostServiceMock.Setup(m => m.GetTotal(It.IsAny<CanadaPost>())).Returns(Task.FromResult(canadaPostQuote));
            this._upsServiceMock.Setup(m => m.GetTotal(It.IsAny<Ups>())).ThrowsAsync(new HttpRequestException("Bad Request"));

            var fromAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var toAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var measurements = new List<Measurement>() { new Measurement(this._fake.Random.Float(), this._fake.Random.Float(), this._fake.Random.Float(), MeasurementSystem.ImperialFeet) };

            var result = this._subject.FindBestShippingDeal(fromAddress, toAddress, measurements);

            Assert.Equal(Math.Min(fedexQuote, canadaPostQuote), result.Value);
        }

        [Fact]
        public void FindBestShippingDeal_WhenAllTasksFailed_Returns_NegativeOne()
        {
            this._fedexServiceMock.Setup(m => m.GetTotal(It.IsAny<Fedex>())).ThrowsAsync(new JsonException("content does not represent a valid single JSON value"));
            this._canadaPostServiceMock.Setup(m => m.GetTotal(It.IsAny<CanadaPost>())).ThrowsAsync(new FormatException("The value cannot be represented as a Decimal"));
            this._upsServiceMock.Setup(m => m.GetTotal(It.IsAny<Ups>())).ThrowsAsync(new HttpRequestException("Bad Request"));

            var fromAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var toAddress = new AddressInfo(this._fake.Address.City(), this._fake.Address.State(), this._fake.Address.Country());
            var measurements = new List<Measurement>() { new Measurement(this._fake.Random.Float(), this._fake.Random.Float(), this._fake.Random.Float(), MeasurementSystem.ImperialFeet) };

            var result = this._subject.FindBestShippingDeal(fromAddress, toAddress, measurements);

            Assert.Equal(-1, result.Value);
            Assert.Empty(result.Key);
        }

        private Carriers GetCarriers()
        {
            var carriers = new Carriers();
            carriers.Fedex = new CarrierInfo() 
            { 
                BaseUrl = new Uri(this._fake.Internet.Url()),
                MeasurementSystem = MeasurementSystem.MetricMetres
            };
            carriers.CanadaPost = new CarrierInfo()
            {
                BaseUrl = new Uri(this._fake.Internet.Url()),
                MeasurementSystem = MeasurementSystem.MetricMetres
            };
            carriers.Ups = new CarrierInfo()
            {
                BaseUrl = new Uri(this._fake.Internet.Url()),
                MeasurementSystem = MeasurementSystem.MetricMetres
            };
            return carriers;
        }
    }
}
