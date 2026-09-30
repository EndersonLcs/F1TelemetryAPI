namespace F1Telemetry.Domain.Entities;

public class Session
{
    public int SessionKey { get; set; } // PK
    public int MeetingKey { get; set; } // FK para GrandPrix
    
    public string SessionName { get; set; } = string.Empty; // Ex: "Practice 1", "Qualifying", "Race"
    public DateTime DateStart { get; set; }
    
    // Propriedades de Navegação
    public GrandPrix GrandPrix { get; set; } = null!; // Relacionamento Acima (N:1)
    public ICollection<LapTime> LapTimes { get; set; } = new List<LapTime>(); // Relacionamento Abaixo (1:N)
    public ICollection<PitStop> PitStops { get; set; } = new List<PitStop>();
}
