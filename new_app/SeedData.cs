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
}