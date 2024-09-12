using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using Stock_Price_Service.Repositories.Cache;
using Stock_Price_Service.Repositories.Persistence;
using Stock_Price_Service.Services;



namespace Stock_Price_Service;
internal static class Program
{
    private static void Main(string[] args)
    {
        var host = new HostBuilder()
             .ConfigureFunctionsWebApplication(worker => worker.UseNewtonsoftJson())
             .ConfigureOpenApi()
             .ConfigureServices(ConfigureServices)
             .Build();

        host.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        ConfigureContext(services);
        ConfigureServicesForDependencyInjection(services);
    }


    private static void ConfigureContext(IServiceCollection services)
    {
        //Redis cache
        string redisConnectionString = Environment.GetEnvironmentVariable("RedisConnection") ?? throw new ArgumentException("Please make sure env variable is set.");
        var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
        services.AddSingleton<IConnectionMultiplexer>(redisConnection);

        //Service bus queue
        string serviceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnection") ?? throw new ArgumentException("Please make sure ServiceBusConnection is set.");
        var serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
        services.AddSingleton(serviceBusClient);

    }
    private static void ConfigureServicesForDependencyInjection(IServiceCollection services)
    {
        services.AddScoped<IStockPriceCacheRepository, StockPriceCacheRepository>();
        services.AddScoped<IStockPriceRepository, StockPriceRepository>();
        services.AddScoped<IStockPriceService, StockPriceService>();
        services.AddScoped<IBroadcastService, BroadcastService>();
    }
}