using CarAutomation.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CarAutomation.WebApi.Vehicles.SearchVehicles;

public static class SearchVehiclesEndpoint
{
    public static async Task<Ok<SearchVehiclesResponse>> SearchVehicles([AsParameters] SearchVehiclesRequest request, AppDbContext dbContext)
    {
        var query = dbContext.Vehicles.AsNoTracking();
        if (request.Type.HasValue)
            query = query.Where(x => x.Type == request.Type.Value);
        if (!string.IsNullOrEmpty(request.Manufacturer))
            query = query.Where(x => x.Manufacturer == request.Manufacturer);
        if (!string.IsNullOrEmpty(request.Model))
            query = query.Where(x => x.Model == request.Model);
        if (request.Year.HasValue)
            query = query.Where(x => x.Year == request.Year.Value);

        var vehicles = await query.ToListAsync();

        return TypedResults.Ok(SearchVehiclesResponse.From(vehicles));
    }
}
