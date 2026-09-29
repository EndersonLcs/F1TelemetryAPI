using F1Telemetry.Domain.Entities;

namespace F1Telemetry.Domain.Interfaces;

public interface ILapTimeRepository
{
    Task<IEnumerable<LapTime>> GetLapTimesBySessionAsync(int sessionKey);
    Task<LapTime?> GetFastestLapBySessionAsync(int sessionKey);
}
