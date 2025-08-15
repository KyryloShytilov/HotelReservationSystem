using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace new_app.Services;

public class SoapFaultExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SoapFaultExceptionHandler> _logger;

    public SoapFaultExceptionHandler(RequestDelegate next, ILogger<SoapFaultExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SOAP service error occurred");
            
            // Convert regular exceptions to SOAP faults
            if (!(ex is FaultException))
            {
                throw new FaultException(ex.Message);
            }
            
            throw;
        }
    }
}

// Extension method
public static class SoapFaultExceptionHandlerExtensions
{
    public static IApplicationBuilder UseSoapFaultExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SoapFaultExceptionHandler>();
    }
}