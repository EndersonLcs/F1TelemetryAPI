namespace F1Telemetry.Domain.Entities;

public class Session
{
    public int SessionKey { get; set; } // PK (ID da OpenF1)
    public int MeetingKey { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public DateTime DateStart { get; set; }

    public ICollection<LapTime> LapTimes { get; set; } = new List<LapTime>();
    public ICollection<PitStop> PitStops { get; set; } = new List<PitStop>();
}
