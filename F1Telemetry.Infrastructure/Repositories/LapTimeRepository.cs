using F1Telemetry.Domain.Entities;
using F1Telemetry.Domain.Interfaces;
using F1Telemetry.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace F1Telemetry.Infrastructure.Repositories;

public class LapTimeRepository : ILapTimeRepository
{
    private readonly F1TelemetryDbContext _context;

    public LapTimeRepository(F1TelemetryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LapTime>> GetLapTimesBySessionAsync(int sessionKey)
    {
        return await _context.LapTimes
            .Include(l => l.Driver) // Traz os dados do piloto junto
            .Where(l => l.SessionKey == sessionKey)
            .OrderBy(l => l.LapNumber)
            .ToListAsync();
    }

    public async Task<LapTime?> GetFastestLapBySessionAsync(int sessionKey)
    {
        return await _context.LapTimes
            .Include(l => l.Driver)
            .Where(l => l.SessionKey == sessionKey)
            .OrderBy(l => l.LapDurationMs) // Ordena do menor tempo para o maior
            .FirstOrDefaultAsync(); // Pega apenas o primeiro (o mais rápido)
    }
}
