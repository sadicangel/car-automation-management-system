namespace CarAutomation.WebApi.Auctions.StartAuction;

public record StartAuctionResponse(Guid AuctionId, Guid VehicleId, decimal StartingBidEur);
