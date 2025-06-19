using CarAutomation.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CarAutomation.WebApi.Vehicles.AddVehicle;

public static class AddVehicleEndpoint
{
    public static async Task<Results<Ok<AddVehicleResponse>, Conflict<string>>> AddVehicle(
        AddVehicleRequest request,
        AppDbContext dbContext,
        ILogger<AddVehicleRequest> logger)
    {
        try
        {
            var entry = dbContext.Vehicles.Add(request.ToVehicle());

            await dbContext.SaveChangesAsync();

            logger.LogInformation("A new vehicle has been added: {@Vehicle}", entry.Entity);

            return TypedResults.Ok(AddVehicleResponse.FromVehicle(entry.Entity));
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return TypedResults.Conflict($"A vehicle with VIN '{request.Vin}' already exists");
        }
    }
}
