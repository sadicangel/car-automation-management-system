using CarAutomation.Domain.Vehicles;

namespace CarAutomation.Domain.Auctions;

public record Auction
{
    public Guid AuctionId { get; init; }
    public required Guid VehicleId { get; init; }
    public Vehicle Vehicle { get; init; } = default!;
    public required decimal StartingBidEur { get; init; }
    public required DateTimeOffset StartDate { get; init; }
    public DateTimeOffset? EndDate { get; set; }
    public bool IsActive { get; set; }
    public List<Bid> Bids { get; init; } = [];
}

public record Bid(decimal Amount, DateTimeOffset Timestamp, Guid BidderId);
