using CarAutomation.Domain;
using CarAutomation.Domain.Vehicles;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CarAutomation.WebApi.Vehicles.AddVehicle;

public static class AddVehicleEndpoint
{
    public static async Task<Results<Created<AddVehicleResponse>, Conflict<string>>> AddVehicle(AddVehicleRequest request, AppDbContext dbContext)
    {
        try
        {
            var entry = dbContext.Vehicles.Add(new Vehicle(
                Id: default,
                Vin: request.Vin,
                Type: request.Type,
                Manufacturer: request.Manufacturer,
                Model: request.Model,
                Year: request.Year,
                NumberOfDoors: request.NumberOfDoors,
                NumberOfSeats: request.NumberOfSeats,
                LoadCapacityKg: request.LoadCapacityKg,
                StartingBidEur: request.StartingBidEur));

            await dbContext.SaveChangesAsync();

            return TypedResults.Created(default(string), AddVehicleResponse.From(entry.Entity));
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return TypedResults.Conflict($"A vehicle with VIN '{request.Vin}' already exists");
        }
    }
}
