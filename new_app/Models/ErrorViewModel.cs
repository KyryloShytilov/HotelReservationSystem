namespace new_app.Models;

/// <summary>
/// Model used for error handling in the application
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Gets or sets the request ID for the error
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets a value indicating whether the request ID should be shown
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}