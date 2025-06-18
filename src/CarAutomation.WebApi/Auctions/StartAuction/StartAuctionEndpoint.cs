using CarAutomation.Domain;
using CarAutomation.Domain.Auctions;
using CarAutomation.Domain.Vehicles;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CarAutomation.WebApi.Auctions.StartAuction;

public static class StartAuctionEndpoint
{
    public static async Task<Results<Ok<StartAuctionResponse>, BadRequest<string>>> StartAuction(
        StartAuctionRequest request,
        AppDbContext dbContext,
        TimeProvider timeProvider)
    {
        var vehicle = await dbContext.FindAsync<Vehicle>(request.VehicleId);
        if (vehicle is null)
        {
            return TypedResults.BadRequest($"Vehicle '{request.VehicleId}' does not exist");
        }

        if (dbContext.Auctions.Where(x => x.VehicleId == request.VehicleId).Any(x => x.IsActive))
        {
            return TypedResults.BadRequest($"An auction is already active for vehicle '{request.VehicleId}'");
        }

        var auction = new Auction(
            AuctionId: default,
            VehicleId: vehicle.Id,
            StartingBidEur: vehicle.StartingBidEur,
            IsActive: true,
            StartDate: timeProvider.GetUtcNow(),
            EndDate: null);

        dbContext.Auctions.Add(auction);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(new StartAuctionResponse(
            auction.AuctionId,
            auction.VehicleId,
            auction.StartingBidEur));
    }
}
