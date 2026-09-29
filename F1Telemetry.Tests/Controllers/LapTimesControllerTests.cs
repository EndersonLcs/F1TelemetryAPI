using F1Telemetry.API.Controllers;
using F1Telemetry.Domain.Entities;
using F1Telemetry.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace F1Telemetry.Tests.Controllers;

public class LapTimesControllerTests
{
    private readonly Mock<ILapTimeRepository> _repositoryMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly LapTimesController _controller;

    public LapTimesControllerTests()
    {
        _repositoryMock = new Mock<ILapTimeRepository>();
        _cacheMock = new Mock<IDistributedCache>(); 
        
        // Injetamos ambos os mocks no controller
        _controller = new LapTimesController(_repositoryMock.Object, _cacheMock.Object);
    }

    [Fact(DisplayName = "GetFastestLap deve retornar 404 NotFound quando sessão não tiver voltas")]
    public async Task GetFastestLap_WhenNoLapsExist_ReturnsNotFound()
    {
        // Arrange
        int sessionKey = 9999;
        
        _repositoryMock.Setup(r => r.GetFastestLapBySessionAsync(sessionKey))
            .ReturnsAsync((LapTime?)null);

        // Act
        var result = await _controller.GetFastestLap(sessionKey);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)result;
        notFoundResult.Value.Should().BeEquivalentTo(new { message = "Nenhuma volta encontrada para esta sessão." });
    }

    [Fact(DisplayName = "GetFastestLap deve retornar 200 OK com a volta quando encontrar dados")]
    public async Task GetFastestLap_WhenLapsExist_ReturnsOkWithLap()
    {
        // Arrange
        int sessionKey = 9158;
        var fastestLap = new LapTime 
        { 
            Id = Guid.NewGuid(), 
            SessionKey = sessionKey, 
            DriverNumber = 44, 
            LapDurationMs = 82000 
        };

        _repositoryMock.Setup(r => r.GetFastestLapBySessionAsync(sessionKey))
            .ReturnsAsync(fastestLap);

        // Act
        var result = await _controller.GetFastestLap(sessionKey);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeEquivalentTo(fastestLap);
    }
}
