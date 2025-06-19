using System.ComponentModel.DataAnnotations;

namespace CarAutomation.WebApi.Auctions.PlaceBid;

public record PlaceBidRequest(Guid AuctionId, decimal BidAmount, string UserEmail) : IValidatableObject
{
    private static readonly EmailAddressAttribute s_emailAddressValidator = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BidAmount < 0)
        {
            yield return new ValidationResult($"Bid amount must be a positive number", [nameof(BidAmount)]);
        }

        if (!s_emailAddressValidator.IsValid(UserEmail))
        {
            yield return new ValidationResult($"Invalid email '{UserEmail}'", [nameof(UserEmail)]);
        }
    }
}
