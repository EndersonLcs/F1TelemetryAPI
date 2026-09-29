using F1Telemetry.Domain.Entities;
using F1Telemetry.Infrastructure.Repositories;
using F1Telemetry.Tests.Fixtures;
using FluentAssertions;

namespace F1Telemetry.Tests.Repositories;

// A annotation IClassFixture garante que o banco suba antes do primeiro teste e caia no final
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
        int sessionKey = 101;
        
        // Criando piloto
        var hamilton = new Driver { DriverNumber = 44, FullName = "Lewis Hamilton", TeamName = "Scuderia Ferrari" };
        var leclerc = new Driver { DriverNumber = 16, FullName = "Charles Leclerc", TeamName = "Scuderia Ferrari" };
        
        // Criando sessão
        var session = new Session { SessionKey = sessionKey, SessionName = "Race", MeetingKey = 1, DateStart = DateTime.UtcNow };

        // Limpa o banco (caso outros testes rodem em paralelo) e insere a massa
        _fixture.DbContext.LapTimes.RemoveRange(_fixture.DbContext.LapTimes);
        _fixture.DbContext.Drivers.AddRange(hamilton, leclerc);
        _fixture.DbContext.Sessions.Add(session);
        
        // Inserindo 3 voltas. A segunda volta do Hamilton deve ser a mais rápida!
        _fixture.DbContext.LapTimes.AddRange(
            new LapTime { Id = Guid.NewGuid(), DriverNumber = 16, SessionKey = sessionKey, LapNumber = 1, LapDurationMs = 85000 },
            new LapTime { Id = Guid.NewGuid(), DriverNumber = 44, SessionKey = sessionKey, LapNumber = 2, LapDurationMs = 81500 }, // Mais rápida
            new LapTime { Id = Guid.NewGuid(), DriverNumber = 44, SessionKey = sessionKey, LapNumber = 3, LapDurationMs = 82200 }
        );
        
        await _fixture.DbContext.SaveChangesAsync();

        // Act: Executa o método real no repositório
        var result = await repository.GetFastestLapBySessionAsync(sessionKey);

        // Assert: Verifica se o banco retornou a volta correta de 81.500 ms (1:21.500)
        result.Should().NotBeNull();
        result!.DriverNumber.Should().Be(44);
        result.LapDurationMs.Should().Be(81500);
        result.Driver.FullName.Should().Be("Lewis Hamilton");
    }
}
