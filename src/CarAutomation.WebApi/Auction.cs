namespace CarAutomation.WebApi;

public record Auction(
    Guid AuctionId,
    Guid VehicleId,
    decimal StartingBidEur,
    bool IsActive,
    DateTimeOffset StartDate,
    DateTimeOffset? EndDate)
{
    public bool IsActive { get; set; } = IsActive;
    public Vehicle Vehicle { get; init; } = default!;
    public List<Bid> Bids { get; init; } = [];

}

public record Bid(decimal Amount, DateTimeOffset Timestamp, Guid BidderId);
