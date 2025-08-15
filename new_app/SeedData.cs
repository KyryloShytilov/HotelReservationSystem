using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using new_app.Data;
using new_app.Models;

namespace new_app;

/// <summary>
/// Provides methods for seeding initial data in the application database.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Asynchronously initializes the database with seed data.
    /// Method signature matches what is being called in Program.cs.
    /// </summary>
    /// <param name="context">The application database context</param>
    /// <param name="userManager">The ASP.NET Core Identity user manager</param>
    /// <param name="roleManager">The ASP.NET Core Identity role manager</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task InitializeAsync(
        ApplicationDbContext context, 
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Ensure the database is created
        await context.Database.MigrateAsync().ConfigureAwait(false);
        
        // Seed data in order of dependencies
        await SeedRolesAsync(roleManager).ConfigureAwait(false);
        await SeedUsersAsync(userManager).ConfigureAwait(false);
        await SeedCountriesAsync(context).ConfigureAwait(false);
        await SeedDemoDataAsync(context).ConfigureAwait(false);
    }

    /// <summary>
    /// Alternative initialization method with logger support.
    /// </summary>
    /// <param name="context">The application database context</param>
    /// <param name="userManager">The ASP.NET Core Identity user manager</param>
    /// <param name="roleManager">The ASP.NET Core Identity role manager</param>
    /// <param name="logger">Logger for recording initialization events</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task Initialize(
        ApplicationDbContext context, 
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger? logger = null)
    {
        try
        {
            // Apply any pending migrations
            logger?.LogInformation("Applying migrations...");
            await context.Database.MigrateAsync().ConfigureAwait(false);
            logger?.LogInformation("Migrations applied successfully.");
            
            // Seed data in order of dependencies
            await SeedRolesAsync(roleManager, logger).ConfigureAwait(false);
            await SeedUsersAsync(userManager, logger).ConfigureAwait(false);
            await SeedCountriesAsync(context, logger).ConfigureAwait(false);
            await SeedDemoDataAsync(context, logger).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "An error occurred while seeding the database");
            throw; // Re-throw to allow caller to handle
        }
    }
    
    /// <summary>
    /// Seeds the application roles into the database.
    /// </summary>
    private static async Task SeedRolesAsync(
        RoleManager<IdentityRole> roleManager,
        ILogger? logger = null)
    {
        logger?.LogInformation("Seeding application roles...");
        
        // Create roles if they don't exist
        if (!await roleManager.RoleExistsAsync(RoleName.Admin).ConfigureAwait(false))
        {
            logger?.LogInformation("Creating Admin role");
            await roleManager.CreateAsync(new IdentityRole(RoleName.Admin)).ConfigureAwait(false);
        }
        
        if (!await roleManager.RoleExistsAsync(RoleName.HotelManager).ConfigureAwait(false))
        {
            logger?.LogInformation("Creating HotelManager role");
            await roleManager.CreateAsync(new IdentityRole(RoleName.HotelManager)).ConfigureAwait(false);
        }
        
        logger?.LogInformation("Role seeding completed");
    }

    /// <summary>
    /// Seeds the application roles into the database (without logger).
    /// </summary>
    private static Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        return SeedRolesAsync(roleManager, null);
    }
    
    /// <summary>
    /// Seeds users into the database with appropriate roles.
    /// </summary>
    private static async Task SeedUsersAsync(
        UserManager<ApplicationUser> userManager,
        ILogger? logger = null)
    {
        logger?.LogInformation("Seeding application users...");
        
        // Admin users
        var adminUsers = new[] 
        {
            new { Email = "admin@admin.com", Phone = "123-456-7890", Roles = new[] { RoleName.Admin, RoleName.HotelManager } },
            new { Email = "admin@book.go", Phone = "123-555-7890", Roles = new[] { RoleName.Admin, RoleName.HotelManager } }
        };
        
        // Hotel manager users
        var managerUsers = new[]
        {
            new { Email = "manager@hotel.com", Phone = "987-654-3210", Roles = new[] { RoleName.HotelManager } }
        };
        
        // Regular users
        var regularUsers = new[]
        {
            new { Email = "guest@book.go", Phone = "555-123-4567", Roles = Array.Empty<string>() },
            new { Email = "user@example.com", Phone = "555-987-6543", Roles = Array.Empty<string>() }
        };
        
        // Default password for development environment
        var defaultPassword = "Admin123!";
        
        // Create admin users
        foreach (var adminUser in adminUsers)
        {
            await CreateUserWithRolesAsync(
                userManager, 
                adminUser.Email, 
                adminUser.Phone, 
                defaultPassword,
                adminUser.Roles,
                logger
            ).ConfigureAwait(false);
        }
        
        // Create manager users
        foreach (var managerUser in managerUsers)
        {
            await CreateUserWithRolesAsync(
                userManager, 
                managerUser.Email, 
                managerUser.Phone, 
                defaultPassword,
                managerUser.Roles,
                logger
            ).ConfigureAwait(false);
        }
        
        // Create regular users
        foreach (var regularUser in regularUsers)
        {
            await CreateUserWithRolesAsync(
                userManager, 
                regularUser.Email, 
                regularUser.Phone, 
                defaultPassword,
                regularUser.Roles,
                logger
            ).ConfigureAwait(false);
        }
        
        logger?.LogInformation("User seeding completed");
    }

    /// <summary>
    /// Seeds users into the database with appropriate roles (without logger).
    /// </summary>
    private static Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        return SeedUsersAsync(userManager, null);
    }
    
    /// <summary>
    /// Helper method to create a user with specified roles.
    /// </summary>
    private static async Task CreateUserWithRolesAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string phone,
        string password,
        string[] roles,
        ILogger? logger = null)
    {
        var existingUser = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
        
        if (existingUser == null)
        {
            logger?.LogInformation("Creating user: {Email}", email);
            
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Phone = phone
            };
            
            var result = await userManager.CreateAsync(user, password).ConfigureAwait(false);
            
            if (result.Succeeded)
            {
                // Assign roles
                foreach (var role in roles)
                {
                    logger?.LogInformation("Assigning role {Role} to user {Email}", role, email);
                    await userManager.AddToRoleAsync(user, role).ConfigureAwait(false);
                }
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger?.LogWarning("Failed to create user {Email}. Errors: {Errors}", email, errors);
            }
        }
        else
        {
            // Update roles for existing user if needed
            var userRoles = await userManager.GetRolesAsync(existingUser).ConfigureAwait(false);
            
            foreach (var role in roles.Except(userRoles))
            {
                logger?.LogInformation("Adding missing role {Role} to existing user {Email}", role, email);
                await userManager.AddToRoleAsync(existingUser, role).ConfigureAwait(false);
            }
        }
    }
    
    /// <summary>
    /// Seeds country data into the database.
    /// </summary>
    private static async Task SeedCountriesAsync(
        ApplicationDbContext context,
        ILogger? logger = null)
    {
        logger?.LogInformation("Seeding countries...");
        
        // Seed countries if they don't exist
        if (!await context.Countries.AnyAsync().ConfigureAwait(false))
        {
            // Original countries from the legacy application migrations
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
                new Country { Name = "Portugal" }, // Fixed typo from original "Portual"
                new Country { Name = "England" },
                // Additional countries for more variety
                new Country { Name = "United States" },
                new Country { Name = "United Kingdom" },
                new Country { Name = "Italy" },
                new Country { Name = "Australia" },
                new Country { Name = "Canada" },
                new Country { Name = "Japan" },
                new Country { Name = "Mexico" },
                new Country { Name = "Brazil" },
                new Country { Name = "China" },
                new Country { Name = "India" }
            };
            
            logger?.LogInformation("Adding {Count} countries to the database", countries.Count);
            await context.Countries.AddRangeAsync(countries).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
            logger?.LogInformation("Countries added successfully");
        }
        else
        {
            logger?.LogInformation("Countries already exist - skipping seeding");
        }
    }

    /// <summary>
    /// Seeds country data into the database (without logger).
    /// </summary>
    private static Task SeedCountriesAsync(ApplicationDbContext context)
    {
        return SeedCountriesAsync(context, null);
    }
    
    /// <summary>
    /// Seeds demonstration data for hotels, customers, and orders.
    /// </summary>
    private static async Task SeedDemoDataAsync(
        ApplicationDbContext context,
        ILogger? logger = null)
    {
        // Only seed demo data if we don't have any hotels yet
        if (!await context.Hotels.AnyAsync().ConfigureAwait(false))
        {
            logger?.LogInformation("Seeding demonstration hotels...");
            
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
            
            logger?.LogInformation("Adding {Count} demo hotels", hotels.Count);
            await context.Hotels.AddRangeAsync(hotels).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
            
            // Create demo customers
            if (!await context.Customers.AnyAsync().ConfigureAwait(false))
            {
                logger?.LogInformation("Seeding demonstration customers...");
                
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
                
                logger?.LogInformation("Adding {Count} demo customers", customers.Count);
                await context.Customers.AddRangeAsync(customers).ConfigureAwait(false);
                await context.SaveChangesAsync().ConfigureAwait(false);
                
                // Create some demo orders
                logger?.LogInformation("Seeding demonstration orders...");
                
                // Get all hotels and customers
                var allHotels = await context.Hotels.ToListAsync().ConfigureAwait(false);
                var allCustomers = await context.Customers.ToListAsync().ConfigureAwait(false);
                
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
                
                logger?.LogInformation("Adding {Count} demo orders", orders.Count);
                await context.Orders.AddRangeAsync(orders).ConfigureAwait(false);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            
            logger?.LogInformation("Demonstration data seeding completed");
        }
        else
        {
            logger?.LogInformation("Hotels already exist - skipping demo data seeding");
        }
    }

    /// <summary>
    /// Seeds demonstration data for hotels, customers, and orders (without logger).
    /// </summary>
    private static Task SeedDemoDataAsync(ApplicationDbContext context)
    {
        return SeedDemoDataAsync(context, null);
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