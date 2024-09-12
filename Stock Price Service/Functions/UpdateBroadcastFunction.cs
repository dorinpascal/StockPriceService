using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Stock_Price_Service.Services;

namespace Stock_Price_Service.Functions;

public class UpdateBroadcastFunction(IStockPriceService stockPriceService, IBroadcastService broadcastService, ILogger<UpdateBroadcastFunction> logger)
{
    [Function(nameof(UpdateStockPricesFunction))]
    [SignalROutput(HubName = "stockpricehub", ConnectionStringSetting = "AzureSignalRConnectionString")]
    public async Task<SignalRMessageAction> UpdateStockPricesFunction([TimerTrigger("*/5 * * * * *")] TimerInfo myTimer)
    {
        logger.LogInformation($"Timer trigger function executed at: {DateTime.Now}");
        // Simulate stock prices and update Redis
        await stockPriceService.GenerateAndStoreStockPricesAsync();
        logger.LogInformation("Stock prices updated successfully.");
        return await broadcastService.BroadcastStockPrices();
    }
}
