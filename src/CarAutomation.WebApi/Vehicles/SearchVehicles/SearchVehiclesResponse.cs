using CarAutomation.Domain.Vehicles;

namespace CarAutomation.WebApi.Vehicles.SearchVehicles;

public record SearchVehiclesResponse(IEnumerable<SearchVehiclesLine> Vehicles);

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
    decimal StartingBidEur)
{
    public static SearchVehiclesLine FromVehicle(Vehicle vehicle)
    {
        return vehicle switch
        {
            Sedan sedan => new(
                sedan.Id,
                sedan.Vin,
                VehicleType.Sedan,
                sedan.Manufacturer,
                sedan.Model,
                sedan.Year,
                sedan.NumberOfDoors,
                NumberOfSeats: null,
                LoadCapacityKg: null,
                sedan.StartingBidEur),

            Hatchback hatchback => new(
                hatchback.Id,
                hatchback.Vin,
                VehicleType.Hatchback,
                hatchback.Manufacturer,
                hatchback.Model,
                hatchback.Year,
                hatchback.NumberOfDoors,
                NumberOfSeats: null,
                LoadCapacityKg: null,
                hatchback.StartingBidEur),

            Suv suv => new(
                suv.Id,
                suv.Vin,
                VehicleType.Suv,
                suv.Manufacturer,
                suv.Model,
                suv.Year,
                NumberOfDoors: null,
                suv.NumberOfSeats,
                LoadCapacityKg: null,
                suv.StartingBidEur),

            Truck truck => new(
                truck.Id,
                truck.Vin,
                VehicleType.Truck,
                truck.Manufacturer,
                truck.Model,
                truck.Year,
                NumberOfDoors: null,
                NumberOfSeats: null,
                truck.LoadCapacityKg,
                truck.StartingBidEur),

            _ => throw new InvalidOperationException("Unknown vehicle type")
        };
    }
}
