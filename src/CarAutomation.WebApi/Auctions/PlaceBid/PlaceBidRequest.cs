using System.ComponentModel.DataAnnotations;

namespace CarAutomation.WebApi.Auctions.PlaceBid;

public record PlaceBidRequest(Guid AuctionId, decimal BidAmount) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BidAmount < 0)
            yield return new ValidationResult($"Bid amount must be a positive number", [nameof(BidAmount)]);
    }
}
