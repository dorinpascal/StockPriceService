using Stock_Price_Service.Helpers;
using Stock_Price_Service.Models;

namespace Stock_Price_Service.Repositories.Persistence;

public class StockPriceRepository : IStockPriceRepository
{
    private static readonly string[] stockTickers = ["DANSKE", "AAPL", "GOOG", "TSLA", "MSFT", "AMZN"];
    public Task<IEnumerable<StockPrice>> GetAllByTickers(List<string> tickers)
    {
        var stockPriceList = stockTickers
            .Where(ticker => tickers.Contains(ticker))
            .Select(ticker => new StockPrice
            {
                Ticker = ticker,
                Price = GenerateStockPrice.GenerateRandom()
            })
            .ToList();

        return Task.FromResult((IEnumerable<StockPrice>)stockPriceList);
    }
}
