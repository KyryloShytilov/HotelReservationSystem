namespace HotelReservationSystem.Models;

/// <summary>
/// View model for error information display
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Gets or sets the request identifier
    /// </summary>
    /// <value>The unique identifier for the HTTP request</value>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the request identifier should be displayed
    /// </summary>
    /// <value>True if the RequestId is not null or empty; otherwise, false</value>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}