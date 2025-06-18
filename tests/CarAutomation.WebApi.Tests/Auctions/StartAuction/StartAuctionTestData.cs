using CarAutomation.Domain.Vehicles;

namespace CarAutomation.WebApi.Tests.Auctions.StartAuction;

public static class StartAuctionTestData
{
    public static Vehicle Vehicle { get; } = new Sedan
    {
        Id = new Guid("B6046201-52E4-4998-95D9-14D54FBD801F"),
        Vin = "3FAHP0HA6AR123456",
        Manufacturer = "Honda",
        Model = "Accord",
        Year = 2021,
        NumberOfDoors = 4,
        StartingBidEur = 12000.00m,
    };
}
