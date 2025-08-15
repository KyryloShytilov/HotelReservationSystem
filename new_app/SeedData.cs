using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using new_app.Data;
using new_app.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_app;

/// <summary>
/// Provides methods for seeding initial data in the application.
/// Includes functionality for creating roles, users, countries, hotels, and customer data.
/// </summary>
public static class SeedData
{
    private static readonly ILogger<SeedData> _logger;

    /// <summary>
    /// Initializes the database with seed data.
    /// 
    /// This method should be called during application startup in Program.cs as follows:
    /// 
    /// Example usage in Program.cs:
    /// <code>
    /// // Add this after the app build
    /// using (var scope = app.Services.CreateScope())
    /// {
    ///     var services = scope.ServiceProvider;
    ///     try
    ///     {
    ///         var context = services.GetRequiredService&lt;ApplicationDbContext&gt;();
    ///         var userManager = services.GetRequiredService&lt;UserManager&lt;ApplicationUser&gt;&gt;();
    ///         var roleManager = services.GetRequiredService&lt;RoleManager&lt;IdentityRole&gt;&gt;();
    ///         var logger = services.GetRequiredService&lt;ILogger&lt;Program&gt;&gt;();
    ///         
    ///         await SeedData.Initialize(context, userManager, roleManager, logger);
    ///     }
    ///     catch (Exception ex)
    ///     {
    ///         var logger = services.GetRequiredService&lt;ILogger&lt;Program&gt;&gt;();
    ///         logger.LogError(ex, "An error occurred while seeding the database.");
    ///     }
    /// }
    /// </code>
    /// </summary>
    /// <param name="context">The application database context</param>
    /// <param name="userManager">The ASP.NET Core Identity user manager</param>
    /// <param name="roleManager">The ASP.NET Core Identity role manager</param>
    /// <param name="logger">The logger instance for recording operations</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task Initialize(
        ApplicationDbContext context, 
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger logger = null)
    {
        try
        {
            // Apply any pending migrations
            await context.Database.MigrateAsync().ConfigureAwait(false);
            
            logger?.LogInformation("Starting database seeding process...");
            
            // Seed roles
            await SeedRoles(roleManager).ConfigureAwait(false);
            logger?.LogInformation("Roles seeded successfully");
            
            // Seed users
            await SeedUsers(userManager).ConfigureAwait(false);
            logger?.LogInformation("Users seeded successfully");
            
            // Seed countries
            await SeedCountries(context).ConfigureAwait(false);
            logger?.LogInformation("Countries seeded successfully");
            
            // Seed hotels
            await SeedHotels(context).ConfigureAwait(false);
            logger?.LogInformation("Hotels seeded successfully");
            
            // Seed customers
            await SeedCustomers(context).ConfigureAwait(false);
            logger?.LogInformation("Customers seeded successfully");
            
            logger?.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "An error occurred during database seeding");
            throw; // Rethrow to allow the calling code to handle the exception
        }
    }
    
    /// <summary>
    /// Seeds the application roles into the database.
    /// Creates Admin and HotelManager roles if they don't exist.
    /// </summary>
    /// <param name="roleManager">The ASP.NET Core Identity role manager</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        // Create roles if they don't exist
        if (!await roleManager.RoleExistsAsync(RoleName.Admin).ConfigureAwait(false))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(RoleName.Admin)).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create {RoleName.Admin} role: {string.Join(", ", result.Errors)}");
            }
        }
        
        if (!await roleManager.RoleExistsAsync(RoleName.HotelManager).ConfigureAwait(false))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(RoleName.HotelManager)).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create {RoleName.HotelManager} role: {string.Join(", ", result.Errors)}");
            }
        }
    }
    
    /// <summary>
    /// Seeds administrator and hotel manager users into the database.
    /// </summary>
    /// <param name="userManager">The ASP.NET Core Identity user manager</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private static async Task SeedUsers(UserManager<ApplicationUser> userManager)
    {
        // Create admin and hotel manager users if they don't exist
        var userDataList = new List<(string Email, string Role, string Phone)>
        {
            ("admin@admin.com", RoleName.Admin, "123-456-7890"),
            ("admin@book.go", RoleName.Admin, "123-456-7891"),
            ("guest@book.go", null, "123-456-7892"),
            ("manager@hotel1.com", RoleName.HotelManager, "123-456-7893"),
            ("manager@hotel2.com", RoleName.HotelManager, "123-456-7894")
        };
        
        var defaultPassword = "Admin123!";
        
        foreach (var userData in userDataList)
        {
            var existingUser = await userManager.FindByEmailAsync(userData.Email).ConfigureAwait(false);
            
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = userData.Email,
                    Email = userData.Email,
                    EmailConfirmed = true,
                    Phone = userData.Phone,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false
                };
                
                var result = await userManager.CreateAsync(user, defaultPassword).ConfigureAwait(false);
                
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create user {userData.Email}: {string.Join(", ", result.Errors)}");
                }
                
                // Assign role if specified
                if (!string.IsNullOrEmpty(userData.Role))
                {
                    var roleResult = await userManager.AddToRoleAsync(user, userData.Role).ConfigureAwait(false);
                    
                    if (!roleResult.Succeeded)
                    {
                        throw new InvalidOperationException($"Failed to assign role {userData.Role} to user {userData.Email}: {string.Join(", ", roleResult.Errors)}");
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Seeds country data into the database.
    /// Provides a comprehensive list of countries for the application.
    /// </summary>
    /// <param name="context">The application database context</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private static async Task SeedCountries(ApplicationDbContext context)
    {
        // Seed countries if they don't exist
        if (!await context.Countries.AnyAsync().ConfigureAwait(false))
        {
            var countries = new List<Country>
            {
                new Country { Name = "United States" },
                new Country { Name = "United Kingdom" },
                new Country { Name = "England" },
                new Country { Name = "France" },
                new Country { Name = "Spain" },
                new Country { Name = "Italy" },
                new Country { Name = "Germany" },
                new Country { Name = "Poland" },
                new Country { Name = "Greece" },
                new Country { Name = "Turkey" },
                new Country { Name = "Malta" },
                new Country { Name = "Egypt" },
                new Country { Name = "Japan" },
                new Country { Name = "Australia" },
                new Country { Name = "Canada" },
                new Country { Name = "Mexico" },
                new Country { Name = "Portugal" },
                new Country { Name = "Brazil" },
                new Country { Name = "Argentina" },
                new Country { Name = "China" },
                new Country { Name = "India" },
                // Additional countries
                new Country { Name = "South Africa" },
                new Country { Name = "New Zealand" },
                new Country { Name = "Russia" },
                new Country { Name = "Sweden" },
                new Country { Name = "Norway" },
                new Country { Name = "Finland" },
                new Country { Name = "Denmark" },
                new Country { Name = "Netherlands" },
                new Country { Name = "Belgium" },
                new Country { Name = "Austria" },
                new Country { Name = "Switzerland" },
                new Country { Name = "Ireland" },
                new Country { Name = "Thailand" },
                new Country { Name = "Singapore" },
                new Country { Name = "Indonesia" },
                new Country { Name = "Malaysia" },
                new Country { Name = "South Korea" },
                new Country { Name = "Vietnam" },
                new Country { Name = "United Arab Emirates" },
                new Country { Name = "Saudi Arabia" },
                new Country { Name = "Croatia" },
                new Country { Name = "Czech Republic" },
                new Country { Name = "Hungary" },
                new Country { Name = "Ukraine" }
            };
            
            await context.Countries.AddRangeAsync(countries).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Seeds hotel data into the database.
    /// Creates demonstration hotels for testing and development.
    /// </summary>
    /// <param name="context">The application database context</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private static async Task SeedHotels(ApplicationDbContext context)
    {
        if (!await context.Hotels.AnyAsync().ConfigureAwait(false))
        {
            // Get countries for reference
            var usa = await context.Countries.FirstOrDefaultAsync(c => c.Name == "United States").ConfigureAwait(false);
            var uk = await context.Countries.FirstOrDefaultAsync(c => c.Name == "United Kingdom").ConfigureAwait(false);
            var france = await context.Countries.FirstOrDefaultAsync(c => c.Name == "France").ConfigureAwait(false);
            var spain = await context.Countries.FirstOrDefaultAsync(c => c.Name == "Spain").ConfigureAwait(false);
            var italy = await context.Countries.FirstOrDefaultAsync(c => c.Name == "Italy").ConfigureAwait(false);
            
            // Create sample hotels
            var hotels = new List<Hotel>
            {
                new Hotel 
                { 
                    Name = "Grand Plaza Hotel", 
                    Address = "123 Main St", 
                    City = "New York", 
                    CountryId = usa?.Id ?? 1, 
                    StarRating = 5, 
                    Description = "Luxury hotel in the heart of Manhattan",
                    AmountOfRooms = 250,
                    Phone = "212-555-1234",
                    Email = "info@grandplaza.com"
                },
                new Hotel 
                { 
                    Name = "Seaside Resort", 
                    Address = "45 Ocean Drive", 
                    City = "Miami", 
                    CountryId = usa?.Id ?? 1, 
                    StarRating = 4, 
                    Description = "Beautiful beachfront property with stunning ocean views",
                    AmountOfRooms = 180,
                    Phone = "305-555-6789",
                    Email = "reservations@seasideresort.com"
                },
                new Hotel 
                { 
                    Name = "London Ritz", 
                    Address = "150 Piccadilly", 
                    City = "London", 
                    CountryId = uk?.Id ?? 2, 
                    StarRating = 5, 
                    Description = "Historic luxury hotel in central London",
                    AmountOfRooms = 136,
                    Phone = "+44-20-7493-8181",
                    Email = "enquire@londonritz.com"
                },
                new Hotel 
                { 
                    Name = "Parisian Elegance", 
                    Address = "15 Rue de Rivoli", 
                    City = "Paris", 
                    CountryId = france?.Id ?? 4, 
                    StarRating = 4, 
                    Description = "Charming hotel with views of the Eiffel Tower",
                    AmountOfRooms = 120,
                    Phone = "+33-1-4455-6677",
                    Email = "bonjour@pariselegance.fr"
                },
                new Hotel 
                { 
                    Name = "Barcelona Beachfront", 
                    Address = "78 La Rambla", 
                    City = "Barcelona", 
                    CountryId = spain?.Id ?? 5, 
                    StarRating = 4, 
                    Description = "Modern hotel near Barcelona's famous beaches",
                    AmountOfRooms = 200,
                    Phone = "+34-93-123-4567",
                    Email = "info@barcelonabeach.es"
                },
                new Hotel 
                { 
                    Name = "Roman Holiday Inn", 
                    Address = "42 Via Veneto", 
                    City = "Rome", 
                    CountryId = italy?.Id ?? 6, 
                    StarRating = 3, 
                    Description = "Cozy hotel within walking distance of major attractions",
                    AmountOfRooms = 90,
                    Phone = "+39-06-8765-4321",
                    Email = "ciao@romanholiday.it"
                }
            };
            
            await context.Hotels.AddRangeAsync(hotels).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Seeds customer data into the database.
    /// Creates demonstration customers for testing and development.
    /// </summary>
    /// <param name="context">The application database context</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    private static async Task SeedCustomers(ApplicationDbContext context)
    {
        if (!await context.Customers.AnyAsync().ConfigureAwait(false))
        {
            // Get countries for reference
            var usa = await context.Countries.FirstOrDefaultAsync(c => c.Name == "United States").ConfigureAwait(false);
            var uk = await context.Countries.FirstOrDefaultAsync(c => c.Name == "United Kingdom").ConfigureAwait(false);
            var germany = await context.Countries.FirstOrDefaultAsync(c => c.Name == "Germany").ConfigureAwait(false);
            var japan = await context.Countries.FirstOrDefaultAsync(c => c.Name == "Japan").ConfigureAwait(false);
            var canada = await context.Countries.FirstOrDefaultAsync(c => c.Name == "Canada").ConfigureAwait(false);
            
            // Create sample customers
            var customers = new List<Customer>
            {
                new Customer 
                { 
                    FirstName = "John", 
                    LastName = "Smith",
                    Email = "john.smith@example.com",
                    Phone = "212-555-9876",
                    CountryId = usa?.Id ?? 1,
                    Address = "123 Broadway, New York, NY 10001"
                },
                new Customer 
                { 
                    FirstName = "Emma", 
                    LastName = "Johnson",
                    Email = "emma.johnson@example.com",
                    Phone = "415-555-3456",
                    CountryId = usa?.Id ?? 1,
                    Address = "456 Market St, San Francisco, CA 94105"
                },
                new Customer 
                { 
                    FirstName = "James", 
                    LastName = "Williams",
                    Email = "james.williams@example.com",
                    Phone = "+44-20-5555-7890",
                    CountryId = uk?.Id ?? 2,
                    Address = "78 Baker Street, London, W1U 6AG"
                },
                new Customer 
                { 
                    FirstName = "Sophie", 
                    LastName = "Brown",
                    Email = "sophie.brown@example.com",
                    Phone = "+44-161-555-1234",
                    CountryId = uk?.Id ?? 2,
                    Address = "25 Oxford Road, Manchester, M13 9PR"
                },
                new Customer 
                { 
                    FirstName = "Hans", 
                    LastName = "Mueller",
                    Email = "hans.mueller@example.com",
                    Phone = "+49-30-5555-6789",
                    CountryId = germany?.Id ?? 7,
                    Address = "Unter den Linden 10, 10117 Berlin"
                },
                new Customer 
                { 
                    FirstName = "Takashi", 
                    LastName = "Yamamoto",
                    Email = "takashi.yamamoto@example.com",
                    Phone = "+81-3-5555-9012",
                    CountryId = japan?.Id ?? 13,
                    Address = "1-1-1 Shibuya, Shibuya-ku, Tokyo 150-0002"
                },
                new Customer 
                { 
                    FirstName = "Maria", 
                    LastName = "Garcia",
                    Email = "maria.garcia@example.com",
                    Phone = "305-555-2345",
                    CountryId = usa?.Id ?? 1,
                    Address = "789 Collins Ave, Miami Beach, FL 33139"
                },
                new Customer 
                { 
                    FirstName = "Robert", 
                    LastName = "Taylor",
                    Email = "robert.taylor@example.com",
                    Phone = "+1-416-555-6789",
                    CountryId = canada?.Id ?? 15,
                    Address = "120 Bloor Street East, Toronto, ON M4W 1B7"
                }
            };
            
            await context.Customers.AddRangeAsync(customers).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}