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
    /// Initializes the database with seed data.
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
            await SeedUsersAsync(userManager, roleManager, logger).ConfigureAwait(false);
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
    /// Seeds users into the database with appropriate roles.
    /// </summary>
    private static async Task SeedUsersAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
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
}