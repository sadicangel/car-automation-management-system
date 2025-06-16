using Microsoft.AspNetCore.Http.HttpResults;

namespace CarAutomation.WebApi.Auctioning;

public static class AuctioningEndpoints
{
    public static IEndpointConventionBuilder MapAuctioningEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var actioning = endpoints
            .MapGroup("/auctioning")
            .WithValidation();

        actioning.MapPost("/start", StartAuction);

        actioning.MapPost("/close", CloseAuction);

        actioning.MapPost("/bid", PlaceBid);

        return actioning;
    }

    private static async Task<Results<Ok<StartAuctionResponse>, ValidationProblem>> StartAuction(
        StartAuctionRequest request,
        AppDbContext dbContext,
        TimeProvider timeProvider)
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

        var auction = new Auction(
            AuctionId: default,
            VehicleId: vehicle.Id,
            StartingBid: vehicle.StartingBid,
            IsActive: true,
            StartDate: timeProvider.GetUtcNow(),
            EndDate: null);

        dbContext.Auctions.Add(auction);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(new StartAuctionResponse(
            auction.AuctionId,
            auction.VehicleId,
            auction.StartingBid));
    }

    private static async Task<Results<Ok, ValidationProblem>> CloseAuction(
        CloseAuctionRequest request,
        AppDbContext dbContext)
    {
        var auction = await dbContext.FindAsync<Auction>(request.AuctionId);
        if (auction is null)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["AuctionId"] = [$"Auction '{request.AuctionId}' does not exist"]
            });
        }

        if (!auction.IsActive)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["AuctionId"] = [$"Auction '{request.AuctionId}' is not active"]
            });
        }

        auction.IsActive = false;

        dbContext.Update(auction);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, ValidationProblem>> PlaceBid(
        PlaceBidRequest request,
        AppDbContext dbContext,
        TimeProvider timeProvider)
    {
        var auction = await dbContext.FindAsync<Auction>(request.AuctionId);
        if (auction is null)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(PlaceBidRequest.AuctionId)] = [$"Auction '{request.AuctionId}' does not exist"]
            });
        }

        if (request.BidAmount < auction.StartingBid)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(PlaceBidRequest.BidAmount)] = [$"Bid amount cannot be less than starting bid ({auction.StartingBid})"]
            });
        }

        if (auction.Bids.Count > 0)
        {
            var currentHighestBid = auction.Bids.Select(x => x.Amount).Max();

            if (request.BidAmount <= currentHighestBid)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(PlaceBidRequest.BidAmount)] = [$"Bid amount must be higher than the current highest bid ({currentHighestBid})"]
                });
            }
        }

        auction.Bids.Add(new Bid(request.BidAmount, timeProvider.GetUtcNow(), default));

        dbContext.Auctions.Update(auction);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok();
    }
}
