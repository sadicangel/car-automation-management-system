namespace CarAutomation.Domain.Vehicles;

public abstract record Vehicle
{
    public Guid Id { get; init; }
    public required string Vin { get; init; }
    public required string Manufacturer { get; init; }
    public required string Model { get; init; }
    public required int Year { get; init; }
    public required decimal StartingBidEur { get; init; }
}

public record Sedan : Vehicle
{
    public required int NumberOfDoors { get; init; }
}

public record Hatchback : Vehicle
{
    public required int NumberOfDoors { get; init; }
}

public record Suv : Vehicle
{
    public required int NumberOfSeats { get; init; }
}

public record Truck : Vehicle
{
    public required double LoadCapacityKg { get; init; }
}
