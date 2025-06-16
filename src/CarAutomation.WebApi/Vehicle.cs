namespace CarAutomation.WebApi;

public enum VehicleType { Sedan, Suv, Hatchback, Truck }

public record Vehicle(
    Guid Id,
    string Vin,
    VehicleType Type,
    string Manufacturer,
    string Model,
    int Year,
    int? NumberOfDoors,
    int? NumberOfSeats,
    double? LoadCapacity,
    decimal StartingBid);
