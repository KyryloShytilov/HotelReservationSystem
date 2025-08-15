using System.ComponentModel.DataAnnotations;

namespace new_app.Models;

public class Hotel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public Country? Country { get; set; }

    [Required]
    [Display(Name = "Country")]
    public int CountryId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Address { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string PostCode { get; set; } = string.Empty;

    [Required]
    [Range(1, 1000)]
    public decimal PricePerNight { get; set; }
}