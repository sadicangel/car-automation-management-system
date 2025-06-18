using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarAutomation.Domain;
using CarAutomation.WebApi.Auctions.PlaceBid;

namespace CarAutomation.WebApi.Tests.Auctions.PlaceBid;

public sealed class PlaceBidTests(PostgreSqlFixture postgreSqlFixture) : IAsyncDisposable
{
    private readonly WebApiFixture _fixture = new(postgreSqlFixture) { ConfigureDbContext = ConfigureDbContext };
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    private static void ConfigureDbContext(AppDbContext dbContext)
    {
        dbContext.Vehicles.Add(PlaceBidTestData.Vehicle);
        dbContext.Auctions.Add(PlaceBidTestData.ActiveAuction);
        dbContext.Auctions.Add(PlaceBidTestData.ClosedAuction);
        dbContext.SaveChanges();
    }

    [Fact]
    public async Task Attempting_to_place_a_bid_on_an_auction_that_does_not_exist_results_in_not_found()
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/auctions/bid",
            new PlaceBidRequest(AuctionId: Guid.NewGuid(), BidAmount: 9999),
            _jsonOptions,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Attempting_to_place_a_bid_on_an_auction_that_is_no_longer_active_results_in_bad_request()
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/auctions/bid",
            new PlaceBidRequest(PlaceBidTestData.ClosedAuction.AuctionId, BidAmount: 20000),
            _jsonOptions,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-1), InlineData(0)]
    public async Task Attempting_to_place_a_bid_with_a_non_positive_amount_results_in_bad_request(decimal bidAmount)
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/auctions/bid",
            new PlaceBidRequest(PlaceBidTestData.ActiveAuction.AuctionId, bidAmount),
            _jsonOptions,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempting_to_place_a_bid_lower_than_starting_bid_results_in_bad_request()
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/auctions/bid",
            new PlaceBidRequest(PlaceBidTestData.ActiveAuction.AuctionId, PlaceBidTestData.ActiveAuction.StartingBidEur - 1),
            _jsonOptions,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempting_to_place_a_bid_lower_than_the_current_highest_bid_results_in_bad_request()
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/auctions/bid",
            new PlaceBidRequest(PlaceBidTestData.ActiveAuction.AuctionId, PlaceBidTestData.ActiveAuction.Bids.Select(x => x.Amount).Max() - 1),
            _jsonOptions,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempting_to_place_a_bid_equal_to_the_current_highest_bid_results_in_bad_request()
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/auctions/bid",
            new PlaceBidRequest(PlaceBidTestData.ActiveAuction.AuctionId, PlaceBidTestData.ActiveAuction.Bids.Select(x => x.Amount).Max()),
            _jsonOptions,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
