using CarAutomation.WebApi.Auctions.CloseAuction;
using CarAutomation.WebApi.Auctions.PlaceBid;
using CarAutomation.WebApi.Auctions.StartAuction;

namespace CarAutomation.WebApi.Auctions;

public static class AuctionsEndpoints
{
    public static IEndpointConventionBuilder MapAuctionsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var actioning = endpoints
            .MapGroup("/auctioning")
            .WithValidation()
            .WithTags(nameof(AuctionsEndpoints));

        actioning.MapPost("/start", StartAuctionEndpoint.StartAuction);

        actioning.MapPost("/close", CloseAuctionEndpoint.CloseAuction);

        actioning.MapPost("/bid", PlaceBidEndpoint.PlaceBid);

        return actioning;
    }
}
