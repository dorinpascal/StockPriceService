using StackExchange.Redis;
using Stock_Price_Service.Helpers;
using Stock_Price_Service.Models;

namespace Stock_Price_Service.Repositories.Cache;

public class StockPriceCacheRepository : IStockPriceCacheRepository
{
    private readonly IDatabase _database;
    private readonly string _redisHash = "stockPrices";

    public StockPriceCacheRepository(IConnectionMultiplexer connection)
    {
        _database = connection.GetDatabase();
    }

    public async Task<IEnumerable<StockPrice>> GetAll()
    {
        var hashEntries = await _database.HashGetAllAsync(_redisHash);

        var stockPrices = hashEntries
        .Where(entry => !entry.Name.IsNullOrEmpty && !entry.Value.IsNullOrEmpty)
        .Select(entry => new StockPrice
        {
            Ticker = entry.Name!,
            Price = DoubleConversionHelper.ConvertStringToDouble(entry.Value) ?? 0.0
        })
        .ToList();

        return stockPrices;
    }

    public async Task<IEnumerable<StockPrice>> GetAllByTickers(List<string> tickers)
    {
        var stockPrices = new List<StockPrice>();
        foreach (var ticker in tickers)
        {
            await HandleStockPrices(stockPrices, ticker);
        }
        return stockPrices;
    }

    public async Task<double?> GetStockPriceAsync(string ticker)
    {
        var stockPriceString = await _database.HashGetAsync(_redisHash, ticker);
        if (!stockPriceString.IsNull)
        {
            return DoubleConversionHelper.ConvertStringToDouble(stockPriceString);
        }
        return null;
    }

    public async Task SetStockPriceAsync(StockPrice stockPrice)
    {
        await _database.HashSetAsync(_redisHash, stockPrice.Ticker, stockPrice.Price.ToString());
    }

    public async Task ClearAllCache()
    {
        await _database.ExecuteAsync("FLUSHDB");
    }


    #region private

    private async Task HandleStockPrices(List<StockPrice> stockPrices, string ticker)
    {
        var stockPriceValue = await _database.HashGetAsync(_redisHash, ticker);
        if (!stockPriceValue.IsNull)
        {
            stockPrices.Add(new StockPrice
            {
                Ticker = ticker,
                Price = DoubleConversionHelper.ConvertStringToDouble(stockPriceValue)!.Value
            });
        }
    }

    #endregion
}
