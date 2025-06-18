using System.ComponentModel.DataAnnotations;
using CarAutomation.Domain.Vehicles;

namespace CarAutomation.WebApi.Vehicles.AddVehicle;

public record AddVehicleRequest(
    string Vin,
    VehicleType Type,
    string Manufacturer,
    string Model,
    int Year,
    int? NumberOfDoors,
    int? NumberOfSeats,
    double? LoadCapacityKg,
    decimal StartingBidEur)
    : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Vin is not { Length: 17 } str || str.Any(x => !char.IsAsciiLetterOrDigit(x)))
            yield return new ValidationResult("VIN must be a 17-character alphanumeric code", [nameof(Vin)]);

        if (string.IsNullOrWhiteSpace(Manufacturer))
            yield return new ValidationResult("Manufacturer cannot be null or whitespace", [nameof(Manufacturer)]);

        if (string.IsNullOrWhiteSpace(Model))
            yield return new ValidationResult("Model cannot be null or whitespace", [nameof(Manufacturer)]);

        if (Year <= 0)
            yield return new ValidationResult("Year must be a positive value", [nameof(Year)]);

        if (StartingBidEur <= 0)
            yield return new ValidationResult("Start bid must be a positive value", [nameof(StartingBidEur)]);

        if (Type is VehicleType.Sedan or VehicleType.Hatchback && NumberOfDoors is not > 0)
            yield return new ValidationResult($"'{Type}' vehicles must specify a number of doors greater than 0");

        if (Type is VehicleType.Suv && NumberOfSeats is not > 0)
            yield return new ValidationResult($"'{Type}' vehicles must specify a number of seats greater than 0");

        if (Type is VehicleType.Truck && LoadCapacityKg is not > 0)
            yield return new ValidationResult($"'{Type}' vehicles must specify a load capacity greater than 0");
    }
}
