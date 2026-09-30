using System.Net;
using System.Net.Http.Json;
using F1Telemetry.Domain.Entities;
using F1Telemetry.Infrastructure.Data;
using F1Telemetry.Tests.Setup;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace F1Telemetry.Tests.IntegrationTests;

// IClassFixture injeta o nosso Testcontainer (o banco no Docker)
public class HistoricalControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HistoricalControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();

        // Cria a estrutura de tabelas no banco de dados efémero antes de testar
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<F1TelemetryDbContext>();
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task Should_Execute_Historical_Data_Journey_Successfully()
    {
        // 1. Arrange: Executar o Seed para popular o banco de QA
        var seedResponse = await _client.PostAsync("/api/historical/seed", null);
        seedResponse.EnsureSuccessStatusCode();

        // 2. Act & Assert (RF03): Consulta do Arquivo de Temporadas
        var seasonsResponse = await _client.GetAsync("/api/historical/seasons");
        seasonsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var seasons = await seasonsResponse.Content.ReadFromJsonAsync<List<Season>>();
        seasons.Should().NotBeNull().And.HaveCount(1);
        seasons!.First().Year.Should().Be(2024);
        seasons.First().GrandPrixes.First().CountryName.Should().Be("Brazil");

        // 3. Act & Assert (RF02): Histórico do Fim de Semana (Sessões do GP)
        int gpMeetingKey = seasons.First().GrandPrixes.First().MeetingKey;
        var sessionsResponse = await _client.GetAsync($"/api/historical/grandprix/{gpMeetingKey}/sessions");
        sessionsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var sessions = await sessionsResponse.Content.ReadFromJsonAsync<List<Session>>();
        sessions.Should().NotBeNull().And.HaveCount(1);
        sessions!.First().SessionKey.Should().Be(9158);
        sessions.First().SessionName.Should().Be("Practice 1");
    }
}
