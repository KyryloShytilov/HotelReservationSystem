using Microsoft.Extensions.DependencyInjection;
using SoapCore;
using SoapCore.Meta;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace new_app.Services;

public static class ServiceMetadataExtension
{
    public static void AddSoapMetadata<TService>(this IServiceCollection services, string path)
        where TService : class
    {
        services.AddSingleton<ServiceMetadataBehavior>();
        services.AddHttpContextAccessor();
        services.AddSoapServiceOperationTuner<CustomOperationContextTuner>();
    }
}

public class CustomOperationContextTuner : IServiceOperationTuner
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CustomOperationContextTuner(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Tune(object serviceInstance, object[] args, OperationDescription operation, Message message)
    {
        // Handle authorization, authentication, and custom headers
        var httpContext = _httpContextAccessor.HttpContext;
        
        if (httpContext == null)
            return;

        // You can add custom logic here to handle authentication/authorization
    }
}