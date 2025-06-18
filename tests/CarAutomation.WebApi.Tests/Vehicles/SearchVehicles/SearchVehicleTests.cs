using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarAutomation.Domain;
using CarAutomation.WebApi.Vehicles;
using CarAutomation.WebApi.Vehicles.SearchVehicles;

namespace CarAutomation.WebApi.Tests.Vehicles.SearchVehicles;
public sealed class SearchVehicleTests(PostgreSqlFixture postgreSqlFixture) : IAsyncDisposable
{
    private readonly WebApiFixture _fixture = new(postgreSqlFixture) { ConfigureDbContext = ConfigureDbContext };
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    private static void ConfigureDbContext(AppDbContext dbContext)
    {
        dbContext.Vehicles.AddRange(SearchVehicleTestData.Vehicles);
        dbContext.SaveChanges();
    }

    [Fact]
    public async Task Search_returns_all_vehicles_when_no_parameters_are_specified()
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.GetFromJsonAsync<SearchVehiclesResponse>("/vehicles", _jsonOptions, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
        Assert.Equal(SearchVehicleTestData.Vehicles.Count, response.Vehicles.Count);
    }

    [Theory]
    [MemberData(nameof(GetAllVehicleTypes))]
    public async Task Search_with_type_filter_returns_only_vehicles_matching_that_type(VehicleType type)
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.GetFromJsonAsync<SearchVehiclesResponse>($"/vehicles?type={type}", _jsonOptions, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
        Assert.NotEmpty(response.Vehicles);
        Assert.All(response.Vehicles, vehicle => Assert.Equal(type, vehicle.Type));
    }

    public static TheoryData<VehicleType> GetAllVehicleTypes() => [.. Enum.GetValues<VehicleType>()];

    [Theory]
    [InlineData("Toyota", 2)]
    [InlineData("Ford", 1)]
    [InlineData("BMW", 0)]
    public async Task Search_with_manufacturer_filter_returns_only_vehicles_matching_that_manufacturer(string manufacturer, int expectedCount)
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.GetFromJsonAsync<SearchVehiclesResponse>($"/vehicles?manufacturer={manufacturer}", _jsonOptions, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
        Assert.Equal(expectedCount, response.Vehicles.Count);
        Assert.All(response.Vehicles, vehicle => Assert.Equal(manufacturer, vehicle.Manufacturer));
    }

    [Theory]
    [InlineData("Accord", 1)]
    [InlineData("Odyssey", 0)]
    public async Task Search_with_model_filter_returns_only_vehicles_matching_that_model(string model, int expectedCount)
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.GetFromJsonAsync<SearchVehiclesResponse>($"/vehicles?manufacturer=Honda&model={model}", _jsonOptions, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
        Assert.Equal(expectedCount, response.Vehicles.Count);
        Assert.All(response.Vehicles, vehicle => Assert.Equal(model, vehicle.Model));
    }

    [Theory]
    [InlineData(2021, 3)]
    [InlineData(2025, 0)]
    public async Task Search_with_year_filter_returns_only_vehicles_matching_that_year(int year, int expectedCount)
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.GetFromJsonAsync<SearchVehiclesResponse>($"/vehicles?year={year}", _jsonOptions, TestContext.Current.CancellationToken);

        Assert.NotNull(response);
        Assert.Equal(expectedCount, response.Vehicles.Count);
        Assert.All(response.Vehicles, vehicle => Assert.Equal(year, vehicle.Year));
    }
}
