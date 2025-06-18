using CarAutomation.Domain;
using CarAutomation.ServiceDefaults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AppDbContext>("car-automation-management-system");

var app = builder.Build();

await app.StartAsync();

using (var scope = app.Services.CreateScope())
{
    // This would perform actual migrations.
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

await app.StopAsync();
