namespace WebApi.VehicleCatalog;

public record class SearchVehiclesRequest(VehicleType? Type, string? Manufacturer, string? Model, int? Year);

