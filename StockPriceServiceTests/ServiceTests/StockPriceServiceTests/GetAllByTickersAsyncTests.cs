using AutoFixture;
using Azure.Messaging.ServiceBus;
using NSubstitute;
using Stock_Price_Service.Models;
using Stock_Price_Service.Repositories.Cache;
using Stock_Price_Service.Repositories.Persistence;
using Stock_Price_Service.Services;
using StockPriceServiceTests.Helper;

namespace StockPriceServiceTests.ServiceTests.StockPriceServiceTests;

public class GetAllByTickersAsyncTests
{
    private readonly IStockPriceCacheRepository _cacheRepository;
    private readonly IStockPriceRepository _repository;
    private readonly StockPriceService _sut;
    private readonly IFixture fixture;
    private readonly ServiceBusClient _serviceBusClient;

    public GetAllByTickersAsyncTests()
    {
        _cacheRepository = Substitute.For<IStockPriceCacheRepository>();
        _repository = Substitute.For<IStockPriceRepository>();
        _serviceBusClient = Substitute.For<ServiceBusClient>();
        _sut = new StockPriceService(_cacheRepository, _repository, _serviceBusClient);
        fixture = new CustomFixture();
    }


    [Fact]
    public async Task StockPriceService_RepositoryIsCalled_ReturnsEmptyList()
    {
        // Arrange
        var tickers = new List<StockPrice>();
        // Act
        _repository.GetAllByTickers(Arg.Any<List<string>>()).Returns(tickers);
        var result = await _sut.GetByTickersAsync(["DANSKE"]);
        var response = (List<StockPrice>)result;

        // Assert
        Assert.Empty(response);
    }

    [Fact]
    public async Task StockPriceService_RepositoryIsCalled_ReturnsListOfObjects()
    {
        // Arrange
        var tickers = fixture.Create<List<StockPrice>>();
        // Act
        _repository.GetAllByTickers(Arg.Any<List<string>>()).Returns(tickers);
        var result = await _sut.GetByTickersAsync(["DANSKE"]);
        var response = (List<StockPrice>)result;
        // Assert
        Assert.NotNull(result);
        Assert.Equal(tickers.Count, response.Count);
    }
}
