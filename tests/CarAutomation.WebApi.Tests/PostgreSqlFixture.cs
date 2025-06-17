using CarAutomation.WebApi.Tests;
using Testcontainers.PostgreSql;

[assembly: AssemblyFixture(typeof(PostgreSqlFixture))]

namespace CarAutomation.WebApi.Tests;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; } = new PostgreSqlBuilder().Build();

    public async ValueTask InitializeAsync() => await Container.StartAsync();

    public async ValueTask DisposeAsync() => await Container.StopAsync();
}
