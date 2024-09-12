using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Stock_Price_Service.Helpers;
using Stock_Price_Service.Services;
using System.Text.Json;

namespace Stock_Price_Service.Functions.ServiceBus;

public class ProcessStockPriceQueueFunction(ILogger<ProcessStockPriceQueueFunction> _logger, IStockPriceService _stockPriceService)
{
    [Function(nameof(ProcessStockPriceQueueFunction))]
    public async Task Run(
        [ServiceBusTrigger("stockprice-requests-queue", Connection = "ServiceBusConnection")]
        string queueMessage)
    {
        _logger.LogInformation($"Processing stock price request: {queueMessage}");

        try
        {
            var tickers = JsonSerializer.Deserialize<List<string>>(queueMessage);
            if (tickers is null || tickers.Count == 0)
            {
                _logger.LogError("No valid tickers found in the queue message.");
                return;
            }
            var stockPrices = await _stockPriceService.GetByTickersAsync(tickers);
            foreach (var stockPrice in stockPrices)
            {
                await _stockPriceService.UpdateAsync(stockPrice);
                _logger.LogInformation($"Cached stock price: {stockPrice.Ticker} - {stockPrice.Price}");

            }
        }
        catch (Exception ex)
        {
            HttpResponseMessageHelper.HandleException(ex, _logger);
        }
    }
}
