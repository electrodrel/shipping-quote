using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using ShippingQuote.Api;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShippingQuote
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            try
            {
                var shippingCalculator = host.Services.GetRequiredService<IShippingCalculator>();
                // in reality this will be a web call to our host or a message bus event to handle
                var result = shippingCalculator.FindBestShippingDeal(new AddressInfo("Barnaby", "BC", "Canada"), new AddressInfo("Winnipeg", "MB", "Canada"), new List<Measurement>() { new Measurement(10, 10, 20, MeasurementSystem.MetricCentimetre) });
                Console.WriteLine(result);

                await host.RunAsync();
            }
            catch (System.Exception e)
            {
                logger.LogError(e, e.Message);
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .AddCommandLine(args)
             .Build();

            return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => 
            {
                builder.Sources.Clear();
                builder.AddConfiguration(configuration);
            })
            .ConfigureServices((hostContext, services) => 
            {
                services.Configure<Carriers>(configuration.GetSection("Carriers"));
                services.AddTransient<IShippingCalculator, ShippingCalculator>();

                services.AddHttpClient<IShippingApiService<Fedex>, FedexService>(client =>
                {
                    client.BaseAddress = new Uri(configuration["Carriers:Fedex:BaseUrl"]);
                }).AddPolicyHandler(GetRetryPolicy());

                services.AddHttpClient<IShippingApiService<CanadaPost>, CanadaPostService>(client =>
                {
                    client.BaseAddress = new Uri(configuration["Carriers:CanadaPost:BaseUrl"]);
                }).AddPolicyHandler(GetRetryPolicy());

                services.AddHttpClient<IShippingApiService<Ups>, UpsService>(client =>
                {
                    client.BaseAddress = new Uri(configuration["Carriers:Ups:BaseUrl"]);
                }).AddPolicyHandler(GetRetryPolicy());
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return (IAsyncPolicy<HttpResponseMessage>)HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}