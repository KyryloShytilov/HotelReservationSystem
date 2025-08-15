using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using new_app.Data;
using new_app.Models;

namespace new_app;

/// <summary>
/// Provides methods for seeding initial data in the application.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Asynchronous method for database initialization - called from Program.cs.
    /// </summary>
    /// <param name="context">The application database context</param>
    /// <param name="userManager">The ASP.NET Core Identity user manager</param>
    /// <param name="roleManager">The ASP.NET Core Identity role manager</param>
    public static async Task InitializeAsync(
        ApplicationDbContext context, 
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Ensure the database is created
        await context.Database.EnsureCreatedAsync().ConfigureAwait(false);
        
        // Seed roles
        await SeedRolesAsync(roleManager).ConfigureAwait(false);
        
        // Seed admin user
        await SeedAdminUserAsync(userManager).ConfigureAwait(false);
        
        // Seed countries
        await SeedCountriesAsync(context).ConfigureAwait(false);

        // Seed demo data
        await SeedDemoDataAsync(context).ConfigureAwait(false);
    }

    /// <summary>
    /// Initializes the database with seed data.
    /// </summary>
    /// <param name="context">The application database context</param>
    /// <param name="userManager">The ASP.NET Core Identity user manager</param>
    /// <param name="roleManager">The ASP.NET Core Identity role manager</param>
    public static async Task Initialize(
        ApplicationDbContext context, 
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        await InitializeAsync(context, userManager, roleManager).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Seeds the application roles into the database.
    /// </summary>
    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        // Create roles if they don't exist
        if (!await roleManager.RoleExistsAsync(RoleName.Admin).ConfigureAwait(false))
        {
            await roleManager.CreateAsync(new IdentityRole(RoleName.Admin)).ConfigureAwait(false);
        }
        
        if (!await roleManager.RoleExistsAsync(RoleName.HotelManager).ConfigureAwait(false))
        {
            await roleManager.CreateAsync(new IdentityRole(RoleName.HotelManager)).ConfigureAwait(false);
        }
    }
    
    /// <summary>
    /// Seeds administrator users into the database.
    /// </summary>
    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        // Create admin users if they don't exist
        var adminEmails = new[] 
        { 
            new { Email = "admin@admin.com", IsAdmin = true, Phone = "123-456-7890" },
            new { Email = "admin@book.go", IsAdmin = true, Phone = "123-555-7890" },
            new { Email = "guest@book.go", IsAdmin = false, Phone = "555-123-4567" }
        };
        
        var defaultPassword = "Admin123!";
        
        foreach (var admin in adminEmails)
        {
            var existingUser = await userManager.FindByEmailAsync(admin.Email).ConfigureAwait(false);
            
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = admin.Email,
                    Email = admin.Email,
                    EmailConfirmed = true,
                    Phone = admin.Phone
                };
                
                var result = await userManager.CreateAsync(user, defaultPassword).ConfigureAwait(false);
                
                if (result.Succeeded)
                {
                    // Add roles based on user type
                    if (admin.IsAdmin)
                    {
                        await userManager.AddToRoleAsync(user, RoleName.Admin).ConfigureAwait(false);
                        await userManager.AddToRoleAsync(user, RoleName.HotelManager).ConfigureAwait(false);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Seeds country data into the database.
    /// </summary>
    private static async Task SeedCountriesAsync(ApplicationDbContext context)
    {
        // Seed countries if they don't exist
        if (!await context.Countries.AnyAsync().ConfigureAwait(false))
        {
            var countries = new List<Country>
            {
                new Country { Name = "Egypt" },
                new Country { Name = "Poland" },
                new Country { Name = "Germany" },
                new Country { Name = "Spain" },
                new Country { Name = "Greece" },
                new Country { Name = "Turkey" },
                new Country { Name = "Malta" },
                new Country { Name = "France" },
                new Country { Name = "Portugal" }, // Corrected from Portual
                new Country { Name = "England" },
                new Country { Name = "United States" },
                new Country { Name = "United Kingdom" },
                new Country { Name = "Italy" },
                new Country { Name = "Australia" },
                new Country { Name = "Canada" },
                new Country { Name = "Japan" },
                new Country { Name = "Mexico" },
                new Country { Name = "Brazil" },
                new Country { Name = "Argentina" },
                new Country { Name = "China" },
                new Country { Name = "India" }
            };
            
            await context.Countries.AddRangeAsync(countries).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Seeds demonstration data into the database.
    /// </summary>
    private static async Task SeedDemoDataAsync(ApplicationDbContext context)
    {
        // Only seed demo data if we don't have any hotels yet
        if (!await context.Hotels.AnyAsync().ConfigureAwait(false))
        {
            // Get all country IDs for reference
            var countries = await context.Countries.ToDictionaryAsync(
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
            
            await context.Hotels.AddRangeAsync(hotels).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
            
            // Create demo customers
            if (!await context.Customers.AnyAsync().ConfigureAwait(false))
            {
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
                
                await context.Customers.AddRangeAsync(customers).ConfigureAwait(false);
                await context.SaveChangesAsync().ConfigureAwait(false);
                
                // Create some demo orders
                // Get all hotels and customers
                var allHotels = await context.Hotels.ToListAsync().ConfigureAwait(false);
                var allCustomers = await context.Customers.ToListAsync().ConfigureAwait(false);
                
                if (allHotels.Count >= 3 && allCustomers.Count >= 3)
                {
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
                    
                    await context.Orders.AddRangeAsync(orders).ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }
    }
    
    /// <summary>
    /// Helper method to get a country ID safely.
    /// </summary>
    private static int GetCountryId(Dictionary<string, int> countries, string countryName)
    {
        if (countries.TryGetValue(countryName, out int countryId))
        {
            return countryId;
        }
        
        // Fallback to the first country if the specified one doesn't exist
        return countries.Values.FirstOrDefault();
    }
}