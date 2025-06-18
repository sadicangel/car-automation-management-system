using CarAutomation.WebApi.Vehicles.AddVehicle;
using CarAutomation.WebApi.Vehicles.SearchVehicles;

namespace CarAutomation.WebApi.Vehicles;

public static class VehiclesEndpoints
{
    public static IEndpointRouteBuilder MapVehiclesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var vehicles = endpoints
            .MapGroup("/vehicles")
            .WithValidation()
            .WithTags(nameof(VehiclesEndpoints));

        vehicles.MapPost("/", AddVehicleEndpoint.AddVehicle);

        vehicles.MapGet("/", SearchVehiclesEndpoint.SearchVehicles);

        return endpoints;
    }
}
