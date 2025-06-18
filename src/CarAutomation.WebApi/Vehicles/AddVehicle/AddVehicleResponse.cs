using CarAutomation.Domain.Vehicles;

namespace CarAutomation.WebApi.Vehicles.AddVehicle;

public record AddVehicleResponse(
    Guid Id,
    string Vin,
    VehicleType Type,
    string Manufacturer,
    string Model,
    int Year,
    int? NumberOfDoors,
    int? NumberOfSeats,
    double? LoadCapacityKg,
    decimal StartingBidEur)
{
    public static AddVehicleResponse From(Vehicle vehicle) => new(
        vehicle.Id,
        vehicle.Vin,
        vehicle.Type,
        vehicle.Manufacturer,
        vehicle.Model,
        vehicle.Year,
        vehicle.NumberOfDoors,
        vehicle.NumberOfSeats,
        vehicle.LoadCapacityKg,
        vehicle.StartingBidEur);
}
