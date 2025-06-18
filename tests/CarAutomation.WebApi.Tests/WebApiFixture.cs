using System.Security.Cryptography;
using CarAutomation.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace CarAutomation.WebApi.Tests;

public sealed class WebApiFixture(PostgreSqlFixture postgreSqlFixture) : WebApplicationFactory<Program>
{
    public Action<AppDbContext>? ConfigureDbContext { get; set; }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var connectionString = new NpgsqlConnectionStringBuilder(postgreSqlFixture.Container.GetConnectionString())
        {
            Database = RandomNumberGenerator.GetHexString(16)
        }.ToString();

        builder.ConfigureHostConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:car-automation-management-system"] = connectionString
        }));

        var host = base.CreateHost(builder);

        using (var scope = host.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();
            ConfigureDbContext?.Invoke(dbContext);
        }

        return host;
    }
}
