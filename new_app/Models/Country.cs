namespace HotelReservationSystem.Models;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a country entity in the hotel reservation system.
/// </summary>
public class Country
{
    /// <summary>
    /// Gets or sets the unique identifier for the country.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the country.
    /// Must not be null and cannot exceed 50 characters in length.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}