namespace CarAutomation.WebApi.Auctioning;

public record StartAuctionRequest(Guid VehicleId);

public record StartAuctionResponse(
    Guid AuctionId,
    Guid VehicleId,
    decimal StartingBidEur);
