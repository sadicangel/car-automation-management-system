var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithPgWeb();
var database = postgres.AddDatabase("car-automation-management-system", "car_automation_management_system");

builder.AddProject<Projects.CarAutomation_WebApi>("webapi")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();
