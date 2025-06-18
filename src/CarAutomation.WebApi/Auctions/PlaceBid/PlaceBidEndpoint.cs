using CarAutomation.Domain;
using CarAutomation.Domain.Auctions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CarAutomation.WebApi.Auctions.PlaceBid;

public static class PlaceBidEndpoint
{
    public static async Task<Results<Ok, NotFound, ValidationProblem>> PlaceBid(
        PlaceBidRequest request,
        AppDbContext dbContext,
        TimeProvider timeProvider)
    {
        var auction = await dbContext.FindAsync<Auction>(request.AuctionId);
        if (auction is null)
        {
            return TypedResults.NotFound();
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

        auction.Bids.Add(new Bid(request.BidAmount, timeProvider.GetUtcNow(), default));

        dbContext.Auctions.Update(auction);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok();
    }
}
