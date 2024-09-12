using Stock_Price_Service.Models;

namespace Stock_Price_Service.Repositories.Persistence;


/// <summary>
/// Interface for the Stock Price Repository, providing methods to retrieve stock price data.
/// This repository simulates interaction with a data source, such as an SQL database, for stock prices.
/// </summary>
public interface IStockPriceRepository
{
    /// <summary>
    /// Retrieves stock prices for the specified list of tickers from the data source.
    /// This method mocks the retrieval of stock prices, typically from an SQL database.
    /// </summary>
    /// <param name="tickers">A list of stock tickers for which stock prices should be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation, containing a collection of stock prices.</returns>
    Task<IEnumerable<StockPrice>> GetAllByTickers(List<string> tickers);
}
