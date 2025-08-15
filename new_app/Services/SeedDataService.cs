using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using new_app.Data;
using new_app.Models;

namespace new_app.Services;

/// <summary>
/// Service class to seed demonstration data into the database.
/// This complements the SeedData class with additional demo data functionality.
/// </summary>
public class SeedDataService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SeedDataService> _logger;
    
    public SeedDataService(ApplicationDbContext context, ILogger<SeedDataService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Seeds demonstration data for hotels, customers, and orders.
    /// </summary>
    public async Task SeedDemoDataAsync()
    {
        // Only seed demo data if we don't have any hotels yet
        if (!await _context.Hotels.AnyAsync().ConfigureAwait(false))
        {
            _logger.LogInformation("Seeding demonstration hotels...");
            
            // Get all country IDs for reference
            var countries = await _context.Countries.ToDictionaryAsync(
                c => c.Name,
                c => c.Id
            ).ConfigureAwait(false);
            
            // Create demo hotels
            var hotels = new List<Hotel>
            {
                new Hotel
                {
                    Name = "Grand Resort & Spa",
                    CountryId = GetCountryId(countries, "Spain"),
                    Address = "123 Playa del Sol, Barcelona",
                    PostCode = "08001",
                    PricePerNight = 199.99m
                },
                new Hotel
                {
                    Name = "Mountain View Lodge",
                    CountryId = GetCountryId(countries, "France"),
                    Address = "45 Rue de la Montagne, Chamonix",
                    PostCode = "74400",
                    PricePerNight = 149.50m
                },
                new Hotel
                {
                    Name = "Seaside Retreat",
                    CountryId = GetCountryId(countries, "Greece"),
                    Address = "78 Harbor Road, Santorini",
                    PostCode = "84700",
                    PricePerNight = 225m
                },
                new Hotel
                {
                    Name = "City Central Hotel",
                    CountryId = GetCountryId(countries, "Germany"),
                    Address = "10 Hauptstraße, Berlin",
                    PostCode = "10115",
                    PricePerNight = 135m
                },
                new Hotel
                {
                    Name = "Desert Oasis Resort",
                    CountryId = GetCountryId(countries, "Egypt"),
                    Address = "120 Pyramid Road, Giza",
                    PostCode = "12556",
                    PricePerNight = 175.25m
                }
            };
            
            _logger.LogInformation("Adding {Count} demo hotels", hotels.Count);
            await _context.Hotels.AddRangeAsync(hotels).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            
            // Create demo customers
            if (!await _context.Customers.AnyAsync().ConfigureAwait(false))
            {
                _logger.LogInformation("Seeding demonstration customers...");
                
                var customers = new List<Customer>
                {
                    new Customer
                    {
                        Name = "John Smith",
                        Birthdate = new DateTime(1985, 5, 15)
                    },
                    new Customer
                    {
                        Name = "Emma Johnson",
                        Birthdate = new DateTime(1990, 8, 22)
                    },
                    new Customer
                    {
                        Name = "Michael Brown",
                        Birthdate = new DateTime(1978, 3, 10)
                    },
                    new Customer
                    {
                        Name = "Sophia Williams",
                        Birthdate = new DateTime(1995, 11, 7)
                    }
                };
                
                _logger.LogInformation("Adding {Count} demo customers", customers.Count);
                await _context.Customers.AddRangeAsync(customers).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                
                // Create some demo orders
                _logger.LogInformation("Seeding demonstration orders...");
                
                // Get all hotels and customers
                var allHotels = await _context.Hotels.ToListAsync().ConfigureAwait(false);
                var allCustomers = await _context.Customers.ToListAsync().ConfigureAwait(false);
                
                // Create a few orders
                var orders = new List<Order>
                {
                    new Order
                    {
                        Customer = allCustomers[0],
                        Hotel = allHotels[0],
                        DateOrdered = DateTime.Now.AddDays(-10),
                        StartDate = DateTime.Now.AddDays(20),
                        EndDate = DateTime.Now.AddDays(25),
                        NumberOfDays = 5,
                        FullPrice = allHotels[0].PricePerNight * 5
                    },
                    new Order
                    {
                        Customer = allCustomers[1],
                        Hotel = allHotels[2],
                        DateOrdered = DateTime.Now.AddDays(-5),
                        StartDate = DateTime.Now.AddDays(30),
                        EndDate = DateTime.Now.AddDays(37),
                        NumberOfDays = 7,
                        FullPrice = allHotels[2].PricePerNight * 7
                    },
                    new Order
                    {
                        Customer = allCustomers[2],
                        Hotel = allHotels[1],
                        DateOrdered = DateTime.Now.AddDays(-15),
                        StartDate = DateTime.Now.AddDays(5),
                        EndDate = DateTime.Now.AddDays(12),
                        NumberOfDays = 7,
                        FullPrice = allHotels[1].PricePerNight * 7
                    }
                };
                
                _logger.LogInformation("Adding {Count} demo orders", orders.Count);
                await _context.Orders.AddRangeAsync(orders).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            
            _logger.LogInformation("Demonstration data seeding completed");
        }
        else
        {
            _logger.LogInformation("Hotels already exist - skipping demo data seeding");
        }
    }
    
    /// <summary>
    /// Helper method to get a country ID safely.
    /// </summary>
    private int GetCountryId(Dictionary<string, int> countries, string countryName)
    {
        if (countries.TryGetValue(countryName, out int countryId))
        {
            return countryId;
        }
        
        // Fallback to the first country if the specified one doesn't exist
        return countries.Values.FirstOrDefault();
    }
}