using CarAutomation.Domain.Auctions;
using CarAutomation.Domain.Vehicles;

namespace CarAutomation.WebApi.Tests.Auctions.CloseAuction;

public static class CloseAuctionTestData
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

    public static Auction ActiveAuction { get; } = new Auction
    {
        AuctionId = new Guid("2CE02E73-A353-4FB2-965D-A7E2FF01D79C"),
        VehicleId = new Guid("B6046201-52E4-4998-95D9-14D54FBD801F"),
        StartDate = DateTimeOffset.UtcNow.AddDays(-1),
        IsActive = true,
        StartingBidEur = 12000.00m,
    };

    public static Auction ClosedAuction { get; } = new Auction
    {
        AuctionId = new Guid("ADD6E186-AE70-4D2F-86C0-EAF8AE8AE10B"),
        VehicleId = new Guid("B6046201-52E4-4998-95D9-14D54FBD801F"),
        StartDate = DateTimeOffset.UtcNow.AddDays(-10),
        EndDate = DateTimeOffset.UtcNow.AddDays(-5),
        IsActive = false,
        StartingBidEur = 12000.00m,
    };
}
