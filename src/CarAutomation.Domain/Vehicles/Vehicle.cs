namespace CarAutomation.Domain.Vehicles;

public record Vehicle(
    Guid Id,
    string Vin,
    VehicleType Type,
    string Manufacturer,
    string Model,
    int Year,
    int? NumberOfDoors,
    int? NumberOfSeats,
    double? LoadCapacityKg,
    decimal StartingBidEur);
