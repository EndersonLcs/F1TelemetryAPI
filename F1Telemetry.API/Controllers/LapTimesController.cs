using System.Text.Json;
using F1Telemetry.Domain.Entities;
using F1Telemetry.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace F1Telemetry.API.Controllers;

[ApiController]
[Route("api/v1/sessions/{sessionKey}/laptimes")]
public class LapTimesController : ControllerBase
{
    private readonly ILapTimeRepository _repository;
    private readonly IDistributedCache _cache;

    public LapTimesController(ILapTimeRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> GetLapTimes(int sessionKey)
    {
        string cacheKey = $"laptimes_session_{sessionKey}";
        
        // 1. Tenta buscar do Redis
        var cachedData = await _cache.GetStringAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            // Cache HIT: Retorna direto da memória
            var lapsFromCache = JsonSerializer.Deserialize<IEnumerable<LapTime>>(cachedData);
            return Ok(lapsFromCache);
        }

        // 2. Cache MISS: Busca no banco de dados
        var laps = await _repository.GetLapTimesBySessionAsync(sessionKey);
        
        if (!laps.Any()) return NotFound();

        // 3. Salva no Redis com expiração (ex: 5 minutos)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
        
        await _cache.SetStringAsync(
            cacheKey, 
            JsonSerializer.Serialize(laps), 
            cacheOptions);

        return Ok(laps);
    }

    [HttpGet("fastest")]
    public async Task<IActionResult> GetFastestLap(int sessionKey)
    {
        // Aqui posso aplicar o cache (Passo fufturo)
        var fastestLap = await _repository.GetFastestLapBySessionAsync(sessionKey);
        
        if (fastestLap == null) 
            return NotFound(new { message = "Nenhuma volta encontrada para esta sessão." });
            
        return Ok(fastestLap);
    }
}
