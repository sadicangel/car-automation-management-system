using System.ComponentModel.DataAnnotations;

namespace CarAutomation.WebApi.Auctioning;

public record StartAuctionRequest(Guid VehicleId) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (VehicleId == Guid.Empty)
        {
            yield return new ValidationResult($"Vehicle ID must be a valid identifier", [nameof(VehicleId)]);
        }
    }
}

public record StartAuctionResponse(
    Guid AuctionId,
    Guid VehicleId,
    decimal StartingBid);
