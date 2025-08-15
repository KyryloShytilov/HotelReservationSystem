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
    /// Asynchronously initializes the database with seed data.
    /// This method is called from Program.cs.
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
        await SeedRoles(roleManager).ConfigureAwait(false);
        
        // Seed admin user
        await SeedAdminUser(userManager).ConfigureAwait(false);
        
        // Seed countries
        await SeedCountries(context).ConfigureAwait(false);
    }

    /// <summary>
    /// Initializes the database with seed data.
    /// </summary>
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
    private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
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
    private static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
    {
        // Create admin users if they don't exist
        var adminEmails = new[] { "admin@admin.com", "admin@book.go", "guest@book.go" };
        var defaultPassword = "Admin123!";
        
        foreach (var email in adminEmails)
        {
            var existingUser = await userManager.FindByEmailAsync(email).ConfigureAwait(false);
            
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    Phone = "123-456-7890"
                };
                
                await userManager.CreateAsync(user, defaultPassword).ConfigureAwait(false);
                
                // Only add users with "admin" in their email to the Admin role
                if (email.Contains("admin"))
                {
                    await userManager.AddToRoleAsync(user, RoleName.Admin).ConfigureAwait(false);
                }
            }
        }
    }
    
    /// <summary>
    /// Seeds country data into the database.
    /// </summary>
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
                new Country { Name = "India" }
            };
            
            await context.Countries.AddRangeAsync(countries).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}