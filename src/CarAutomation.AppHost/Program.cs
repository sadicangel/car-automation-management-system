var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithPgWeb();
var database = postgres.AddDatabase("car-automation-management-system", "car_automation_management_system");

var dbMigration = builder.AddProject<Projects.CarAutomation_DbMigration>("db-migration")
    .WithReference(database)
    .WaitFor(database);

builder.AddProject<Projects.CarAutomation_WebApi>("webapi")
    .WithReference(database)
    .WaitFor(database)
    .WaitFor(dbMigration);

builder.Build().Run();
