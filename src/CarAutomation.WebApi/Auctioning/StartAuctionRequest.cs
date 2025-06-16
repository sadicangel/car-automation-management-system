namespace CarAutomation.WebApi.Auctioning;

public record StartAuctionRequest(Guid VehicleId, DateTimeOffset StartDate, DateTimeOffset EndDate);

public record StartAuctionResponse(
    Guid AuctionId,
    Guid VehicleId,
    decimal StartingBid,
    bool IsActive,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    List<Bid> Bids);
