using Azure.Messaging.ServiceBus;
using Stock_Price_Service.Helpers;
using Stock_Price_Service.Models;
using Stock_Price_Service.Repositories.Cache;
using Stock_Price_Service.Repositories.Persistence;
using System.Text.Json;

namespace Stock_Price_Service.Services;

public class StockPriceService(IStockPriceCacheRepository _stockPriceCacheRepository, IStockPriceRepository _stockPriceRepository, ServiceBusClient _serviceBusClient) : IStockPriceService
{
    public async Task GenerateAndStoreStockPricesAsync()
    {
        var stockPrices = await _stockPriceCacheRepository.GetAll();
        foreach (var stockPrice in stockPrices)
        {
            double newPrice = GenerateStockPrice.GenerateRandom();
            await _stockPriceCacheRepository.SetStockPriceAsync(new StockPrice { Price = newPrice, Ticker = stockPrice.Ticker });
        }
    }

    public async Task UpdateAsync(StockPrice stockPrice)
    {
        await _stockPriceCacheRepository.SetStockPriceAsync(stockPrice);
    }

    public async Task<IEnumerable<StockPrice>> GetCachedByTickersAsync(List<string> tickers)
    {
        await _stockPriceCacheRepository.ClearAllCache();
        var stockPrices = await _stockPriceCacheRepository.GetAllByTickers(tickers);
        return stockPrices;
    }

    public async Task<IEnumerable<StockPrice>> GetAllCachedAsync()
    {
        var stockPrices = await _stockPriceCacheRepository.GetAll();
        return stockPrices;
    }

    public async Task<IEnumerable<StockPrice>> GetByTickersAsync(List<string> tickers)
    {
        var stockPrices = await _stockPriceRepository.GetAllByTickers(tickers);
        return stockPrices;
    }


    public async Task EnqueueTickersAsync(List<string> tickers)
    {
        var sender = _serviceBusClient.CreateSender("stockprice-requests-queue");
        var message = new ServiceBusMessage(JsonSerializer.Serialize(tickers));

        // Send the message to the Service Bus queue
        await sender.SendMessageAsync(message);
    }
}
