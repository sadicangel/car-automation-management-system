using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CarAutomation.WebApi.Vehicles;

public static class VehicleCatalogEndpoints
{
    public static IEndpointConventionBuilder MapVehicleCatalogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var vehicles = endpoints
            .MapGroup("/vehicles")
            .WithValidation();

        vehicles.MapPost("/", AddVehicle);

        vehicles.MapGet("/", SearchVehicles);

        return vehicles;
    }

    private static async Task<Results<Created<Vehicle>, Conflict<string>>> AddVehicle(AddVehicleRequest request, AppDbContext dbContext)
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
            request.LoadCapacityKg,
            request.StartingBidEur);

        dbContext.Vehicles.Add(vehicle);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return TypedResults.Conflict($"A vehicle with VIN '{request.Vin}' already exists");
        }

        return TypedResults.Created(default(string), vehicle);
    }

    private static async Task<Results<Ok<SearchVehiclesResponse>, BadRequest>> SearchVehicles(
        [AsParameters] SearchVehiclesRequest request,
        AppDbContext dbContext)
    {
        var query = dbContext.Vehicles.AsQueryable();
        if (request.Type.HasValue)
            query = query.Where(x => x.Type == request.Type.Value);
        if (!string.IsNullOrEmpty(request.Manufacturer))
            query = query.Where(x => x.Manufacturer == request.Manufacturer);
        if (!string.IsNullOrEmpty(request.Model))
            query = query.Where(x => x.Model == request.Model);
        if (request.Year.HasValue)
            query = query.Where(x => x.Year == request.Year.Value);

        var vehicles = await query.ToListAsync();

        return TypedResults.Ok(new SearchVehiclesResponse(vehicles));
    }
}
