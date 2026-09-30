using F1Telemetry.Domain.Entities;
using F1Telemetry.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace F1Telemetry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HistoricalController : ControllerBase
{
    private readonly F1TelemetryDbContext _context;

    public HistoricalController(F1TelemetryDbContext context)
    {
        _context = context;
    }

    // RF03: Arquivo de Temporadas e seus GPs
    [HttpGet("seasons")]
    public async Task<IActionResult> GetSeasons()
    {
        var seasons = await _context.Seasons
            .Include(s => s.GrandPrixes)
            .OrderByDescending(s => s.Year)
            .AsNoTracking() // Aumenta a performance pois é apenas leitura
            .ToListAsync();

        return Ok(seasons);
    }

    // RF02: Histórico do Fim de Semana (Sessões de um GP)
    [HttpGet("grandprix/{meetingKey}/sessions")]
    public async Task<IActionResult> GetSessionsByGrandPrix(int meetingKey)
    {
        var sessions = await _context.Sessions
            .Where(s => s.MeetingKey == meetingKey)
            .OrderBy(s => s.DateStart)
            .AsNoTracking()
            .ToListAsync();

        if (!sessions.Any())
            return NotFound($"Nenhuma sessão encontrada para o MeetingKey {meetingKey}.");

        return Ok(sessions);
    }

    // Rota utilitária de QA para popular o banco e permitir os testes do Worker
    [HttpPost("seed")]
    public async Task<IActionResult> SeedTestData()
    {
        if (await _context.Seasons.AnyAsync()) 
            return BadRequest("O banco de dados já possui dados iniciais.");

        // Criamos a Temporada 2024
        var season = new Season { Year = 2024 };
        
        // Criamos o GP do Brasil
        var gp = new GrandPrix 
        { 
            MeetingKey = 1234, 
            SeasonYear = 2024, 
            RoundNumber = 21, 
            CountryName = "Brazil", 
            CircuitShortName = "Interlagos" 
        };
        
        // Criamos a Sessão com a chave 9158 (a mesma que o seu Worker usa nos logs!)
        var session = new Session 
        { 
            SessionKey = 9158, 
            MeetingKey = 1234, 
            SessionName = "Practice 1", 
            DateStart = DateTime.UtcNow 
        };

        // Adicionamos também o piloto 44 (Hamilton) para manter a integridade
        var driver = new Driver { DriverNumber = 44, FullName = "Lewis Hamilton", TeamName = "Mercedes" };

        _context.Seasons.Add(season);
        _context.GrandPrixes.Add(gp);
        _context.Sessions.Add(session);
        _context.Drivers.Add(driver);
        
        await _context.SaveChangesAsync();

        return Ok("Ambiente de QA populado! Temporada 2024, GP Brasil, Sessão 9158 e Piloto 44 criados.");
    }
}
