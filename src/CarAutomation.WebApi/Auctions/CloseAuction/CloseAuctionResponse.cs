namespace CarAutomation.WebApi.Auctions.CloseAuction;

public record CloseAuctionResponse(
    Guid AuctionId,
    Guid VehicleId,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    decimal? HighestBidEur);
