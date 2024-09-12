using Stock_Price_Service.Models;

namespace Stock_Price_Service.Services;


/// <summary>
/// Interface for managing stock prices, including generating, updating, caching, and retrieving stock price data.
/// </summary>
public interface IStockPriceService
{
    /// <summary>
    /// Generates stock prices and stores them in the cache.
    /// This method simulates price generation for hardcoded tickers.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task GenerateAndStoreStockPricesAsync();

    /// <summary>
    /// Updates the stock price for a specific ticker.
    /// </summary>
    /// <param name="stockPrice">The StockPrice object containing the updated price information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(StockPrice stockPrice);

    /// <summary>
    /// Retrieves stock prices for a list of tickers from the cache.
    /// </summary>
    /// <param name="tickers">A list of stock ticker symbols.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a collection of cached StockPrice objects 
    /// for the provided tickers
    /// </returns>
    Task<IEnumerable<StockPrice>> GetCachedByTickersAsync(List<string> tickers);

    /// <summary>
    /// Retrieves stock prices for a list of tickers from the primary data source (not the cache).
    /// </summary>
    /// <param name="tickers">A list of stock ticker symbols.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a collection of StockPrice objects 
    /// for the provided tickers.
    /// </returns>
    Task<IEnumerable<StockPrice>> GetByTickersAsync(List<string> tickers);

    /// <summary>
    /// Enqueues a list of tickers for background processing, used for service bus queue.
    /// </summary>
    /// <param name="tickers">A list of stock ticker symbols to be enqueued.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task EnqueueTickersAsync(List<string> tickers);

    /// <summary>
    /// Retrieves all stock prices currently stored in the cache.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation, containing a collection of all cached StockPrice objects.
    /// </returns>
    Task<IEnumerable<StockPrice>> GetAllCachedAsync();
}