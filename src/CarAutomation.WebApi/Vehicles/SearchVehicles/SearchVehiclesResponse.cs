using CarAutomation.Domain.Vehicles;

namespace CarAutomation.WebApi.Vehicles.SearchVehicles;

public record SearchVehiclesResponse(List<SearchVehiclesLine> Vehicles)
{
    public static SearchVehiclesResponse From(List<Vehicle> vehicles) => new([.. vehicles.Select(vehicle => new SearchVehiclesLine(
        Id: vehicle.Id,
        Vin: vehicle.Vin,
        Type: vehicle.Type,
        Manufacturer: vehicle.Manufacturer,
        Model: vehicle.Model,
        Year: vehicle.Year,
        NumberOfDoors: vehicle.NumberOfDoors,
        NumberOfSeats: vehicle.NumberOfSeats,
        LoadCapacityKg: vehicle.LoadCapacityKg,
        StartingBidEur: vehicle.StartingBidEur))]);
}

public record SearchVehiclesLine(
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
