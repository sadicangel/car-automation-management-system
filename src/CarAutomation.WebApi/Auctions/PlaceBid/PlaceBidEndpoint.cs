using CarAutomation.Domain;
using CarAutomation.Domain.Auctions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CarAutomation.WebApi.Auctions.PlaceBid;

public static class PlaceBidEndpoint
{
    public static async Task<Results<Ok, NotFound, ValidationProblem>> PlaceBid(
        PlaceBidRequest request,
        AppDbContext dbContext,
        TimeProvider timeProvider,
        ILogger<PlaceBidRequest> logger)
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
                ["AuctionId"] = [$"Auction '{request.AuctionId}' is no longer active"]
            });
        }

        if (request.BidAmount < auction.StartingBidEur)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["BidAmount"] = [$"Bid amount cannot be less than starting bid ({auction.StartingBidEur})"]
            });
        }

        if (auction.Bids.Count > 0)
        {
            var currentHighestBid = auction.Bids.Select(x => x.Amount).Max();

            if (request.BidAmount <= currentHighestBid)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["BidAmount"] = [$"Bid amount must be higher than the current highest bid ({currentHighestBid})"]
                });
            }
        }

        auction.Bids.Add(new Bid(request.BidAmount, request.UserEmail, timeProvider.GetUtcNow()));

        dbContext.Auctions.Update(auction);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("User {@UserEmail} has placed a bid of {@Bid} in auction {@Auction}",
            request.UserEmail,
            request.BidAmount,
            auction);

        return TypedResults.Ok();
    }
}
