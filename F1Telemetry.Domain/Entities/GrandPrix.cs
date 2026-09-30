namespace F1Telemetry.Domain.Entities;

public class GrandPrix
{
    public int MeetingKey { get; set; } // PK
    public int SeasonYear { get; set; } // FK para Season
    
    public int RoundNumber { get; set; } // Etapa (Ex: 21 para o GP de São Paulo)
    public string CountryName { get; set; } = string.Empty;
    public string CircuitShortName { get; set; } = string.Empty;
    
    // Propriedades de Navegação
    public Season Season { get; set; } = null!;
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}
