
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace new_app.Models;

public class ApplicationUser : IdentityUser
{
    public string? PhoneNumber { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    // API key property for SOAP authentication
    public string? ApiKey { get; set; }
}