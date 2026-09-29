namespace F1Telemetry.Domain.Entities;

public class LapTime
{
    public Guid Id { get; set; } // PK
    public int SessionKey { get; set; } // FK
    public int DriverNumber { get; set; } // FK
    public int LapNumber { get; set; }
    public double LapDurationMs { get; set; }
    
    public string? TireCompound { get; set; }
    public int? TireAge { get; set; }
    
    // Propriedades de Navegação
    public Session Session { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
}
