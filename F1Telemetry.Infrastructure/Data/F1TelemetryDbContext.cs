using F1Telemetry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace F1Telemetry.Infrastructure.Data;

public class F1TelemetryDbContext : DbContext
{
    public F1TelemetryDbContext(DbContextOptions<F1TelemetryDbContext> options) : base(options) { }

    // --- DbSets (Tabelas do Banco) ---
    public DbSet<Season> Seasons { get; set; }
    public DbSet<GrandPrix> GrandPrixes { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<LapTime> LapTimes { get; set; }
    public DbSet<PitStop> PitStops { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Season>()
            .HasKey(s => s.Year);

        modelBuilder.Entity<GrandPrix>()
            .HasKey(g => g.MeetingKey);

        modelBuilder.Entity<GrandPrix>()
            .HasOne(g => g.Season)
            .WithMany(s => s.GrandPrixes)
            .HasForeignKey(g => g.SeasonYear)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Session>()
            .HasKey(s => s.SessionKey);

        modelBuilder.Entity<Session>()
            .HasOne(s => s.GrandPrix)
            .WithMany(g => g.Sessions)
            .HasForeignKey(s => s.MeetingKey)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Driver>()
            .HasKey(d => d.DriverNumber);

        // Relacionamentos para LapTime
        modelBuilder.Entity<LapTime>()
            .HasOne(l => l.Driver)
            .WithMany(d => d.LapTimes)
            .HasForeignKey(l => l.DriverNumber);

        modelBuilder.Entity<LapTime>()
            .HasOne(l => l.Session)
            .WithMany(s => s.LapTimes)
            .HasForeignKey(l => l.SessionKey);

        // Relacionamentos para PitStop
        modelBuilder.Entity<PitStop>()
            .HasOne(p => p.Driver)
            .WithMany(d => d.PitStops)
            .HasForeignKey(p => p.DriverNumber);

        modelBuilder.Entity<PitStop>()
            .HasOne(p => p.Session)
            .WithMany(s => s.PitStops)
            .HasForeignKey(p => p.SessionKey);
    }
}
