using F1Telemetry.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace F1Telemetry.Infrastructure.Data;

public class F1TelemetryDbContext : DbContext
{
    public F1TelemetryDbContext(DbContextOptions<F1TelemetryDbContext> options) : base(options) { }

    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<LapTime> LapTimes { get; set; }
    public DbSet<PitStop> PitStops { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define as Chaves Primárias explicitamente
        modelBuilder.Entity<Driver>().HasKey(d => d.DriverNumber);
        modelBuilder.Entity<Session>().HasKey(s => s.SessionKey);
        
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

        base.OnModelCreating(modelBuilder);
    }
}
