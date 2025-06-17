using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarAutomation.WebApi.Vehicles;

namespace CarAutomation.WebApi.Tests.Vehicles.AddVehicles;

public class AddVehiclesTests(WebApiFixture fixture) : IClassFixture<WebApiFixture>
{
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    [Theory]
    [InlineData(null), InlineData(""), InlineData("12345")]
    public async Task Attempting_to_add_a_vehicle_with_invalid_VIN_results_in_bad_request(string? vin)
    {
        var httpClient = fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/vehicles",
            Data.SedanRequest with { Vin = vin! },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null), InlineData("")]
    public async Task Attempting_to_add_a_vehicle_with_invalid_manufacturer_results_in_bad_request(string? manufacturer)
    {
        var httpClient = fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/vehicles",
            Data.SedanRequest with { Manufacturer = manufacturer! },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null), InlineData("")]
    public async Task Attempting_to_add_a_vehicle_with_invalid_model_results_in_bad_request(string? model)
    {
        var httpClient = fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/vehicles",
            Data.SedanRequest with { Model = model! },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(0), InlineData(-1)]
    public async Task Attempting_to_add_a_vehicle_with_invalid_year_results_in_bad_request(int year)
    {
        var httpClient = fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/vehicles",
            Data.SedanRequest with { Year = year! },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(0), InlineData(-1)]
    public async Task Attempting_to_add_a_vehicle_with_invalid_starting_bid_results_in_bad_request(int startingBid)
    {
        var httpClient = fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/vehicles",
            Data.SedanRequest with { StartingBidEur = startingBid! },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null), InlineData(-1), InlineData(0)]
    public async Task Attempting_to_add_a_sedan_with_invalid_number_of_doors_results_in_bad_request(int? numberOfDoors)
    {
        var httpClient = fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/vehicles",
            Data.SedanRequest with { NumberOfDoors = numberOfDoors },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null), InlineData(-1), InlineData(0)]
    public async Task Attempting_to_add_a_hatchback_with_invalid_number_of_doors_results_in_bad_request(int? numberOfDoors)
    {
        var httpClient = fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/vehicles",
            Data.HatchbackRequest with { NumberOfDoors = numberOfDoors },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null), InlineData(-1), InlineData(0)]
    public async Task Attempting_to_add_a_suv_with_invalid_number_of_seats_results_in_bad_request(int? numberOfSeats)
    {
        var httpClient = fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/vehicles",
            Data.SuvRequest with { NumberOfSeats = numberOfSeats },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(null), InlineData(-1), InlineData(0)]
    public async Task Attempting_to_add_a_truck_with_invalid_load_capacity_results_in_bad_request(int? loadCapacity)
    {
        var httpClient = fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/vehicles",
            Data.TruckRequest with { LoadCapacityKg = loadCapacity },
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Attempting_to_add_a_previously_added_vehicle_results_in_conflict()
    {
        var httpClient = fixture.CreateClient();

        await httpClient.PostAsJsonAsync(
            "/vehicles",
            Data.SedanRequest,
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        var response = await httpClient.PostAsJsonAsync(
            "/vehicles",
            Data.SedanRequest,
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}

public static class Data
{
    public static readonly AddVehicleRequest SedanRequest = new(
        Vin: "3FAHP0HA6AR123456",
        Type: VehicleType.Sedan,
        Manufacturer: "Honda",
        Model: "Accord",
        Year: 2021,
        NumberOfDoors: 4,
        NumberOfSeats: null,
        LoadCapacityKg: null,
        StartingBidEur: 12000.00m
    );

    public static readonly AddVehicleRequest HatchbackRequest = new(
        Vin: "WVWZZZ1KZAW000123",
        Type: VehicleType.Hatchback,
        Manufacturer: "Volkswagen",
        Model: "Golf",
        Year: 2020,
        NumberOfDoors: 5,
        NumberOfSeats: null,
        LoadCapacityKg: null,
        StartingBidEur: 9500.00m
    );

    public static readonly AddVehicleRequest TruckRequest = new(
        Vin: "1FTFW1ET1EFA12345",
        Type: VehicleType.Truck,
        Manufacturer: "Ford",
        Model: "F-150",
        Year: 2023,
        NumberOfDoors: null,
        NumberOfSeats: null,
        LoadCapacityKg: 1200.0,
        StartingBidEur: 25000.00m
    );

    public static readonly AddVehicleRequest SuvRequest = new(
        Vin: "1HGCM82633A004352",
        Type: VehicleType.Suv,
        Manufacturer: "Toyota",
        Model: "RAV4",
        Year: 2022,
        NumberOfDoors: null,
        NumberOfSeats: 5,
        LoadCapacityKg: null,
        StartingBidEur: 15000.00m
    );
}
