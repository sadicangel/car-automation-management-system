using CarAutomation.Domain;
using CarAutomation.Domain.Auctions;
using CarAutomation.Domain.Vehicles;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CarAutomation.WebApi.Auctions.StartAuction;

public static class StartAuctionEndpoint
{
    public static async Task<Results<Ok<StartAuctionResponse>, ValidationProblem>> StartAuction(
        StartAuctionRequest request,
        AppDbContext dbContext,
        TimeProvider timeProvider,
        ILogger<StartAuctionRequest> logger)
    {
        var vehicle = await dbContext.FindAsync<Vehicle>(request.VehicleId);
        if (vehicle is null)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["VehicleId"] = [$"Vehicle '{request.VehicleId}' does not exist"]
            });
        }

        if (dbContext.Auctions.Where(x => x.VehicleId == request.VehicleId).Any(x => x.IsActive))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["VehicleId"] = [$"An auction is already active for vehicle '{request.VehicleId}'"]
            });
        }

        var auction = new Auction
        {
            VehicleId = vehicle.Id,
            StartingBidEur = vehicle.StartingBidEur,
            StartDate = timeProvider.GetUtcNow(),
            IsActive = true,
        };

        dbContext.Auctions.Add(auction);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("A new auction has been started {@Auction}", auction);

        return TypedResults.Ok(new StartAuctionResponse(
            auction.AuctionId,
            auction.VehicleId,
            auction.StartingBidEur));
    }
}
