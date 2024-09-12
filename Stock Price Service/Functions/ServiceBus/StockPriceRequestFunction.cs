using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Stock_Price_Service.Dto;
using Stock_Price_Service.Helpers;
using Stock_Price_Service.Models;
using Stock_Price_Service.Services;
using System.Net;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Stock_Price_Service.Functions.ServiceBus;

public class StockPriceRequestFunction(ILogger<StockPriceRequestFunction> logger, IStockPriceService stockPriceService)
{
    private const string Tag = "StockPrice";

    [Function(nameof(StockPriceFunction))]
    [OpenApiOperation(operationId: nameof(StockPriceFunction), tags: [Tag])]
    [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(List<string>))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(StockPriceDto), Description = "The OK response with stock price")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Invalid bearing: Failed to deserialize the JSON data.")]
    [OpenApiResponseWithBody(HttpStatusCode.Forbidden, Application.Json, typeof(ErrorResponseBody), Description = "Access denied. User does not have permission.")]
    [OpenApiResponseWithBody(HttpStatusCode.ServiceUnavailable, Application.Json, typeof(ErrorResponseBody), Description = "Service didn't work as expected.")]
    public async Task<IActionResult> StockPriceFunction([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "stockprice")] HttpRequest req)
    {
        logger.LogInformation("Received stock price enqueue request.");
        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var tickers = JsonSerializer.Deserialize<List<string>>(requestBody);
            if (tickers is null || tickers.Count == 0)
            {
                throw new ArgumentException("Please provide a valid list of tickers.");
            }
            var cachedStockPrices = await stockPriceService.GetCachedByTickersAsync(tickers);
            if (cachedStockPrices.Count() != tickers.Count)
            {
                return await HandleAcceptedResult(stockPriceService, tickers, cachedStockPrices);
            }
            //ToDo use automapper
            var dto = cachedStockPrices.Select(stockPrice => new StockPriceDto(stockPrice.Ticker, stockPrice.Price));
            return new OkObjectResult(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error enqueueing stock price request.");
            return HttpResponseMessageHelper.HandleException(ex, logger);
        }
    }

    #region private
    private static async Task<IActionResult> HandleAcceptedResult(IStockPriceService stockPriceService, List<string> tickers, IEnumerable<StockPrice> cachedStockPrices)
    {
        var cachedTickers = cachedStockPrices.Select(sp => sp.Ticker).ToList();
        var uncachedTickers = tickers.Where(ticker => !cachedTickers.Contains(ticker)).ToList();
        await stockPriceService.EnqueueTickersAsync(uncachedTickers);
        return new AcceptedResult("", new
        {
            status = "in progress",
            message = "Your request for stock prices is being processed. Please check back later or subscribe to real-time updates."
        });
    }
    #endregion
}
