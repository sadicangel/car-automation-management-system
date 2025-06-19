using CarAutomation.Domain;
using CarAutomation.Domain.Auctions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CarAutomation.WebApi.Auctions.CloseAuction;

public static class CloseAuctionEndpoint
{
    public static async Task<Results<Ok<CloseAuctionResponse>, NotFound, ValidationProblem>> CloseAuction(
        CloseAuctionRequest request,
        AppDbContext dbContext,
        TimeProvider timeProvider,
        ILogger<CloseAuctionRequest> logger)
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

        logger.LogInformation("Auction has been closed: {@Auction}", auction);

        // We would trigger a notification for all participants here.

        var highestBid = auction.Bids.MaxBy(x => x.Amount);

        if (highestBid is not null)
        {
            logger.LogInformation(
                "User {@UserEmail} is the auction winnder with a bid of {@Bid}",
                highestBid.BidderEmail,
                highestBid.Amount);

            // We would trigger a notification for the winner here.
        }
        else
        {
            logger.LogInformation("The auction was closed with no bids");
        }

        return TypedResults.Ok(new CloseAuctionResponse(
            AuctionId: auction.AuctionId,
            VehicleId: auction.VehicleId,
            StartDate: auction.StartDate,
            EndDate: auction.EndDate.Value,
            HighestBidEur: highestBid?.Amount));
    }
}
