using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarAutomation.Domain;
using CarAutomation.WebApi.Auctions.CloseAuction;

namespace CarAutomation.WebApi.Tests.Auctions.CloseAuction;

public sealed class CloseAuctionTests(PostgreSqlFixture postgreSqlFixture) : IAsyncDisposable
{
    private readonly WebApiFixture _fixture = new(postgreSqlFixture) { ConfigureDbContext = ConfigureDbContext };
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    private static void ConfigureDbContext(AppDbContext dbContext)
    {
        dbContext.Vehicles.Add(CloseAuctionTestData.Vehicle);
        dbContext.Auctions.Add(CloseAuctionTestData.ActiveAuction);
        dbContext.Auctions.Add(CloseAuctionTestData.ClosedAuction);
        dbContext.SaveChanges();
    }

    [Fact]
    public async Task Attempting_to_close_an_auction_that_does_not_exist_results_in_not_found()
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/auctions/close",
            new CloseAuctionRequest(AuctionId: Guid.NewGuid()),
            _jsonOptions,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Attempting_to_close_an_auction_that_is_not_active_results_in_bad_request()
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/auctions/close",
            new CloseAuctionRequest(CloseAuctionTestData.ClosedAuction.AuctionId),
            _jsonOptions,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Closing_an_active_auction_results_in_the_auction_being_closed()
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/auctions/close",
            new CloseAuctionRequest(CloseAuctionTestData.ActiveAuction.AuctionId),
            _jsonOptions,
            TestContext.Current.CancellationToken);

        var auction = await response.Content.ReadFromJsonAsync<CloseAuctionResponse>(_jsonOptions, TestContext.Current.CancellationToken);

        Assert.NotNull(auction);
        Assert.Equal(CloseAuctionTestData.Vehicle.Id, auction.VehicleId);
        Assert.Equal(DateTimeOffset.UtcNow.Date, auction.EndDate.Date);
        Assert.Equal(0, auction.HighestBidEur);
    }
}
