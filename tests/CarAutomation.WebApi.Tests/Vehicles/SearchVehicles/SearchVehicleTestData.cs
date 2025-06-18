using CarAutomation.Domain.Vehicles;

namespace CarAutomation.WebApi.Tests.Vehicles.SearchVehicles;

public static class SearchVehicleTestData
{
    public static IReadOnlyList<Vehicle> Vehicles { get; } =
    [
        new Sedan
        {
            Vin = "3FAHP0HA6AR123456",
            Manufacturer = "Honda",
            Model = "Accord",
            Year = 2021,
            NumberOfDoors = 4,
            StartingBidEur = 12000.00m,
        },
        new Sedan
        {
            Vin = "2HGFC2F69KH123456",
            Manufacturer = "Honda",
            Model = "Civic",
            Year = 2022,
            NumberOfDoors = 4,
            StartingBidEur = 11500.00m,
        },
        new Sedan
        {
            Vin = "1N4AL3AP4GC123789",
            Manufacturer = "Nissan",
            Model = "Altima",
            Year = 2020,
            NumberOfDoors = 4,
            StartingBidEur = 11000.00m,
        },
        new Hatchback
        {
            Vin = "WVWZZZ1KZAW000123",
            Manufacturer = "Volkswagen",
            Model = "Golf",
            Year = 2020,
            NumberOfDoors = 5,
            StartingBidEur = 9500.00m,
        },
        new Hatchback
        {
            Vin = "JTDKN3DU1F0456789",
            Manufacturer = "Toyota",
            Model = "Yaris",
            Year = 2019,
            NumberOfDoors = 5,
            StartingBidEur = 8700.00m,
        },
        new Suv
        {
            Vin = "1HGCM82633A004352",
            Manufacturer = "Toyota",
            Model = "RAV4",
            Year = 2022,
            NumberOfSeats = 5,
            StartingBidEur = 15000.00m,
        },
        new Suv
        {
            Vin = "JN8AS5MT1CW123456",
            Manufacturer = "Nissan",
            Model = "Qashqai",
            Year = 2021,
            NumberOfSeats = 5,
            StartingBidEur = 14250.00m,
        },
        new Truck
        {
            Vin = "1FTFW1ET1EFA12345",
            Manufacturer = "Ford",
            Model = "F-150",
            Year = 2023,
            LoadCapacityKg = 1200.0,
            StartingBidEur = 25000.00m,
        },
        new Truck
        {
            Vin = "2GC2KVEG2G1234567",
            Manufacturer = "Chevrolet",
            Model = "Silverado",
            Year = 2021,
            LoadCapacityKg = 1350.0,
            StartingBidEur = 27000.00m,
        },
    ];
}
