using Microsoft.EntityFrameworkCore;
using CarHistory.Api.Entities;

namespace CarHistory.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<CarEntry> CarEntries => Set<CarEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<CarEntry>();

        e.ToTable("car_entries");
        e.HasKey(x => x.Id);

        e.Property(x => x.Make).HasMaxLength(50).IsRequired();
        e.Property(x => x.Model).HasMaxLength(50).IsRequired();
        e.Property(x => x.Trim).HasMaxLength(50);
        e.Property(x => x.Color).HasMaxLength(50);
        e.Property(x => x.Vin).HasMaxLength(17);

        e.Property(x => x.Notes).HasMaxLength(2000);

        e.Property(x => x.Year).IsRequired();
        e.Property(x => x.PurchaseDate).IsRequired();
        e.Property(x => x.OdometerAtPurchase).IsRequired();

        // Simple helpful index (optional)
        e.HasIndex(x => new { x.Make, x.Model, x.Year });
    }
}
