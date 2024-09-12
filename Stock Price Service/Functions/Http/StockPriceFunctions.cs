using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Stock_Price_Service.Dto;
using Stock_Price_Service.Helpers;
using Stock_Price_Service.Services;
using System.Net;

namespace Stock_Price_Service.Functions.Http;

public class StockPriceFunctions(ILogger<StockPriceFunctions> logger, IStockPriceService stockPriceService)
{
    [Function(nameof(GetStockPriceByTickerFunction))]
    [OpenApiOperation(operationId: nameof(GetStockPriceByTickerFunction), tags: ["StockPrice"])]
    [OpenApiParameter(name: "ticker", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The stock ticker symbol.")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(StockPriceDto), Description = "The OK response with stock price")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "Stock ticker not found.")]
    public async Task<IActionResult> GetStockPriceByTickerFunction([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "stockprice/{ticker}")] HttpRequestData req, string ticker)
    {
        try
        {
            // Check if stock price is cached
            var cachedStockPrice = await stockPriceService.GetCachedByTickersAsync([ticker]);
            if (cachedStockPrice.Any())
            {
                return new OkObjectResult(cachedStockPrice.FirstOrDefault());
            }
            // If not cached, fetch from SQL database (real-time)
            var stockPrice = await stockPriceService.GetByTickersAsync([ticker]);
            //Cache for future
            if (stockPrice.Any()) await stockPriceService.UpdateAsync(stockPrice.FirstOrDefault()!);
            return new OkObjectResult(stockPrice.FirstOrDefault());
        }
        catch (Exception ex)
        {
            return HttpResponseMessageHelper.HandleException(ex, logger);
        }
    }
}
