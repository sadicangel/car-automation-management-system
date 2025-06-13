using Microsoft.EntityFrameworkCore;

namespace WebApi;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Vehicle> Vehicles { get; set; }

    public DbSet<Auction> Auctions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>(t =>
        {
            t.HasIndex(x => x.Vin).IsUnique();
            t.HasIndex(x => x.Manufacturer);
            t.HasIndex(x => x.Model);
            t.HasIndex(x => x.Year);
        });

        modelBuilder.Entity<Auction>(t =>
        {
            t.HasKey(x => x.AuctionId);
            t.HasIndex(x => x.VehicleId);
            t.HasOne(x => x.Vehicle).WithMany().HasForeignKey(x => x.VehicleId);
            t.OwnsMany(x => x.Bids, builder => builder.ToJson());
        });
    }
}
