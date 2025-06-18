using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarAutomation.Domain;
using CarAutomation.WebApi.Auctions.StartAuction;

namespace CarAutomation.WebApi.Tests.Auctions.StartAuction;

public sealed class StartAuctionTests(PostgreSqlFixture postgreSqlFixture) : IAsyncDisposable
{
    private readonly WebApiFixture _fixture = new(postgreSqlFixture) { ConfigureDbContext = ConfigureDbContext };
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    private static void ConfigureDbContext(AppDbContext dbContext)
    {
        dbContext.Vehicles.Add(StartAuctionTestData.Vehicle);
        dbContext.SaveChanges();
    }

    [Fact]
    public async Task Attempting_to_start_an_auction_with_a_vehicle_that_does_not_exist_results_in_bad_request()
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/auctions/start",
            new StartAuctionRequest(VehicleId: Guid.NewGuid()),
            _jsonOptions,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempting_to_start_an_auction_with_a_vehicle_that_is_already_being_auctioned_results_in_bad_request()
    {
        var httpClient = _fixture.CreateClient();

        await httpClient.PostAsJsonAsync(
            "/api/v1/auctions/start",
            new StartAuctionRequest(StartAuctionTestData.Vehicle.Id),
            _jsonOptions,
            TestContext.Current.CancellationToken);

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/auctions/start",
            new StartAuctionRequest(StartAuctionTestData.Vehicle.Id),
            _jsonOptions,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Starting_an_auction_with_an_available_vehicle_results_in_the_auction_being_created()
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/auctions/start",
            new StartAuctionRequest(StartAuctionTestData.Vehicle.Id),
            _jsonOptions,
            TestContext.Current.CancellationToken);

        var auction = await response.Content.ReadFromJsonAsync<StartAuctionResponse>(_jsonOptions, TestContext.Current.CancellationToken);

        Assert.NotNull(auction);
        Assert.Equal(StartAuctionTestData.Vehicle.Id, auction.VehicleId);
        Assert.Equal(StartAuctionTestData.Vehicle.StartingBidEur, auction.StartingBidEur);
    }
}
