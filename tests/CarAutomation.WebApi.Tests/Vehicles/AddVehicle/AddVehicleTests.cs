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

    [Fact]
    public async Task Adding_a_valid_sedan_returns_success_and_its_representation()
    {
        var httpClient = _fixture.CreateClient();

        var response = await httpClient.PostAsJsonAsync(
            "/vehicles",
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
            "/vehicles",
            AddVehicleTestData.SedanRequest,
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        var response = await httpClient.PostAsJsonAsync(
            "/vehicles",
            AddVehicleTestData.SedanRequest,
            _jsonOptions,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
