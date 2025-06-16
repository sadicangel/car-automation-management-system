namespace CarAutomation.WebApi.Vehicles;

public record AddVehicleRequest(
    string Vin,
    VehicleType Type,
    string Manufacturer,
    string Model,
    int Year,
    int? NumberOfDoors,
    int? NumberOfSeats,
    double? LoadCapacity,
    decimal StartingBid);


