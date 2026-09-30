using F1Telemetry.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace F1Telemetry.Tests.Setup;

// IAsyncLifetime permite que o xUnit execute código assíncrono antes e depois dos testes
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Configura o contêiner do PostgreSQL (mesma versão que você usa em dev)
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .WithDatabase("f1telemetry_integration")
        .WithUsername("test_qa")
        .WithPassword("test_password")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. Procura a configuração atual do banco de dados e a remove
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<F1TelemetryDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // 2. Adiciona o DbContext apontando para o contêiner efêmero do Docker
            services.AddDbContext<F1TelemetryDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });
        });
    }

    // Roda ANTES de todos os testes começarem
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    // Roda DEPOIS que todos os testes terminarem (Destrói o banco)
    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
