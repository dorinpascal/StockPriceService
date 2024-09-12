
using Microsoft.Azure.Functions.Worker;

namespace Stock_Price_Service.Services;

/// <summary>
/// Interface for a service responsible for broadcasting stock prices to all connected SignalR clients.
/// </summary>
public interface IBroadcastService
{
    /// <summary>
    /// Broadcasts stock prices to all connected SignalR clients.
    /// Retrieves the latest stock prices from the cache and sends them as a real-time message to all clients subscribed to the "stockPriceUpdated" event.
    /// </summary>
    /// <returns>A <see cref="SignalRMessageAction"/> representing the action of broadcasting the stock prices to the clients.</returns>
    Task<SignalRMessageAction> BroadcastStockPrices();
}