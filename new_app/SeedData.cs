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
        // Ensure the database is created and migrations are applied
        await context.Database.MigrateAsync().ConfigureAwait(false);
        
        // Seed roles
        await SeedRolesAsync(roleManager).ConfigureAwait(false);
        
        // Seed admin user
        await SeedAdminUserAsync(userManager).ConfigureAwait(false);
        
        // Seed countries
        await SeedCountriesAsync(context).ConfigureAwait(false);
    }

    /// <summary>
    /// Initializes the database with seed data including logger support.
    /// </summary>
    /// <param name="context">The application database context</param>
    /// <param name="userManager">The ASP.NET Core Identity user manager</param>
    /// <param name="roleManager">The ASP.NET Core Identity role manager</param>
    /// <param name="logger">Optional logger for recording initialization events</param>
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
            await SeedAdminUserAsync(userManager, logger).ConfigureAwait(false);
            await SeedCountriesAsync(context, logger).ConfigureAwait(false);
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
    /// Seeds administrator users into the database.
    /// </summary>
    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        ILogger? logger = null)
    {
        logger?.LogInformation("Seeding application users...");
        
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
                logger?.LogInformation("Creating user: {Email}", admin.Email);
                
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
                        logger?.LogInformation("Assigning admin role to user {Email}", admin.Email);
                        await userManager.AddToRoleAsync(user, RoleName.Admin).ConfigureAwait(false);
                        await userManager.AddToRoleAsync(user, RoleName.HotelManager).ConfigureAwait(false);
                    }
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger?.LogWarning("Failed to create user {Email}. Errors: {Errors}", admin.Email, errors);
                }
            }
        }
        
        logger?.LogInformation("User seeding completed");
    }

    /// <summary>
    /// Seeds administrator users into the database (without logger).
    /// </summary>
    private static Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        return SeedAdminUserAsync(userManager, null);
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
}