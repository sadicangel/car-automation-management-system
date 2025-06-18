using CarAutomation.Domain;
using CarAutomation.Domain.Auctions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CarAutomation.WebApi.Auctions.CloseAuction;

public static class CloseAuctionEndpoint
{
    public static async Task<Results<Ok, NotFound, ValidationProblem>> CloseAuction(
        CloseAuctionRequest request,
        AppDbContext dbContext)
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

        auction.IsActive = false;

        dbContext.Update(auction);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok();
    }
}
