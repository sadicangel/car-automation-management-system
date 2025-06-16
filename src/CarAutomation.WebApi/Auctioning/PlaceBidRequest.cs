namespace WebApi.Auctioning;

public record PlaceBidRequest(Guid AuctionId, decimal BidAmount);
