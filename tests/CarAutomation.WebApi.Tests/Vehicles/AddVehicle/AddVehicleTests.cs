using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarAutomation.WebApi.Vehicles.AddVehicle;

namespace CarAutomation.WebApi.Tests.Vehicles.AddVehicle;

public sealed class AddVehicleTests(PostgreSqlFixture postgreSqlFixture) : IAsyncDisposable
{
    private readonly WebApiFixture _fixture = new(postgreSqlFixture);
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };


    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Theory]
    [InlineData(null), InlineData(""), InlineData("12345")]
    public async Task Attempting_to_add_a_vehicle_with_invalid_VIN_results_in_bad_request(string? vin)
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/vehicles",
            AddVehicleTestData.SedanRequest with { Vin = vin! },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null), InlineData("")]
    public async Task Attempting_to_add_a_vehicle_with_invalid_manufacturer_results_in_bad_request(string? manufacturer)
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/vehicles",
            AddVehicleTestData.SedanRequest with { Manufacturer = manufacturer! },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null), InlineData("")]
    public async Task Attempting_to_add_a_vehicle_with_invalid_model_results_in_bad_request(string? model)
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/vehicles",
            AddVehicleTestData.SedanRequest with { Model = model! },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(0), InlineData(-1)]
    public async Task Attempting_to_add_a_vehicle_with_invalid_year_results_in_bad_request(int year)
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/vehicles",
            AddVehicleTestData.SedanRequest with { Year = year! },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(0), InlineData(-1)]
    public async Task Attempting_to_add_a_vehicle_with_invalid_starting_bid_results_in_bad_request(int startingBid)
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/vehicles",
            AddVehicleTestData.SedanRequest with { StartingBidEur = startingBid! },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null), InlineData(-1), InlineData(0)]
    public async Task Attempting_to_add_a_sedan_with_invalid_number_of_doors_results_in_bad_request(int? numberOfDoors)
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/vehicles",
            AddVehicleTestData.SedanRequest with { NumberOfDoors = numberOfDoors },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null), InlineData(-1), InlineData(0)]
    public async Task Attempting_to_add_a_hatchback_with_invalid_number_of_doors_results_in_bad_request(int? numberOfDoors)
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/vehicles",
            AddVehicleTestData.HatchbackRequest with { NumberOfDoors = numberOfDoors },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null), InlineData(-1), InlineData(0)]
    public async Task Attempting_to_add_a_suv_with_invalid_number_of_seats_results_in_bad_request(int? numberOfSeats)
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/vehicles",
            AddVehicleTestData.SuvRequest with { NumberOfSeats = numberOfSeats },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null), InlineData(-1), InlineData(0)]
    public async Task Attempting_to_add_a_truck_with_invalid_load_capacity_results_in_bad_request(int? loadCapacity)
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/vehicles",
            AddVehicleTestData.TruckRequest with { LoadCapacityKg = loadCapacity },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Adding_a_valid_sedan_returns_success_and_its_representation()
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/vehicles",
            AddVehicleTestData.SedanRequest,
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(response.IsSuccessStatusCode);

        var vehicle = await response.Content.ReadFromJsonAsync<AddVehicleResponse>(_jsonOptions, TestContext.Current.CancellationToken);

        Assert.NotNull(vehicle);
    }

    [Fact]
    public async Task Attempting_to_add_a_previously_added_vehicle_results_in_conflict()
    {
        var httpClient = _fixture.CreateClient();

        await httpClient.PostAsJsonAsync(
            "/api/v1/vehicles",
            AddVehicleTestData.SedanRequest,
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        var response = await httpClient.PostAsJsonAsync(
            "/api/v1/vehicles",
            AddVehicleTestData.SedanRequest,
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
