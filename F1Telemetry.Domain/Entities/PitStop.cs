namespace F1Telemetry.Domain.Entities;

public class PitStop
{
    public Guid Id { get; set; } // PK
    public int SessionKey { get; set; } // FK
    public int DriverNumber { get; set; } // FK
    public int LapNumber { get; set; }
    public double PitDurationMs { get; set; }

    // Propriedades de Navegação
    public Session Session { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
}
