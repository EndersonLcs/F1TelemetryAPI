using F1Telemetry.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace F1Telemetry.Tests.Fixtures;

public class DatabaseFixture : IAsyncLifetime
{
    // Configura o contêiner do PostgreSQL via Testcontainers
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("f1telemetry_test")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    public F1TelemetryDbContext DbContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // 1. Sobe o contêiner no Docker
        await _dbContainer.StartAsync();

        // 2. Cria a string de conexão dinâmica
        var options = new DbContextOptionsBuilder<F1TelemetryDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
            .Options;

        DbContext = new F1TelemetryDbContext(options);

        // 3. Aplica as migrations para criar as tabelas no banco de teste
        await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        // Limpa tudo e destrói o contêiner após os testes
        await DbContext.DisposeAsync();
        await _dbContainer.StopAsync();
    }
}
