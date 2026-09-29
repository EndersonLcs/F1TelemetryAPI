namespace F1Telemetry.Domain.Entities;

public class Driver
{
    public int DriverNumber { get; set; } // PK
    public string FullName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;

    // Propriedades de Navegação
    public ICollection<LapTime> LapTimes { get; set; } = new List<LapTime>();
    public ICollection<PitStop> PitStops { get; set; } = new List<PitStop>();
}
