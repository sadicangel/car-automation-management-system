using CarAutomation.Domain.Vehicles;

namespace CarAutomation.WebApi.Vehicles.SearchVehicles;

public record class SearchVehiclesRequest(VehicleType? Type = null, string? Manufacturer = null, string? Model = null, int? Year = null);

