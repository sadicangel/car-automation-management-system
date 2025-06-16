using System.ComponentModel.DataAnnotations;

namespace CarAutomation.WebApi.Auctioning;

public record PlaceBidRequest(Guid AuctionId, decimal BidAmount) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (AuctionId == Guid.Empty)
        {
            yield return new ValidationResult($"Auction ID must be a valid identifier", [nameof(AuctionId)]);
        }

        if (BidAmount < 0)
        {
            yield return new ValidationResult($"Bid amount must be a positive number", [nameof(BidAmount)]);
        }
    }
}
