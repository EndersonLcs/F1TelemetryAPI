using F1Telemetry.Domain.Entities;
using F1Telemetry.Infrastructure.Repositories;
using F1Telemetry.Tests.Fixtures;
using FluentAssertions;

namespace F1Telemetry.Tests.Repositories;

public class LapTimeRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public LapTimeRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Deve retornar a volta mais rápida corretamente do banco de dados real")]
    public async Task GetFastestLapBySessionAsync_ReturnsFastestLap()
    {
        // Arrange: Prepara os dados no banco de teste
        var repository = new LapTimeRepository(_fixture.DbContext);
        int testMeetingKey = 999;
        int sessionKey = 101;
        
        // 1. Criando a hierarquia histórica
        var season = new Season { Year = 2025 };
        var gp = new GrandPrix { MeetingKey = testMeetingKey, SeasonYear = 2025, RoundNumber = 1, CountryName = "Australia", CircuitShortName = "Albert Park" };

        // 2. Criando pilotos
        var hamilton = new Driver { DriverNumber = 44, FullName = "Lewis Hamilton", TeamName = "Scuderia Ferrari" };
        var leclerc = new Driver { DriverNumber = 16, FullName = "Charles Leclerc", TeamName = "Scuderia Ferrari" };
        
        // 3. Criando sessão vinculada ao GP
        var session = new Session { SessionKey = sessionKey, SessionName = "Race", MeetingKey = testMeetingKey, DateStart = DateTime.UtcNow };

        // Limpa o banco para evitar conflitos de IDs únicos em testes paralelos
        _fixture.DbContext.LapTimes.RemoveRange(_fixture.DbContext.LapTimes);
        _fixture.DbContext.Sessions.RemoveRange(_fixture.DbContext.Sessions);
        _fixture.DbContext.Drivers.RemoveRange(_fixture.DbContext.Drivers);
        _fixture.DbContext.GrandPrixes.RemoveRange(_fixture.DbContext.GrandPrixes);
        _fixture.DbContext.Seasons.RemoveRange(_fixture.DbContext.Seasons);
        await _fixture.DbContext.SaveChangesAsync();

        // Insere a massa de dados estruturada
        _fixture.DbContext.Seasons.Add(season);
        _fixture.DbContext.GrandPrixes.Add(gp);
        _fixture.DbContext.Drivers.AddRange(hamilton, leclerc);
        _fixture.DbContext.Sessions.Add(session);
        
        // Inserindo 3 voltas. A segunda volta do Hamilton deve ser a mais rápida
        _fixture.DbContext.LapTimes.AddRange(
            new LapTime { Id = Guid.NewGuid(), DriverNumber = 16, SessionKey = sessionKey, LapNumber = 1, LapDurationMs = 85000 },
            new LapTime { Id = Guid.NewGuid(), DriverNumber = 44, SessionKey = sessionKey, LapNumber = 2, LapDurationMs = 81500 },
            new LapTime { Id = Guid.NewGuid(), DriverNumber = 44, SessionKey = sessionKey, LapNumber = 3, LapDurationMs = 82200 }
        );
        
        await _fixture.DbContext.SaveChangesAsync();

        // Act: Executa o método real no repositório
        var result = await repository.GetFastestLapBySessionAsync(sessionKey);

        // Assert: Verifica se o banco retornou a volta correta
        result.Should().NotBeNull();
        result!.DriverNumber.Should().Be(44);
        result.LapDurationMs.Should().Be(81500);
        result.Driver.FullName.Should().Be("Lewis Hamilton");
    }
}
