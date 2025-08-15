using System.ComponentModel.DataAnnotations.

namespace HotelReservationSystem.Models;

public class Customer
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    public DateTime? Birthdate { get; set; }
}