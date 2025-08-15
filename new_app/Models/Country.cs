using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.Models;

public class Country
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}