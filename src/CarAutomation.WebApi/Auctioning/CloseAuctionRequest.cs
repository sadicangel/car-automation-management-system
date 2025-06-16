using System.ComponentModel.DataAnnotations;

namespace CarAutomation.WebApi.Auctioning;

public record CloseAuctionRequest(Guid AuctionId) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (AuctionId == Guid.Empty)
        {
            yield return new ValidationResult($"Auction ID must be a valid identifier", [nameof(AuctionId)]);
        }
    }
}
