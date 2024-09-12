using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Stock_Price_Service.Services;

public class BroadcastService(IStockPriceService stockPriceService, ILogger<BroadcastService> logger ) : IBroadcastService
{
    public async Task<SignalRMessageAction> BroadcastStockPrices()
    {
        var stockPrices = await stockPriceService.GetAllCachedAsync();

        var stockPriceMessages = stockPrices.Select(stock => new
        {
            stock.Ticker,
            stock.Price
        }).ToList();

        logger.LogInformation("Broadcasting stock prices to all clients.");

        // Broadcast the stock prices to all connected clients
        return new SignalRMessageAction("stockPriceUpdated")
        {
            Arguments = new[] { stockPriceMessages }
        };
    }
}
