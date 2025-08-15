using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HotelReservationSystem.Models;

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
        
        // Configure your model relationships here
        builder.Entity<Hotel>()
            .HasOne(h => h.Country)
            .WithMany(c => c.Hotels)
            .HasForeignKey(h => h.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .IsRequired();
            
        builder.Entity<Order>()
            .HasOne(o => o.Hotel)
            .WithMany(h => h.Orders)
            .HasForeignKey(o => o.HotelId)
            .IsRequired();
    }
}