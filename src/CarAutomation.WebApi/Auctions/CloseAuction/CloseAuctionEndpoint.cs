using CarAutomation.Domain;
using CarAutomation.Domain.Auctions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CarAutomation.WebApi.Auctions.CloseAuction;

public static class CloseAuctionEndpoint
{
    public static async Task<Results<Ok<CloseAuctionResponse>, NotFound, ValidationProblem>> CloseAuction(
        CloseAuctionRequest request,
        AppDbContext dbContext,
        TimeProvider timeProvider)
    {
        var auction = await dbContext.FindAsync<Auction>(request.AuctionId);
        if (auction is null)
        {
            return TypedResults.NotFound();
        }

        if (!auction.IsActive)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["AuctionId"] = [$"Auction '{request.AuctionId}' is not active."]
            });
        }

        auction.EndDate = timeProvider.GetUtcNow();
        auction.IsActive = false;

        dbContext.Update(auction);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(new CloseAuctionResponse(
            AuctionId: auction.AuctionId,
            VehicleId: auction.VehicleId,
            StartDate: auction.StartDate,
            EndDate: auction.EndDate.Value,
            HighestBidEur: auction.Bids.Select(x => x.Amount).DefaultIfEmpty(0).Max()));
    }
}
