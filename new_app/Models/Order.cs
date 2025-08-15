namespace HotelReservationSystem.Models;

public class Order
{
    public int Id { get; set; }

    public Customer Customer { get; set; } = null!;

    public Hotel Hotel { get; set; } = null!;

    public DateTime DateOrdered { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int NumberOfDays { get; set; }

    public decimal FullPrice { get; set; }
}