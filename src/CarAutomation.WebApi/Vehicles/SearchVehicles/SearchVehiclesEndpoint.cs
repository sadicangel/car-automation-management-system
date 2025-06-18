using CarAutomation.Domain;
using CarAutomation.Domain.Vehicles;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CarAutomation.WebApi.Vehicles.SearchVehicles;

public static class SearchVehiclesEndpoint
{
    public static async Task<Ok<SearchVehiclesResponse>> SearchVehicles([AsParameters] SearchVehiclesRequest request, AppDbContext dbContext)
    {
        var query = dbContext.Vehicles.AsNoTracking();

        query = request.Type switch
        {
            VehicleType.Sedan => query.OfType<Sedan>(),
            VehicleType.Suv => query.OfType<Suv>(),
            VehicleType.Hatchback => query.OfType<Hatchback>(),
            VehicleType.Truck => query.OfType<Truck>(),
            null => query,
            _ => throw new InvalidOperationException($"Unsupported vehicle type: {request.Type}")
        };
        if (!string.IsNullOrEmpty(request.Manufacturer))
            query = query.Where(x => x.Manufacturer == request.Manufacturer);
        if (!string.IsNullOrEmpty(request.Model))
            query = query.Where(x => x.Model == request.Model);
        if (request.Year.HasValue)
            query = query.Where(x => x.Year == request.Year.Value);

        var vehicles = await query.ToListAsync();

        return TypedResults.Ok(new SearchVehiclesResponse([.. vehicles.Select(SearchVehiclesLine.FromVehicle)]));
    }
}
