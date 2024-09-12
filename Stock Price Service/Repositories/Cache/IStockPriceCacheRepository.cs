using Stock_Price_Service.Models;

namespace Stock_Price_Service.Repositories.Cache;


/// <summary>
/// Interface for caching stock prices in a Redis data store.
/// Provides methods to interact with cached stock price data.
/// </summary>
public interface IStockPriceCacheRepository
{
    /// <summary>
    /// Retrieves the stock price for a specific ticker from the cache.
    /// </summary>
    /// <param name="ticker">The stock ticker symbol.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the stock price as a double, 
    /// or null if the ticker is not found in the cache.
    /// </returns>
    Task<double?> GetStockPriceAsync(string ticker);

    /// <summary>
    /// Caches a stock price for a specific ticker.
    /// </summary>
    /// <param name="stockPrice">The StockPrice object containing the ticker and price to cache.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetStockPriceAsync(StockPrice stockPrice);

    /// <summary>
    /// Retrieves stock prices for a list of tickers from the cache.
    /// </summary>
    /// <param name="tickers">A list of stock ticker symbols.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a collection of StockPrice objects 
    /// for the provided tickers.
    /// </returns>
    Task<IEnumerable<StockPrice>> GetAllByTickers(List<string> tickers);

    /// <summary>
    /// Retrieves all stock prices from the cache.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation, containing a collection of all cached StockPrice objects.
    /// </returns>
    Task<IEnumerable<StockPrice>> GetAll();

    /// <summary>
    /// Clears all stock prices from the cache.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. Used for testing purposes.</returns>
    Task ClearAllCache();
}