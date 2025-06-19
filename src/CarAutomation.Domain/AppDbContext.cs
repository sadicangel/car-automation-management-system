using CarAutomation.Domain.Auctions;
using CarAutomation.Domain.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace CarAutomation.Domain;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Vehicle> Vehicles { get; set; }

    public DbSet<Auction> Auctions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>(t =>
        {
            t.HasDiscriminator<string>("VehicleType")
                .HasValue<Sedan>(nameof(Sedan))
                .HasValue<Hatchback>(nameof(Hatchback))
                .HasValue<Suv>(nameof(Suv))
                .HasValue<Truck>(nameof(Truck));

            t.HasIndex(x => x.Vin).IsUnique();
            t.HasIndex(x => x.Manufacturer);
            t.HasIndex(x => x.Model);
            t.HasIndex(x => x.Year);
        });

        modelBuilder.Entity<Sedan>()
            .Property(x => x.NumberOfDoors)
            .HasColumnName(nameof(Sedan.NumberOfDoors));

        modelBuilder.Entity<Hatchback>()
            .Property(x => x.NumberOfDoors)
            .HasColumnName(nameof(Hatchback.NumberOfDoors));

        modelBuilder.Entity<Auction>(t =>
        {
            t.HasKey(x => x.AuctionId);
            t.HasIndex(x => x.VehicleId);
            t.HasOne(x => x.Vehicle).WithMany().HasForeignKey(x => x.VehicleId);
            t.OwnsMany(x => x.Bids, builder => builder.ToJson());
        });
    }
}
