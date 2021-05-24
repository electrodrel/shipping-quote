## Getting Started

These instructions will get you a copy of the project up and running on your local machine for demo purposes.

### Prerequisites

Make sure you have .Net 5.0 installed on your local machine 

## General Comments

I could have written a simple Console App with no `IHost` running in the back, but I wanted to make use of the build-in IHost dependency injection as well as the ConfigurationBuilder.
Also, in a real application, such subsystem most likely won't be a exe console app, it would be a win service processing incoming messages to find the best deal. It can techinically be a library, but typically there is still a system between client library and external APIs (an internal API or asynchronous messaging system)

From the assigniment it wasn't clear what the format of each box dimension is. I parse it to the simple float array for two JSON Api, and a `Package` object for the third one.
Same applies to the addresses. It seems like warehouse and desitination addresses are simple strings and that's what I am parsing to.

Conversion between the measurement systems is rather awkward (I could have saved some switch cases) and there is a room for refactoring

I didn't include authorization headers when calling external APIs. Typically, there is an individual `Authorize` endpoint that accepts username and password and returns an API key. That API key is included with every subsequent request.
However, there were no mentions of such endpoints for any of the APIs in the assignment. That would mean that either the APIs are public or they require basic authentication handler on each request. 
If the latter is the case, then we just need to 
1. Add username&password to each of the carriers in appsettings.json
2. When configuring each API service class's `httpClient` in `IHostBuilder.ConfigureServices`, add the following:
```
var byteArray = new UTF8Encoding().GetBytes($"{Carriers:<Name of the Carrier>:UserName}:{Carriers:<Name of the Carrier>:Password}");
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
```

I added retry policy with exponential backoff for calling external APIs. Each subsequent retry is exponentially longer than the previous one, up to 3 retires. Retry logic can be simply configured per API (in case we don't want to retry for some APIs)

MeasurementSystem unit tests are for representational purposes only. Adjusting all the values for different conversion is exhausting and not necessary for a demo project.

There are no unit tests for `IShippingApiService<T>`. I think they require integration tests with calls to the actual APIs, not unit tests. I could have mock `HttpMessageHandler` of `HttpClient`, but I don't see any value in it, because all the logic in those services is to parse incoming data, to communicate with external APIs and then to parse the received data.

## Built With

* [Polly](https://github.com/App-vNext/Polly) - .NET resilience and transient-fault-handling library
* [XUnit](https://xunit.net/) - Test framework
* [Moq](https://github.com/Moq/) - Mocking framework
* [Bogus](https://github.com/bchavez/Bogus) - Fake data generator
