using CarAutomation.Domain.Vehicles;
using CarAutomation.WebApi.Vehicles.AddVehicle;

namespace CarAutomation.WebApi.Tests.Vehicles.AddVehicles;

public static class TestData
{
    public static readonly AddVehicleRequest SedanRequest = new(
        Vin: "3FAHP0HA6AR123456",
        Type: VehicleType.Sedan,
        Manufacturer: "Honda",
        Model: "Accord",
        Year: 2021,
        NumberOfDoors: 4,
        NumberOfSeats: null,
        LoadCapacityKg: null,
        StartingBidEur: 12000.00m
    );

    public static readonly AddVehicleRequest HatchbackRequest = new(
        Vin: "WVWZZZ1KZAW000123",
        Type: VehicleType.Hatchback,
        Manufacturer: "Volkswagen",
        Model: "Golf",
        Year: 2020,
        NumberOfDoors: 5,
        NumberOfSeats: null,
        LoadCapacityKg: null,
        StartingBidEur: 9500.00m
    );

    public static readonly AddVehicleRequest TruckRequest = new(
        Vin: "1FTFW1ET1EFA12345",
        Type: VehicleType.Truck,
        Manufacturer: "Ford",
        Model: "F-150",
        Year: 2023,
        NumberOfDoors: null,
        NumberOfSeats: null,
        LoadCapacityKg: 1200.0,
        StartingBidEur: 25000.00m
    );

    public static readonly AddVehicleRequest SuvRequest = new(
        Vin: "1HGCM82633A004352",
        Type: VehicleType.Suv,
        Manufacturer: "Toyota",
        Model: "RAV4",
        Year: 2022,
        NumberOfDoors: null,
        NumberOfSeats: 5,
        LoadCapacityKg: null,
        StartingBidEur: 15000.00m
    );
}
