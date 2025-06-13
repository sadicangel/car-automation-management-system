using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace WebApi.VehicleCatalog;

public static class VehicleCatalogEndpoints
{
    public static IEndpointConventionBuilder MapVehicleCatalogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var vehicles = endpoints.MapGroup("/vehicles");

        vehicles.MapPost("/", AddVehicle);

        vehicles.MapGet("/", SearchVehicles);

        return vehicles;
    }

    private static async Task<Results<Created<Vehicle>, Conflict>> AddVehicle(AddVehicleRequest request, AppDbContext dbContext)
    {
        var vehicle = new Vehicle(
            default,
            request.Vin,
            request.Type,
            request.Manufacturer,
            request.Model,
            request.Year,
            request.NumberOfDoors,
            request.NumberOfSeats,
            request.LoadCapacity,
            request.StartingBid);

        dbContext.Vehicles.Add(vehicle);
        await dbContext.SaveChangesAsync();

        return TypedResults.Created(default(string), vehicle);
    }

    private static async Task<Results<Ok<SearchVehiclesResponse>, BadRequest>> SearchVehicles(
        [AsParameters] SearchVehiclesRequest request,
        AppDbContext dbContext)
    {
        var query = dbContext.Vehicles.AsQueryable();
        if (request.Type.HasValue)
        {
            query = query.Where(x => x.Type == request.Type.Value);
        }
        if (!string.IsNullOrEmpty(request.Manufacturer))
        {
            query = query.Where(x => x.Manufacturer == request.Manufacturer);
        }
        if (!string.IsNullOrEmpty(request.Model))
        {
            query = query.Where(x => x.Model == request.Model);
        }
        if (request.Year.HasValue)
        {
            query = query.Where(x => x.Year == request.Year.Value);
        }

        var vehicles = await query.ToListAsync();

        return TypedResults.Ok(new SearchVehiclesResponse(vehicles));
    }
}
