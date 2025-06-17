namespace CarAutomation.WebApi.Vehicles;

public record class SearchVehiclesRequest(VehicleType? Type = null, string? Manufacturer = null, string? Model = null, int? Year = null);

