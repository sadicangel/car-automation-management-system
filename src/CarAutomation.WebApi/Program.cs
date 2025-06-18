using System.Text.Json.Serialization;
using CarAutomation.Domain;
using CarAutomation.ServiceDefaults;
using CarAutomation.WebApi.Auctions;
using CarAutomation.WebApi.Vehicles;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<AppDbContext>("car-automation-management-system");

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(opts => opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapVehiclesEndpoints();
app.MapAuctionsEndpoints();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

app.Run();

public partial class Program;
