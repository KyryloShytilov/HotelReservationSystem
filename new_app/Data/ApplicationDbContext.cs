using HotelReservationSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationSystem.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Hotel> Hotels { get; set; } = null!;
    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure entity relationships
        builder.Entity<Hotel>()
            .HasOne(h => h.Country)
            .WithMany()
            .HasForeignKey(h => h.CountryId);
            
        builder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany();

        builder.Entity<Order>()
            .HasOne(o => o.Hotel)
            .WithMany();
    }
}