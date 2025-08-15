using System;
using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.Models;

/// <summary>
/// Represents a customer in the hotel reservation system
/// </summary>
public class Customer
{
    /// <summary>
    /// The unique identifier for the customer
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The full name of the customer
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The birthdate of the customer
    /// </summary>
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? Birthdate { get; set; }
}