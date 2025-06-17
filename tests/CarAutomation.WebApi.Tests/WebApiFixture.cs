using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace CarAutomation.WebApi.Tests;

public sealed class WebApiFixture(PostgreSqlFixture postgreSqlFixture) : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var connectionString = new NpgsqlConnectionStringBuilder(postgreSqlFixture.Container.GetConnectionString())
        {
            Database = "car_automation_management_system"
        }.ToString();

        builder.ConfigureHostConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:car-automation-management-system"] = connectionString
        }));

        return base.CreateHost(builder);
    }
}
