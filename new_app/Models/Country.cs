using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.Models;

/// <summary>
/// Represents a country in the hotel reservation system
/// </summary>
public class Country
{
    /// <summary>
    /// The unique identifier for the country
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the country
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}